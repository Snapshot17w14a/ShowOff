using UnityEngine;

public class CutscenePenguinAnims : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void CheckForAnimChange(int index)
    {
        switch(index)
        {
            case 1:
                animator.SetBool("IsHolding", true);
                animator.SetFloat("Magnitude", 1f);
                break;
            case 2:
                animator.SetBool("IsHolding", false);
                break;
            default:
                return;
        }
    }
}
