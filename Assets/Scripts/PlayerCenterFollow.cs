using UnityEngine;

public class PlayerCenterFollow : MonoBehaviour
{
    private Transform[] playerTransforms;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var players = FindObjectsByType<MinigamePlayer>(FindObjectsSortMode.None);
        playerTransforms = new Transform[players.Length];
        for(int i = 0; i < players.Length; i++)
        {
            playerTransforms[i] = players[i].transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, )
    }

    private Vector3 AvragePlayerPosition()
    {
        float sumX = 0, sumY = 0, sumZ = 0;

        foreach(var transform in playerTransforms)
        {
            var pos = transform.position;
            sumX += pos.x;
            sumY += pos.y;
            sumZ += pos.z;
        }
    }
}
