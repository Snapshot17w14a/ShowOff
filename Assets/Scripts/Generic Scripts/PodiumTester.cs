using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PodiumTester : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var registry = Services.Get<PlayerRegistry>();
        registry.CreatePlayerWithDevice(Keyboard.current);
        registry.CreatePlayerWithDevice(Keyboard.current);

        var scoreReg = Services.Get<ScoreRegistry>();

        scoreReg.AddScore(0, Random.Range(0, 4));
        scoreReg.AddScore(1, Random.Range(0, 10));

        PlayerPrefs.SetInt("DoPodium", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene("HubScene");
    }
}
