using UnityEngine;

public class BillboardText : MonoBehaviour
{
    Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        transform.forward = cameraTransform.forward;
    }
}
