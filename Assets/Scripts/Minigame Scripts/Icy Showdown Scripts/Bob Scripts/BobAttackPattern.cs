using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackPattern", menuName = "Scriptables/BobAttackPattern")]
public class BobAttackPattern : ScriptableObject
{
    public enum BobStates
    {
        Idle,
        IceBeam,
        TailBurst,
        SpruceBomb,
        HeavyStomp,
        Star,
        BobsRage
    }

    [SerializeField] private List<BobAttackContainer> bobAttackContainers = new();
    public int StateCount => bobAttackContainers.Count;

    private int index = -1;

    public BobAttackContainer NextState() => bobAttackContainers[+index % bobAttackContainers.Count];

    private void OnValidate() => index = -1;
}

[System.Serializable]
public struct BobAttackContainer
{
    public BobAttackPattern.BobStates State;
    public float time;
}
