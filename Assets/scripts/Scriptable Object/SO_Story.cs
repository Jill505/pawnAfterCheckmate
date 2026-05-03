using AKTool;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_Story", menuName = "Scriptable Objects/SO_Story")]
public class SO_Story : ScriptableObject
{
    [TextArea(2,13)]
    public string comment;

    [Header("MutiLang update here")]
    public TextAsset myMutiLangData;

    [Header("DONT UPDATE HERE!!!!!!")]
    public string[] conversationContext = new string[0];

    public void LoadLangData()
    {
        //string[] sStrs = new string[0];
        string[] str = new string[0];
        AK_ToolBox.LoadLangData(myMutiLangData, ref conversationContext);

        //strs = str;
    }

    public UnityEvent[] registerEvents;
}
