using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class HubMinecart : MonoBehaviour
{
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private float minecartSpeed;
    [SerializeField] private float minecartSpeedIncrease;
    [SerializeField] private float minecartMaxSpeed;

    [SerializeField] private TextMeshPro scoreText;
    private int currentScore;

    private VisualEffect firework;

    private void Start()
    {
        firework = GetComponent<VisualEffect>();
    }

    private void Update()
    {
        transform.RotateAround(rotatePoint.transform.position, Vector3.up, minecartSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Pickupable>())
        {
            Destroy(other.gameObject);
            currentScore++;
            scoreText.text = currentScore.ToString();

            if (minecartSpeed < minecartMaxSpeed)
            {
                minecartSpeed += minecartSpeedIncrease;
                firework.SetFloat("ParticleCount", currentScore);
                firework.Play();
            }
        }
    }
}
