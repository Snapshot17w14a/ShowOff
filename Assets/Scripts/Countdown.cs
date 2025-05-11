using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Countdown : MonoBehaviour
{
    [Header("Time settings")]
    [SerializeField] private int countdownSeconds;
    [SerializeField] private bool startOnUnityStart = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onContdownStart;
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
        onContdownStart?.Invoke();
    }

    private void Update()
    {
        if (!isCountdownStarted) return;
        currentSeconds -= Time.deltaTime;
        var numString = Mathf.RoundToInt(currentSeconds).ToString();
        countdownText.text = numString == "0" ? "GO!" : numString;
        if (currentSeconds <= 0)
        {
            onContdownEnd?.Invoke();
            isCountdownStarted = false;
        }
    }
}
