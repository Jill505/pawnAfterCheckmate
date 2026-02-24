using UnityEngine;

[CreateAssetMenu(fileName = "SO_Structure", menuName = "Scriptable Objects/SO_Structure")]
public class SO_Structure : ScriptableObject
{
    public StructureAbility[] myAbilities;

    public Sprite myStructureSprite;

    public bool isAllowStanding = false;
    public bool isAttackableTarget = false;
    public bool isAllowBeingDestroyByEnergyHighState;
}
