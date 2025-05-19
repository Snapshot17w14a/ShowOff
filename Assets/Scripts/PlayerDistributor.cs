using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDistributor : MonoBehaviour
{
    private const float Tau = 2 * Mathf.PI;

    [SerializeField] private InputActionAsset inputActions;

    private List<(PlayerInput, RegisteredPlayer)> inputsToUpdate;

    public void InstantiatePlayersInCircle(float radius)
    {
        var playerRegistry = ServiceLocator.GetService<PlayerRegistry>();
        playerRegistry.InstantiateAllPlayers();

        int playerCount = playerRegistry.RegisteredPlayerCount;
        float increment = 1 / (float)playerCount;
        float t = increment;

        //foreach(var player in playerRegistry.AllPlayers)
        //{
        //    if (player == null) continue;
        //    player.transform.position = new Vector3(Mathf.Cos(t * Tau), 1, Mathf.Sin(t * Tau)) * radius;

        //    t += increment;
        //}

        inputsToUpdate = new(playerCount);

        foreach (var player in playerRegistry.AllPlayers)
        {
            if (RegisteredPlayer.IsNull(player)) continue;

            var minigamePlayer = player.minigamePlayer;
            minigamePlayer.transform.position = new Vector3(Mathf.Cos(t * Tau), 1, Mathf.Sin(t * Tau)) * radius;
            //minigamePlayer.transform.parent = transform;

            //(PlayerInput, RegisteredPlayer) tuple = (minigamePlayer.GetComponent<PlayerInput>(), player);
            //inputsToUpdate.Add(tuple);

            t += increment;
        }

        //StartCoroutine(LateUpdateInputs());
    }

    //private IEnumerator LateUpdateInputs()
    //{
    //    yield return null;

    //    foreach (var obj in inputsToUpdate)
    //    {
    //        obj.Item1.currentActionMap = inputActions.FindActionMap("Player");

    //        obj.Item1.SwitchCurrentControlScheme(
    //            controlScheme: obj.Item2.controlScheme,
    //            devices: new[] { obj.Item2.device }
    //        );
    //    }

    //    inputsToUpdate = null;
    //}
}
