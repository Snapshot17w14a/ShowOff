using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.VFX;

public class HubGoalScoring : MonoBehaviour
{
    [Header("Ball")]
    [SerializeField] private Vector3 ballRespawnPosition;
    [SerializeField] private GameObject ballPrefab;

    [Header("Assign Please")]
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private VisualEffect partyParticle;
    [SerializeField] private AudioSource partySound;
    private int currentScore; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            currentScore++;
            scoreText.text = currentScore.ToString();

            if (partyParticle)
                partyParticle.Play();

            if (partySound)
                partySound.Play();

            StartCoroutine(RespawnBall(other.gameObject));
        }
    }

    private IEnumerator RespawnBall(GameObject curBall)
    {
        yield return new WaitForSeconds(1f);
        Destroy(curBall);
        Instantiate(ballPrefab, ballRespawnPosition, Quaternion.identity);
    }
}
