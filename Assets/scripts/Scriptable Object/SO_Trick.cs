using UnityEngine;
using AKTool;

[CreateAssetMenu(fileName = "SO_Trick", menuName = "Scriptable Objects/SO_Trick")]
public class SO_Trick : ScriptableObject
{
    [Header("LangData")]
    public TextAsset myMutiLangData;

    [Header("Trick Data")]
    public string trickName = "預設戲法名稱";

    public TrickType myTrickType;

    public Sprite mySprite;

    public float trickRequireEnergy = 100f;
    public int maxTrickAmount = 1;

    [TextArea]
    public string trickDesc = "預設戲法描述";

    public void LoadLangData()
    {
        string[] strs = new string[0];
        AK_ToolBox.LoadLangData(myMutiLangData, ref strs);
            
        trickName = strs[0];
        trickDesc = strs[1];
    }
}
public enum TrickType
{
    noTrick,
    testTrick,
    StrawMan,
    SelfEnergyHigh
}