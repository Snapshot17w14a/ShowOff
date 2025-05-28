using UnityEngine;

public class PlaySoundExit : StateMachineBehaviour
{
    [SerializeField] private ESoundType sound;
    [SerializeField, Range(0, 1)] private float volume = 1f;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.PlaySound(sound, name, volume);
    }
}
