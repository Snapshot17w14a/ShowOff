using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    private List<GameObject> allPlayers = new List<GameObject>();
    private List<int> players = new();
    private float playerReadyThreshold;
    SplineAutoDolly.FixedSpeed autodolly;

    private void Start()
    {
        ServiceLocator.GetService<PlayerRegistry>().OnPlayerSpawn += AddPlayer;
        ServiceLocator.GetService<PlayerRegistry>().OnPlayerDisconnect += RemovePlayer;
        autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
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
                autodolly.Speed = 0.5f;

                for (int i = 0; i < allPlayerInputs.Count; i++)
                {
                    if (allPlayers[i] != null)
                    {
                        allPlayerInputs[i].DeactivateInput();
                        allPlayers[i].transform.Find("Indicator").gameObject.SetActive(false);
                    }
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

        //Call this with the player controller idk how to do that
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkipCustcene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            curPlayersReady++;
            players.Add(other.GetComponent<MinigamePlayer>().RegistryID);
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
            players.Remove(other.GetComponent<MinigamePlayer>().RegistryID);

            curPlayersReady--;
            RecheckThreshold();
        }
    }

    private void SkipCustcene()
    {
        autodolly.Speed = 10f;
    }

    private void AddPlayer(MinigamePlayer player)
    {

        int registeredPlayers = ServiceLocator.GetService<PlayerRegistry>().RegisteredPlayerCount;
        float curPlayers = (float)registeredPlayers;
        playerReadyThreshold = Mathf.Ceil(curPlayers / 2f + 1f);
        RecheckThreshold();
        

        allPlayers.Add(player.gameObject);
        allPlayerInputs.Add(player.GetComponent<PlayerInput>());
                 
    }

    private void RemovePlayer(int curPlayers)
    {
        if (players.Contains(curPlayers))
            curPlayersReady--;

        playerReadyThreshold = Mathf.Ceil((float)curPlayersReady / 2f + 1f);

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
