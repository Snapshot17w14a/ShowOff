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
        transform.LookAt(cameraTransform, Vector3.up);
        transform.rotation *= Quaternion.Euler(0, 180, 0);
    }
}
