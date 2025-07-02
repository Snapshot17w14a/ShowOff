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
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            return;
        }
        transform.forward = cameraTransform.forward;
    }
}
