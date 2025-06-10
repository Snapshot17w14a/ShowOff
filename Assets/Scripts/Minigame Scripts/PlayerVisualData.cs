using UnityEngine;

[CreateAssetMenu(fileName = "Player Visual Data", menuName = "Scriptables/Player Visual Data")]
public class PlayerVisualData : ScriptableObject
{
    public Material material;
    public int id;
    public Color color;
}
