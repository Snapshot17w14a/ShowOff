using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class HubGoalScoring : MonoBehaviour
{
    [Header("Ball")]
    [SerializeField] private Vector3 ballRespawnPosition;
    [SerializeField] private GameObject ballPrefab;
    private bool hasScored;

    [Header("Assign Please")]
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private VisualEffect partyParticle;
    [SerializeField] private AudioSource partySound;
    private int currentScore;

    private void OnTriggerEnter(Collider other)
    {
        if (hasScored) return;

        if (other.CompareTag("Ball"))
        {
            currentScore++;
            scoreText.text = currentScore.ToString();

            if (partyParticle)
                partyParticle.Play();

            AudioManager.PlaySound(ESoundType.Other, "Goal", false);

            if (partySound)
                partySound.Play();

            hasScored = true;
            Scheduler.Instance.DelayExecution(() =>
            {
                Destroy(other.gameObject);
                Instantiate(ballPrefab, ballRespawnPosition, Quaternion.identity);
                hasScored = false;
            }, 1f);
        }
    }
}
