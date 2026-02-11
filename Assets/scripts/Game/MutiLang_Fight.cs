using TMPro;
using UnityEngine;
using AKTool;

public class MutiLang_Fight : MonoBehaviour
{
    public TextAsset Fight_MutiLangData;
    public string[] langDataBuffer;

    public TextMeshProUGUI Esc_Continue_Button_TMP;
    public TextMeshProUGUI Esc_BackToLobby_Button_TMP;
    public TextMeshProUGUI Interface_Target_TMP;

    public void Start()
    {
        LoadLangData();
    }

    public void LoadLangData()
    {
        AK_ToolBox.LoadLangData(Fight_MutiLangData, ref langDataBuffer);

        Esc_Continue_Button_TMP.text = langDataBuffer[0];
        Esc_BackToLobby_Button_TMP.text = langDataBuffer[1];

        Interface_Target_TMP.text = langDataBuffer[2];
    }
}
