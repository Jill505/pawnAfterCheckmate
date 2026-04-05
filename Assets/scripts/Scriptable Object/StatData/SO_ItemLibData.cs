using AKTool;
using UnityEngine;

public class SO_ItemLibData : MonoBehaviour
{
    public TextAsset LangData;
    public Sprite[] SpriteData;

    public string[] strs_landData;

    public int ItemUID = -1;
    public int[] KnowledgeLevel = new int[0]; //這個數字有多少 會依照最大值顯示出來

    public ItemSort myItemSort = ItemSort.NoSort;
    public TrickType myTrickType = TrickType.noTrick;

    public void LoadLangData()
    {
        AK_ToolBox.LoadLangData(LangData, ref strs_landData);
    }

    public string GetMaxKnowledgeStrs()
    {
        LoadLangData();

        int kLevel = -1;
        int kn = SaveSystem.SF.ItemKnowledgeLevelData[ItemUID];
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
        int kn = SaveSystem.SF.ItemKnowledgeLevelData[ItemUID];

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

public enum ItemSort
{
    NoSort,
    Trick,
    Armor,
}