using AKTool;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Mechanics", menuName = "Scriptable Objects/SO_Mechanics")]
public class SO_MechanicsLibData : ScriptableObject
{
    public TextAsset LangData;
    public Sprite SpriteData;

    public string[] strs_landData;

    public int MechanicsUID = -1;

    public void LoadLangData()
    {
        AK_ToolBox.LoadLangData(LangData, ref strs_landData);
    }

    public string GetName()
    {
        LoadLangData();
        if (strs_landData[0] == null)
        {
            return "ERROR";
        }
        return strs_landData[0];
    }

    public string GetMaxKnowledgeStrs()
    {
        LoadLangData();

        int kn = SaveSystem.SF.MechanicsKnowledgeLevelData[MechanicsUID];

        return strs_landData[1];
    }
    public Sprite GetMaxKnowledgeSprite()
    {
        return SpriteData;
    }
}
