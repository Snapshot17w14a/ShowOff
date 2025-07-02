using UnityEngine;
using UnityEngine.VFX;

public class DifficultyTrigger : MonoBehaviour
{
    private new Renderer renderer;
    [SerializeField] private GameObject dashUIIndication;

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
        {
            GetComponentInChildren<VisualEffect>().Play();
            
            if (dashUIIndication)
            {
                dashUIIndication.SetActive(true);
            }
        }
        else
        {
            GetComponentInChildren<VisualEffect>().Stop();

            if (dashUIIndication)
            {
                dashUIIndication.SetActive(false);
            }
        }
    }
}
