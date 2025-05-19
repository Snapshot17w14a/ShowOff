using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    [SerializeField] private Color solidColor;
    [SerializeField] private Color brittleColor;
    [SerializeField] private Color waterColor;

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public bool IsBrittle { get; private set; } = false;

    public bool IsBroken => !meshCollider.enabled;


    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        meshRenderer.material.color = solidColor;
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
        meshRenderer.enabled = false;
        meshRenderer.material.color = waterColor;
    }
}
