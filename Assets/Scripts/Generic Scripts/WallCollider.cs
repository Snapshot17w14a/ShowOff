using UnityEngine;

public class WallCollider : MonoBehaviour
{
    private void Start()
    {
        ToggleCollider();
    }
    private void ToggleCollider()
    {
        if (DifficultyManager.IsEasyMode())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
