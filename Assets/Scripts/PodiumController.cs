using UnityEngine;

public class PodiumController : MonoBehaviour
{
    [SerializeField] private GameObject podiumPrefab;
    [SerializeField] private float spacing;

    private Podium[] podiums;

    public void StartPodiumSequence()
    {
        var registry = ServiceLocator.GetService<PlayerRegistry>();
        int playerCount = registry.RegisteredPlayerCount;

        for (int i = 0; i < playerCount; i++)
        {
            var podium = CreatePodium(i);
            podiums[i] = podium;
        }

        var parentPos = transform.position;
        parentPos.x = 0.5f * playerCount + spacing * Mathf.Max(playerCount - 1, 0) / 2f;

        PlayerPrefs.SetInt("DoPodium", 0);
    }

    private Podium CreatePodium(int index)
    {
        var pos = new Vector3(0.5f * (index + 1) + spacing * index, 10, 0);
        var podium = Instantiate(podiumPrefab, pos, Quaternion.identity, transform).GetComponent<Podium>();
        podium.player = ServiceLocator.GetService<PlayerRegistry>().InstantiatePlayerWithId(index);

        return podium;
    }
}
