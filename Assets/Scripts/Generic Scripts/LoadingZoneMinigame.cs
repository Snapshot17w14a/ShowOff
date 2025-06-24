using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadingZoneMinigame : MonoBehaviour
{
    [SerializeField] private string minigameName;
    [SerializeField] private TextMeshProUGUI countDownTimerText;
    [SerializeField] private GameObject skipButton;

    [SerializeField] private CinemachineSplineDolly cinemachineSplineDolly;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Camera cutsceneCamera;

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

        registry.OnPlayerSpawn += AddPlayer;
        registry.BeforePlayerDisconnect += RemovePlayer;
        autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;

        readyPlayers = new(registry.MaxPlayers);

        playerReadyThreshold = Mathf.Ceil((float)registry.RegisteredPlayerCount / 2f + 1f);
        AudioManager.PlayMusic(ESoundType.Music, "Icy_Showdown", 0.3f);

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
                autodolly.Speed = 0.5f;
                skipButton.SetActive(true);

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            readyPlayers.Remove(other.GetComponent<MinigamePlayer>());
            RecheckThreshold();
        }
    }

    private void AddPlayer(MinigamePlayer player)
    {
        int registeredPlayers = Services.Get<PlayerRegistry>().RegisteredPlayerCount;
        float curPlayers = (float)registeredPlayers;
        playerReadyThreshold = Mathf.Ceil(curPlayers / 2f + 1f);
        RecheckThreshold();
    }

    private void RemovePlayer(MinigamePlayer disconnectingPlayer)
    {
        if (readyPlayers.Contains(disconnectingPlayer)) readyPlayers.Remove(disconnectingPlayer);

        playerReadyThreshold = Mathf.Ceil((float)readyPlayers.Count / 2f + 1f);

        RecheckThreshold();
    }

    private void RecheckThreshold()
    {
        Debug.Log("Current Player Threshold to start the game: " + playerReadyThreshold);

        playersReady = false;
        countdownTimer = 3;
        countDownTimerText.enabled = false;

        if (playerReadyThreshold == readyPlayers.Count)
        {
            playersReady = true;
            countDownTimerText.enabled = true;
        }
    }

    private void OnDisable()
    {
        Services.Get<PlayerRegistry>().OnPlayerSpawn -= AddPlayer;
        Services.Get<PlayerRegistry>().BeforePlayerDisconnect -= RemovePlayer;
        skipInput.performed -= SkipCutscene;
    }

    private void SkipCutscene(InputAction.CallbackContext ctx)
    {
        if (isTranstitioning)
        {
            autodolly.Speed = 10f;
        }
    }

    private void OnEnable()
    {
        playerReadyThreshold = Mathf.Ceil((float)Services.Get<PlayerRegistry>().RegisteredPlayerCount / 2f + 1f);
    }
}
