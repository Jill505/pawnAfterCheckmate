using UnityEngine;
using AKTool;

[CreateAssetMenu(fileName = "SO_EnemyLibData", menuName = "Scriptable Objects/SO_EnemyLibData")]
public class SO_EnemyLibData : ScriptableObject
{
    public TextAsset LangData;
    public Sprite[] SpriteData;

    public string[] strs_landData;

    public int EnemyUID = -1;
    public int[] KnowledgeKillRequireArr = new int[0]; //這個數字有多少 會依照最大值顯示出來

    public void LoadLangData()
    {
        AK_ToolBox.LoadLangData(LangData, ref strs_landData);
    }

    public string GetMaxKnowledgeStrs()
    {
        LoadLangData();

        int kLevel = -1;
        int kn = SaveSystem.SF.EnemyHistoryKillData[EnemyUID];
        for (int i = 0; i < KnowledgeKillRequireArr.Length; i++)
        {
            if (kn >= KnowledgeKillRequireArr[i])
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
        int kn = SaveSystem.SF.EnemyHistoryKillData[EnemyUID];

        for (int i = 0; i < KnowledgeKillRequireArr.Length; i++)
        {
            if (kn >= KnowledgeKillRequireArr[i])
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
