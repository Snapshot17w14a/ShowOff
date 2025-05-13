using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public bool IsPickedUp {  get; private set; } = false;

    public void Collect(Transform parent)
    {
        IsPickedUp = true;
        transform.SetParent(parent);

    }

    public void Drop()
    {
        IsPickedUp = false;
        transform.SetParent(null);
    }
}
