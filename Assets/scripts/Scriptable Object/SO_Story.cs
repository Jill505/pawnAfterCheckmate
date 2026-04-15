using AKTool;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Story", menuName = "Scriptable Objects/SO_Story")]
public class SO_Story : ScriptableObject
{
    [Header("MutiLang")]
    public TextAsset myMutiLangData;

    public string[] strs = new string[0];

    public void LoadLangData()
    {
        //string[] sStrs = new string[0];
        AK_ToolBox.LoadLangData(myMutiLangData, ref strs);

        //strs = sStrs;
    }
}
