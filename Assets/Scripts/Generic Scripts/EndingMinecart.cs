using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class EndingMinecart : MonoBehaviour
{
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform jumpPoint;
    [SerializeField] private Transform[] playerSitPositions;

    [SerializeField] private GameObject minecart;

    [SerializeField] private float minecartLerpTime = 2f;
    [SerializeField] private float playerWalkTime = 1f;
    [SerializeField] private float playerJumpTime = 2f;

    [SerializeField] private UnityEvent OnMinecartLeft;

    [SerializeField] private float cameraSpeed = 0.5f;
    [SerializeField] private CinemachineSplineDolly cinemachineSplineDolly;

    private Vector3 minecartPosition;
    private int playerCount;

    private void Start()
    {
        minecartPosition = minecart.transform.position;

        PlayerRegistry playerReg = Services.Get<PlayerRegistry>();
        playerCount = playerReg.RegisteredPlayerCount;
        playerReg.ExecuteForEachPlayer(player =>
        {
            player.SetInputEnabled(false);
        });

        StartExiting();
    }

    public void StartExiting()
    {
        Scheduler.Instance.Lerp(t => LerpMinecart(1 - t), minecartLerpTime, StartGatheringPlayers);
    }
    
    private void LerpMinecart(float t)
    {
        minecart.transform.position = Vector3.Lerp(minecartPosition, exitPoint.position, t);
    }

    private void StartGatheringPlayers()
    {
        Span<RegisteredPlayer> players = Services.Get<PlayerRegistry>().AllPlayers;

        for (int i = 0; i < playerCount; i++)
        {
            var player = players[i].minigamePlayer;
            if (i != 0)
                Scheduler.Instance.DelayExecution(() => LerpPlayerToMinecart(player), playerWalkTime * player.RegistryID);
            else
                LerpPlayerToMinecart(player);
        }
    }

    private void LerpPlayerToMinecart(MinigamePlayer player)
    {
        Vector3 initialPlayerPos = player.transform.position;

        Scheduler.Instance.Lerp(t => {
            player.transform.position = Vector3.Lerp(initialPlayerPos, jumpPoint.position, t);
            player.transform.forward = GetForwardToMinecart(player.transform.position);
        }, 
        playerWalkTime, 
        () => {
            JumpPlayer(player);
            if (player.RegistryID == playerCount - 1)
                Scheduler.Instance.DelayExecution(LeaveArena, playerJumpTime + .3f);
        });
    }

    private void JumpPlayer(MinigamePlayer player)
    {
        var force = PathCalculator.CalculateRequiredVelocity(player.transform.position, playerSitPositions[player.RegistryID].position, playerJumpTime);
        var rb = player.GetComponent<Rigidbody>();
        rb.linearDamping = 0;
        rb.linearVelocity = force;
        player.GetPlayerAnimator.SetTrigger("Stun");
        Scheduler.Instance.DelayExecution(() =>
        {
            rb.isKinematic = true;
            player.transform.SetParent(minecart.transform);
            player.transform.position = playerSitPositions[player.RegistryID].position;
            player.GetPlayerAnimator.SetTrigger("StunOver");
        }, playerJumpTime);
    }

    private void LeaveArena()
    {
        var autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        autodolly.Speed = cameraSpeed;

        Scheduler.Instance.Lerp(t => {
            transform.position = Vector3.Lerp(minecartPosition, exitPoint.position, t);
        }, minecartLerpTime, () => OnMinecartLeft?.Invoke());
    }

    private Vector3 GetForwardToMinecart(Vector3 playerPosition)
    {
        playerPosition.y = transform.position.y;
        return transform.position - playerPosition;
    }
}
