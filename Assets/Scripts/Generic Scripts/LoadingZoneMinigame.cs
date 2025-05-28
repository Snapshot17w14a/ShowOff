using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingZoneMinigame : MonoBehaviour
{
    [SerializeField] private string minigameName;
    [SerializeField] private TextMeshProUGUI countDownTimerText;

    private int curPlayersReady;
    private float countdownTimer = 3;
    private bool playersReady;

    private bool isTranstitioning = false;

    private void Update()
    {
        if (playersReady && !isTranstitioning)
        {
            countdownTimer -= Time.deltaTime;
            countDownTimerText.text = countdownTimer.ToString("F0");

            if (countdownTimer <= 0)
            {
                //SceneManager.LoadScene(minigameName);
                isTranstitioning = true;
                TransitionController.Instance.TransitionOut("Icy Showdown");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            curPlayersReady++;
        }

        if (ServiceLocator.GetService<PlayerRegistry>().RegisteredPlayerCount == curPlayersReady && curPlayersReady >= 2)
        {
            playersReady = true;
            countDownTimerText.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            curPlayersReady--;
            playersReady = false;
            countdownTimer = 3;
            countDownTimerText.enabled = false;
        }
    }
}
