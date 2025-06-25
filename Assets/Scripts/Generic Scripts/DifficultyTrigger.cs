using UnityEngine;

public class DifficultyTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MinigamePlayer>() != null)
        {
            DifficultyManager.SwitchDifficulty();
            ColorChange();
        }
    }

    private void ColorChange()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (DifficultyManager.IsEasyMode())
        {
            renderer.material.color = Color.blue;
        }
        else
        {
            renderer.material.color = Color.red;

        }
    }
}
