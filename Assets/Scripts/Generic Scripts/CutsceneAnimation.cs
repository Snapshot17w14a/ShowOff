using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Curve))]
public class CutsceneAnimation : MonoBehaviour
{
    [SerializeField] private float animationDuration;
    [SerializeField] private int listenerIndex;
    [SerializeField] private bool skipLastSegment;
    [SerializeField] private bool onlyAnimateOnce;

    [SerializeField] private CutsceneAnimation[] segmentChangesToListenTo;

    private Curve curve;

    private float[] segmentTimings;
    private bool animate = false;
    private float time = 0;
    private int targetIndex = 1;
    private bool isAnimationDone = false;
    private Vector3 initialPosition;

    [HideInInspector] public UnityEvent<int> OnSegmentChange;
    public UnityEvent OnAnimationEnd;

    public bool IsAnimating => animate;

    private void Awake()
    {
        curve = GetComponent<Curve>();

        float magnitudeSum = 0;

        for (int i = 0; i < curve.points.Count; i++)
        {
            magnitudeSum += Vector3.Distance(curve.points[i], curve.points[(i + 1) % curve.points.Count]);
        }

        segmentTimings = new float[curve.points.Count];

        for (int i = 0; i < segmentTimings.Length; i++)
        {
            segmentTimings[i] = Vector3.Distance(curve.points[i], curve.points[(i + 1) % curve.points.Count]) / magnitudeSum;
        }

        initialPosition = transform.position;
        foreach (var animScript in segmentChangesToListenTo) animScript.OnSegmentChange.AddListener(ListenToSegmentChange);

        gameObject.SetActive(false);
    }

    public void StartAnimation()
    {
        animate = true;
        transform.forward = (curve.points[targetIndex % curve.points.Count] - curve.points[targetIndex - 1]).normalized;
        OnSegmentChange?.Invoke(targetIndex);
    }

    public void ListenToSegmentChange(int targetIndex)
    {
        if (targetIndex != listenerIndex) return;

        gameObject.SetActive(true);
        StartAnimation();
    }

    private void Update()
    {
        if (!animate || isAnimationDone) return;

        time += Time.deltaTime * (1 / segmentTimings[targetIndex - 1]) / animationDuration;

        transform.position = initialPosition + Vector3.Lerp(curve.points[targetIndex - 1], curve.points[targetIndex % curve.points.Count], time);
        if (time >= 1)
        {
            if (targetIndex == (skipLastSegment ? segmentTimings.Length - 1 : segmentTimings.Length))
            {
                if (onlyAnimateOnce) isAnimationDone = true;

                animate = false;
                targetIndex = 1;
                OnAnimationEnd?.Invoke();
                return;
            }

            time = 0;
            targetIndex++;

            transform.forward = (curve.points[targetIndex % curve.points.Count] - curve.points[targetIndex - 1]).normalized;

            OnSegmentChange.Invoke(targetIndex);
        }
    }
}
