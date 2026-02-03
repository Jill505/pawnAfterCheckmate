using UnityEngine;
using UnityEngine.UI;
using AKTool;

public class MutiLang_GameLobby : MonoBehaviour
{
    public TextAsset gameLobbyMutiLangData;

    public string[] langData; 

    public void LoadLangSetting(string[] landData)
    {
        
    }
    public void LoadTextData()
    {
        string[] str = AK_ToolBox.GetReadCSV(gameLobbyMutiLangData);
        int langIndex = ((int)SaveSystem.SF.SelectingLanguage);
        langData = AK_ToolBox.GetCertainColumn(str, AllGameManager.SystemLanguageNumber, langIndex);
    }

    void Start()
    {
        LoadTextData();
        LoadLangSetting(langData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
