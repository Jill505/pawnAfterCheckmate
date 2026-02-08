using UnityEngine;
using UnityEngine.UI;
using AKTool;
using TMPro;

public class MutiLang_GameLobby : MonoBehaviour
{
    public TextAsset gameLobbyMutiLangData;

    public string[] langData;

    public TextMeshProUGUI StartGameTMP;
    public TextMeshProUGUI TutorialTMP;
    public TextMeshProUGUI CloseMenuTMP;
    public TextMeshProUGUI BackToLobbyTMP;
    public TextMeshProUGUI DevPanelTMP;

    public void LoadLangSetting(string[] landData)
    {
        StartGameTMP.text = landData[0];
        TutorialTMP.text = landData[1];
        CloseMenuTMP.text = landData[2];
        BackToLobbyTMP.text = landData[3];
        DevPanelTMP.text = landData[4];
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
}
