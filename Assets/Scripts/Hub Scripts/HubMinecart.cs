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

    [SerializeField] private VisualEffect firework;
    [SerializeField] private VisualEffect goldenShine;

    private void Update()
    {
        transform.RotateAround(rotatePoint.transform.position, Vector3.up, minecartSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Pickupable>())
        {
            Destroy(other.gameObject);

            if (other.gameObject.GetComponent<Pickupable>().PickupType == PickupType.Large)
            {
                currentScore += 3;
                if (minecartSpeed < minecartMaxSpeed)
                    minecartSpeed += minecartSpeedIncrease * 3;
            }
            else
            {
                currentScore++;
                if (minecartSpeed < minecartMaxSpeed)
                    minecartSpeed += minecartSpeedIncrease;
            }
       
            scoreText.text = currentScore.ToString();
            goldenShine.Play();
            firework.SetFloat("ParticleCount", currentScore);
            firework.Play();
            
        }
    }
}
