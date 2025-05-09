using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Countdown : MonoBehaviour
{
    [Header("Time settings")]
    [SerializeField] private int countdownSeconds;
    [SerializeField] private bool startOnUnityStart = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onContdownStarted;
    [SerializeField] private UnityEvent onContdownEnd;

    private float currentSeconds;
    private bool isCountdownStarted = false;
    private TextMeshProUGUI countdownText;

    private void Start()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
        if (startOnUnityStart) StartCountdown();
    }

    public void StartCountdown()
    {
        currentSeconds = countdownSeconds;
        isCountdownStarted = true;
        onContdownStarted?.Invoke();
    }

    private void Update()
    {
        if (!isCountdownStarted) return;
        currentSeconds -= Time.deltaTime;
        countdownText.text = Mathf.RoundToInt(currentSeconds).ToString();
        if (currentSeconds <= 0)
        {
            onContdownEnd?.Invoke();
            isCountdownStarted = false;
        }
    }
}
