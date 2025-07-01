using UnityEngine;
using UnityEngine.VFX;

public class DifficultyTrigger : MonoBehaviour
{
    private new Renderer renderer;

    private void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
        ChangeColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MinigamePlayer>(out var player) && player.IsDashing)
        {
            DifficultyManager.SwitchDifficulty();
            ChangeColor();
        }
    }

    private void ChangeColor()
    {
        int mood = DifficultyManager.IsEasyMode() ? 0 : 1;
        GetComponentInChildren<Animator>().SetFloat("Mood", mood);
        renderer.material.SetFloat("_Blend", mood);
        if(mood == 1)
            GetComponentInChildren<VisualEffect>().Play();
        else
            GetComponentInChildren<VisualEffect>().Stop();
    }
}
