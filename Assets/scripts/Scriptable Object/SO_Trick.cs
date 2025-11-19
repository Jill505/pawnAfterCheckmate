using UnityEngine;

[CreateAssetMenu(fileName = "SO_Trick", menuName = "Scriptable Objects/SO_Trick")]
public class SO_Trick : ScriptableObject
{
    public string trickName = "預設戲法名稱";

    public float trickRequireEnergy = 100f;
    public int maxTrickAmount = 1;
}
