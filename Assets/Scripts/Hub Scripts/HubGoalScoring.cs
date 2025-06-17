using System.Collections;
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
        if (other.CompareTag("Ball") && !hasScored)
        {
            currentScore++;
            scoreText.text = currentScore.ToString();

            if (partyParticle)
                partyParticle.Play();

            if (partySound)
                partySound.Play();

            hasScored = true;
            StartCoroutine(RespawnBall(other.gameObject));
        }
    }

    private IEnumerator RespawnBall(GameObject curBall)
    {
        yield return new WaitForSeconds(1f);
        Destroy(curBall);
        Instantiate(ballPrefab, ballRespawnPosition, Quaternion.identity);
        hasScored = false;
    }
}
