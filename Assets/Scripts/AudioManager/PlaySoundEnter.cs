using UnityEngine;

public class PlaySoundEnter : StateMachineBehaviour
{
    [SerializeField] private ESoundType sound;
    [SerializeField, Range(0, 1)] private float volume = 1f; 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
