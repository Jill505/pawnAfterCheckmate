using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    public SO_LibraryEntry[] EnemyLibEntry_Arr; //統計
    public SO_LibraryEntry[] ItemLibEntry_Arr; //物品說明
    public SO_LibraryEntry[] Playback; //回顧劇情
    public SO_LibraryEntry[] CharacterInfo_Arr; //角色資訊

    void Start()
    {
        LoadLibraryFunc(0);
    }

    void Update()
    {
        
    }

    public void SwitchCategory()
    {

    }

    public void LoadLibraryFunc(LibraryCategory LC)
    {
        switch (LC)
        {

        }
    }

}

public enum LibraryCategory
{
    Enemy,
    Items,
    Playback,
    Character
}

public class LibraryEntry
{
    public TextAsset langData;
    public string[] strs_lang;

    public int knowledgeLevel;

    public void loadKnowledgeLevel()
    {
        //載入認知等級
        //0 = 條目未解鎖
    }

    public void LoadLangData()
    {

    }
}