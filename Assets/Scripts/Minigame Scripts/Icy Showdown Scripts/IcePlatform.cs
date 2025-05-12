using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    [SerializeField] private Color solidColor;
    [SerializeField] private Color brittleColor;
    [SerializeField] private Color waterColor;

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public bool IsBrittle => isBrittle;
    private bool isBrittle = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        meshRenderer.material.color = solidColor;
    }

    public void FreezePlatform()
    {
        meshCollider.enabled = true;
        meshRenderer.material.color = solidColor;
    }

    public void SetBrittle()
    {
        isBrittle = true;
        meshRenderer.material.color = brittleColor;
    }

    public void BreakIfBrittle()
    {
        if (!isBrittle) return;

        BreakPlatform();
    }

    public void BreakPlatform()
    {
        isBrittle = false;
        meshCollider.enabled = false;
        meshRenderer.material.color = waterColor;
    }
}
