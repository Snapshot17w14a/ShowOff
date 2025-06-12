using UnityEngine;

public class PlaySoundExit : StateMachineBehaviour
{
    [SerializeField] private ESoundType sound;
    [SerializeField] private string soundName;
    [SerializeField, Range(0, 1)] private float volume = 1f;
    [SerializeField][Range(0, 5)] private float pitch = 1f;

    [SerializeField] private bool randomizePitch = false;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.PlaySound(sound, soundName, randomizePitch, pitch, volume);
    }
}
