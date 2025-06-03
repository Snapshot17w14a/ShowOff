using UnityEngine;

public class CameraLerp : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private float duration;
    [SerializeField] private Transform startTransform;
    [SerializeField] private GameObject swap;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isLerpingAllowed = false;
    private float time = 0;

    private void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (!isLerpingAllowed) return;

        time += Time.deltaTime / duration;
        var delayedTime = Mathf.Max(0, time - delay);

        transform.SetPositionAndRotation(Vector3.Lerp(startTransform.position, initialPosition, delayedTime), Quaternion.Lerp(startTransform.rotation, initialRotation, delayedTime));

        if (delayedTime >= 1f)
        {
            isLerpingAllowed = false;
            swap.SetActive(true);
        }
    }

    public void StartLerping()
    {
        time = 0;
        isLerpingAllowed = true;
        swap.SetActive(false);
    }
}
