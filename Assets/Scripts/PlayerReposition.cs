using UnityEngine;

public class PlayerReposition : MonoBehaviour
{
    [SerializeField] private Vector3 reposition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = reposition;
        }
    }
}
