using UnityEngine;
using AKTool;
using System.Collections;

[CreateAssetMenu(fileName = "SO_Stats", menuName = "Scriptable Objects/SO_Stats")]
public class SO_StatsLibData : ScriptableObject
{
    public TextAsset LangData;
    public Sprite SpriteData;

    public string[] strs_landData;
    public StatsType statsType;
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

    public string GetDesc()
    {
        string returnText = "";
        switch (statsType)
        {
            case StatsType.Movement:
                returnText = "";
                break;

            case StatsType.Kills:
                returnText = "";
                break;
        }
        return returnText;
    }

    public Sprite GetSprite()
    {
        return SpriteData;
    }

}

public enum StatsType
{
    Movement,
    Kills
}