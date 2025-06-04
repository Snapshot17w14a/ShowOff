using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadingZoneMinigame : MonoBehaviour
{
    [SerializeField] private string minigameName;
    [SerializeField] private TextMeshProUGUI countDownTimerText;

    [SerializeField] private CinemachineSplineDolly cinemachineSplineDolly;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Camera cutsceneCamera;

    private int curPlayersReady;
    private float countdownTimer = 3;
    private bool playersReady;

    private bool isTranstitioning = false;
    private bool isLoadingMinigame = false;

    private List<PlayerInput> allPlayerInputs = new List<PlayerInput>();
    private float playerReadyThreshold;

    private void Start()
    {
        ServiceLocator.GetService<PlayerRegistry>().OnPlayerSpawn += AddPlayer;
        ServiceLocator.GetService<PlayerRegistry>().OnPlayerDisconnect += RemovePlayer;
    }
    private void Update()
    {
        if (playersReady && !isTranstitioning)
        {
            countdownTimer -= Time.deltaTime;
            countDownTimerText.text = countdownTimer.ToString("F0");

            if (countdownTimer <= 0 && !isTranstitioning)
            {
                isTranstitioning = true;
                MainCamera.enabled = false;
                cutsceneCamera.enabled = true;
                countDownTimerText.enabled = false;

                var autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
                autodolly.Speed = 0.5f;

                for (int i = 0; i < allPlayerInputs.Count; i++)
                {
                    allPlayerInputs[i].DeactivateInput();
                }
            }
        }

        if (isTranstitioning)
        {
            if (cinemachineSplineDolly.CameraPosition >= 2.3f && !isLoadingMinigame)
            {
                isLoadingMinigame = true;
                TransitionController.Instance.TransitionOut(minigameName);
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            curPlayersReady++;
        }
        
        if (playerReadyThreshold == curPlayersReady)
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
            RecheckThreshold();
        }
    }

    private void AddPlayer(MinigamePlayer player)
    {
        int registeredPlayers = ServiceLocator.GetService<PlayerRegistry>().RegisteredPlayerCount;
        float curPlayers = (float)registeredPlayers;
        playerReadyThreshold = Mathf.Ceil(curPlayers / 2f + 1f);
        RecheckThreshold();

        if (!allPlayerInputs.Contains(player.GetComponent<PlayerInput>()))
            allPlayerInputs.Add(player.GetComponent<PlayerInput>());
    }

    private void RemovePlayer(int curPlayers)
    {
        float currentPlayers = (float)curPlayers;
        playerReadyThreshold = Mathf.Ceil(currentPlayers / 2f + 1f);

        RecheckThreshold();
    }

    private void RecheckThreshold()
    {
        Debug.Log("Current Player Threshold to start the game: " + playerReadyThreshold);

        playersReady = false;
        countdownTimer = 3;
        countDownTimerText.enabled = false;

        if (playerReadyThreshold == curPlayersReady)
        {
            playersReady = true;
            countDownTimerText.enabled = true;
        }
    }

    private void OnDisable()
    {
        ServiceLocator.GetService<PlayerRegistry>().OnPlayerSpawn -= AddPlayer;
        ServiceLocator.GetService<PlayerRegistry>().OnPlayerDisconnect -= RemovePlayer;
    }
}
