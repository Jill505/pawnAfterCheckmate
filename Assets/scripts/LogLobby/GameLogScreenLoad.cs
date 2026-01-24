using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameLogScreenLoad : MonoBehaviour
{
    public TextAsset LogScreenTextSheet;

    public string[] LangRef;

    public TextMeshProUGUI StartGameButton;
    public TextMeshProUGUI LeaveGameButton;
    public TextMeshProUGUI SettingGameButton;
    public TextMeshProUGUI ResetFileButton;
    public TextMeshProUGUI LinkButton;
    public TextMeshProUGUI CreditButton;

    void Start()
    {
        LoadLogScreenLan();
        LoadLanLogScreen();
    }

    void Update()
    {
        
    }

    public void LoadLogScreenLan()
    {
        string[] strs = AK_ToolBox.GetReadCSV(LogScreenTextSheet);
        int langIndex = ((int)SaveSystem.SF.SelectingLanguage);
        LangRef = AK_ToolBox.GetCertainColumn(strs, AllGameManager.SystemLanguageNumber, langIndex, false);
    }

    public void LoadLanLogScreen()
    {
        StartGameButton.text = LangRef[1];
        LeaveGameButton.text = LangRef[2];
        SettingGameButton.text = LangRef[3];
        ResetFileButton.text = LangRef[4];
        LinkButton.text = LangRef[5];   
        CreditButton.text = LangRef[6];
    }
}
