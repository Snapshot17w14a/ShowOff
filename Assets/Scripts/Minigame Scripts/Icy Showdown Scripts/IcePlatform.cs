using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    [SerializeField] private Texture solidTexture;
    [SerializeField] private Texture brittleTexture;
    [SerializeField] private Texture waterTexture;

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public bool IsBrittle => isBrittle;
    private bool isBrittle = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        meshRenderer.material.mainTexture = solidTexture;
    }

    public void FreezePlatform()
    {
        meshCollider.enabled = true;
        meshRenderer.material.mainTexture = solidTexture;
    }

    public void SetBrittle()
    {
        isBrittle = true;
        meshRenderer.material.mainTexture = brittleTexture;
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
        meshRenderer.material.mainTexture = waterTexture;
    }
}
