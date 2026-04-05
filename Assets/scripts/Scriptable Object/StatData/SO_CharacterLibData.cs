using AKTool;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_CharacterLibData", menuName = "Scriptable Objects/SO_CharacterLibData")]
public class SO_CharacterLibData : ScriptableObject
{
    public TextAsset LangData;
    public Sprite[] SpriteData;

    public string[] strs_landData;

    public int CharacterUID = -1;
    public int[] KnowledgeLevel = new int[0]; //這個數字有多少 會依照最大值顯示出來

    public void LoadLangData()
    {
        AK_ToolBox.LoadLangData(LangData, ref strs_landData);
    }

    public string GetMaxKnowledgeStrs()
    {
        LoadLangData();

        int kLevel = -1;
        int kn = SaveSystem.SF.CharacterKnowledgeLevelData[CharacterUID];
        for (int i = 0; i < KnowledgeLevel.Length; i++)
        {
            if (kn >= KnowledgeLevel[i])
            {
                kLevel = i;
            }
            else
            {
                break;
            }
        }

        kLevel = Mathf.Min(strs_landData.Length, kLevel);

        if (kLevel >= 0)
        {
            return strs_landData[kLevel];
        }
        else
        {
            return null;
        }
    }

    public Sprite GetMaxKnowledgeSprite()
    {
        int kLevel = -1;
        int kn = SaveSystem.SF.CharacterKnowledgeLevelData[CharacterUID];

        for (int i = 0; i < KnowledgeLevel.Length; i++)
        {
            if (kn >= KnowledgeLevel[i])
            {
                kLevel = i;
            }
            else
            {
                break;
            }
        }

        kLevel = Mathf.Min(SpriteData.Length, kLevel);

        if (kLevel >= 0)
        {
            return SpriteData[kLevel];
        }
        else
        {
            return null;
        }
    }
}
