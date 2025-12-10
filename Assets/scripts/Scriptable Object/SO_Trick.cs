using UnityEngine;

[CreateAssetMenu(fileName = "SO_Trick", menuName = "Scriptable Objects/SO_Trick")]
public class SO_Trick : ScriptableObject
{
    public string trickName = "預設戲法名稱";

    public TrickType myTrickType;

    public Sprite mySprite;

    public float trickRequireEnergy = 100f;
    public int maxTrickAmount = 1;

    public string trickDesc = "預設戲法描述";
}
public enum TrickType
{
    noTrick,
    testTrick,
    StrawMan,
}