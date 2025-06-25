using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class IcePlatform : MonoBehaviour
{
    [SerializeField] private Material solidMaterial;
    [SerializeField] private Material brittleMaterial;

    [SerializeField] private float sinkTime = 1f;
    [SerializeField] private float freezeTime = 1f;
    [SerializeField] private AnimationCurve falloffSpeed;

    private Vector3 startingPosition;
    private Vector3 sinkPosition;

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private VisualEffect freezeEffect;
    private Coroutine currentRoutine;

    public bool IsBrittle { get; private set; } = false;

    public bool IsBroken => !meshCollider.enabled;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        startingPosition = transform.position;

        sinkPosition = startingPosition + new Vector3(0, -8, 0);

        freezeEffect = GetComponent<VisualEffect>();
        freezeEffect.Reinit();
    }

    public void FreezePlatform()
    {
        if (IsBroken)
        {
            freezeEffect.Reinit();
            freezeEffect.Play();
            AudioManager.PlaySound(ESoundType.Environment, "Ice_Regrowing", false);

            if (currentRoutine != null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(FreezeRoutine());
        }

        meshCollider.enabled = true;
        meshRenderer.enabled = true;

        meshRenderer.material = solidMaterial;

        IsBrittle = false;
    }

    public void SetBrittle()
    {
        IsBrittle = true;
        meshRenderer.material = brittleMaterial;
    }

    public void BreakIfBrittle()
    {
        if (!IsBrittle) return;

        BreakPlatform();
    }

    public void BreakPlatform()
    {
        IsBrittle = false;
        meshCollider.enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
            AudioManager.PlaySound(ESoundType.Environment, "Platform_Break", false);
        }

        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(SinkPlatform());
    }

    private IEnumerator SinkPlatform()
    {
        float time = 0;

        while (time < sinkTime)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPosition, sinkPosition, falloffSpeed.Evaluate(time / sinkTime));
            yield return null;
        }

        meshRenderer.enabled = false;
        transform.position = startingPosition;
    }

    private IEnumerator FreezeRoutine()
    {
        float time = 0;
        transform.position = startingPosition;

        while (time < freezeTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Lerp(0, 1, time / freezeTime);
            transform.localScale = new Vector3(t, t, t);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}
