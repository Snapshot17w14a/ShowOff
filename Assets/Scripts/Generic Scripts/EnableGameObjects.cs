using UnityEngine;

public class EnableGameObjects : MonoBehaviour
{
    [SerializeField] private GameObject crystals;

    //Here is enable
    private void Start()
    {
        EnableWallColliders();
        EnableCrystals();
    }
    private void EnableWallColliders()
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

    private void EnableCrystals()
    {
        if(crystals != null)
        {
            if (DifficultyManager.IsEasyMode())
            {
                crystals.SetActive(false);
            }
            else
            {
                crystals.SetActive(true);
            }
        }
    }
}
