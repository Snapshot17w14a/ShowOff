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
    [SerializeField] private GameObject skipIndicator;

    [SerializeField] private InputAction skipInput;

    private float countdownTimer = 3;
    private bool playersReady;

    private bool isTranstitioning = false;
    private bool isLoadingMinigame = false;

    private List<MinigamePlayer> readyPlayers;
    private float playerReadyThreshold;
    SplineAutoDolly.FixedSpeed autodolly;

    private void Start()
    {
        var registry = Services.Get<PlayerRegistry>();

        registry.OnPlayerRegistered += AddPlayer;
        registry.OnBeforePlayerDisconnect += RemovePlayer;
        registry.OnAfterPlayerDisconnect += AfterDisconnect;
        autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;

        readyPlayers = new(registry.MaxPlayers);

        CalcThreshold();
        AudioManager.PlayMusic(ESoundType.Music, "Hub", 0.3f);

        skipInput.performed += SkipCutscene;
        skipInput.Enable();
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
                skipIndicator.SetActive(true);
                autodolly.Speed = 0.5f;

                Services.Get<PlayerRegistry>().ExecuteForEachPlayer(player =>
                {
                    player.transform.Find("Indicator").gameObject.SetActive(false);
                    player.GetComponent<PlayerInput>().DeactivateInput();
                });
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
            readyPlayers.Add(other.GetComponent<MinigamePlayer>());

        if (playerReadyThreshold == readyPlayers.Count)
        {
            playersReady = true;
            countDownTimerText.enabled = true;
            AudioManager.PlaySound(ESoundType.Other, "CountDown_Hub", false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            readyPlayers.Remove(other.GetComponent<MinigamePlayer>());
            RecheckThreshold();
            AudioManager.StopSound();
        }
    }

    private void AddPlayer(int id)
    {
        CalcThreshold();
        RecheckThreshold();
    }

    private void RemovePlayer(MinigamePlayer disconnectingPlayer)
    {
        if (readyPlayers.Contains(disconnectingPlayer)) readyPlayers.Remove(disconnectingPlayer);

        RecheckThreshold();
    }

    private void CalcThreshold()
    {
        int registeredPlayers = Services.Get<PlayerRegistry>().RegisteredPlayerCount;
        float curPlayers = registeredPlayers;
        playerReadyThreshold = Mathf.Ceil(curPlayers / 2f);
    }

    private void RecheckThreshold()
    {
        Debug.Log("Current Player Threshold to start the game: " + playerReadyThreshold);
        if (readyPlayers.Count == 0)
        {
            playersReady = false;
            countdownTimer = 3;
            countDownTimerText.enabled = false;
            return;
        }

        playersReady = false;
        countdownTimer = 3;
        countDownTimerText.enabled = false;

        if (readyPlayers.Count >= Mathf.Max(1, playerReadyThreshold))
        {
            playersReady = true;
            countDownTimerText.enabled = true;
        }
    }

    private void OnDisable()
    {
        var registry = Services.Get<PlayerRegistry>();
        registry.OnPlayerRegistered -= AddPlayer;
        registry.OnBeforePlayerDisconnect -= RemovePlayer;
        registry.OnAfterPlayerDisconnect -= AfterDisconnect;
        skipInput.performed -= SkipCutscene;
    }

    private void SkipCutscene(InputAction.CallbackContext ctx)
    {
        if (isTranstitioning)
        {
            autodolly.Speed = 10f;
        }
    }

    private void AfterDisconnect(int id)
    {
        CalcThreshold();
        RecheckThreshold();
    }

    private void OnEnable()
    {
        CalcThreshold();
    }
}
