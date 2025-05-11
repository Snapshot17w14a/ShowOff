using UnityEngine;

public class PlayerCenterFollow : MonoBehaviour
{
    [SerializeField] private float sensitivity = 1;
    private Transform[] playerTransforms;
    private Vector3 initialOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var players = FindObjectsByType<MinigamePlayer>(FindObjectsSortMode.None);
        playerTransforms = new Transform[players.Length];
        for(int i = 0; i < players.Length; i++)
        {
            playerTransforms[i] = players[i].transform;
        }

        initialOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var players = FindObjectsByType<MinigamePlayer>(FindObjectsSortMode.None);
        playerTransforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerTransforms[i] = players[i].transform;
        }
        transform.position = Vector3.Slerp(transform.position, AveragePlayerPosition(), Time.deltaTime * sensitivity);
    }

    private Vector3 AveragePlayerPosition()
    {
        if (playerTransforms.Length == 0) return initialOffset;

        float sumX = 0, sumY = 0, sumZ = 0;
        float numTransform = playerTransforms.Length;

        foreach (var transform in playerTransforms)
        {
            var pos = transform.position;
            sumX += pos.x;
            sumY += pos.y;
            sumZ += pos.z;
        }

        return new Vector3(sumX / numTransform, sumY / numTransform, sumZ / numTransform) + initialOffset;
    }
}
