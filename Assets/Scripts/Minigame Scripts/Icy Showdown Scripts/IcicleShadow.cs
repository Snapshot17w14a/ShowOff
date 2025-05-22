using UnityEngine;

public class IcicleShadow : MonoBehaviour
{
    [SerializeField] private float maxScale;

    public void UpdateTime(float t)
    {
        float scale = Mathf.Lerp(maxScale, 0, t);
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
