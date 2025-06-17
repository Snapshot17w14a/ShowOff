using UnityEngine;

public class MinecartThrow : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Pickupable>() != null)
        {

        }
    }
}
