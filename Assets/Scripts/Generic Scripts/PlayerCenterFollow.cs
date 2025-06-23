using System.Collections.Generic;
using UnityEngine;

public class PlayerCenterFollow : MonoBehaviour
{
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float shakeAmount = 1f;

    private List<Transform> playerTransforms;
    private Vector3 initialOffset;

    private bool isShaking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        PlayerRegistry registry = Services.Get<PlayerRegistry>();

        playerTransforms = new(registry.MaxPlayers);
        registry.OnPlayerSpawn += AddPlayerTransform;

        initialOffset = transform.position;
    }

    private void OnDestroy()
    {
        Services.Get<PlayerRegistry>().OnPlayerSpawn -= AddPlayerTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking) return;
        else transform.position = Vector3.Slerp(transform.position, AveragePlayerPosition(), Time.deltaTime * sensitivity);
    }

    private Vector3 AveragePlayerPosition()
    {
        if (playerTransforms.Count == 0) return initialOffset;

        float sumX = 0, sumY = 0, sumZ = 0;
        float numTransform = playerTransforms.Count;

        foreach (var transform in playerTransforms)
        {
            var pos = transform.position;
            sumX += pos.x;
            sumY += pos.y;
            sumZ += pos.z;
        }

        return new Vector3(sumX / numTransform, sumY / numTransform, sumZ / numTransform) + initialOffset;
    }

    private void CalculateShake(float t)
    {
        var offset = (transform.up + transform.right).normalized;
        float maxShake = Mathf.Lerp(0, shakeAmount, 1 - t);

        offset.Set(
            offset.x * Random.Range(-1f, 1f) * maxShake,
            offset.y * Random.Range(-1f, 1f) * maxShake,
            0
        );

        transform.position = AveragePlayerPosition() + offset;
    }

    public void ShakeCamera(float time)
    {
        isShaking = true;
        var currentCameraPos = transform.position;
        Scheduler.Instance.Lerp(
            t => transform.position = Vector3.Lerp(currentCameraPos, AveragePlayerPosition(), t),
            0.15f,
            () => Scheduler.Instance.Lerp(CalculateShake, time, () => isShaking = false)
        );
    }

    private void AddPlayerTransform(MinigamePlayer player) => playerTransforms.Add(player.transform);
}
