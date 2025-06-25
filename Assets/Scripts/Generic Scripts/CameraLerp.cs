using UnityEngine;

public class CameraLerp : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private float duration;
    [SerializeField] private Transform startTransform;
    [SerializeField] private GameObject swap;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void StartLerping()
    {
        transform.SetPositionAndRotation(startTransform.position, startTransform.rotation);
        Scheduler.Instance.DelayExecution(() =>
        {
            Scheduler.Instance.Lerp(t =>
            {
                transform.SetPositionAndRotation(Vector3.Lerp(startTransform.position, initialPosition, t), Quaternion.Lerp(startTransform.rotation, initialRotation, t));
            }, duration, () => swap.SetActive(true));
        }, delay);
    }
}
