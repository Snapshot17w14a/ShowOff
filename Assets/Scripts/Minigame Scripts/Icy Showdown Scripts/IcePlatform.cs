using System.Collections;
using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    [SerializeField] private Color solidColor;
    [SerializeField] private Color brittleColor;
    [SerializeField] private Color waterColor;

    [SerializeField] private float sinkTime = 1f;

    private Vector3 startingPosition;
    private Vector3 sinkPosition;

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public bool IsBrittle { get; private set; } = false;

    public bool IsBroken => !meshCollider.enabled;


    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        startingPosition = transform.position;

        sinkPosition = startingPosition + new Vector3(0, -8, 0);
    }

    public void FreezePlatform()
    {
        IsBrittle = false;
        meshCollider.enabled = true;
        meshRenderer.enabled = true;

        meshRenderer.material.color = solidColor;
    }

    public void SetBrittle()
    {
        IsBrittle = true;
        meshRenderer.material.color = brittleColor;
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
        }

        StartCoroutine(SinkPlatform());
    }

    private IEnumerator SinkPlatform()
    {
        float time = 0;

        while (time < sinkTime)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPosition, sinkPosition, time / sinkTime);
            yield return null;
        }

        meshRenderer.enabled = false;
        transform.position = startingPosition;
    }
}
