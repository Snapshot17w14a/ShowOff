using UnityEngine;

public class PlaySoundEnter : StateMachineBehaviour
{
    [SerializeField] private ESoundType sound;
    [SerializeField] private string soundName;
    [SerializeField, Range(0, 1)] private float volume = 1f;
    [SerializeField] private bool randomizePitch = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.PlaySound(sound, soundName, randomizePitch, volume);
    }
}
