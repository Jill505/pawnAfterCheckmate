using UnityEngine;
using AKTool;

[CreateAssetMenu(fileName = "SO_LobbyClickableObject", menuName = "Scriptable Objects/SO_LobbyClickableObject")]
public class SO_LobbyClickableObject : ScriptableObject
{
    [Header("MutiLang")]
    public TextAsset myMutiLangData;

    [Header("ClickableObject Narratives Data")]
    public string ObjectName;

    public Sprite myObjectSprite;

    public float myObjectX;
    public float myObjectY;

    public string[] Narratives;

    public Sprite[] Sprites;

    public void LoadLangData()
    {
        string[] strs = new string[0];
        AK_ToolBox.LoadLangData(myMutiLangData, ref strs);

        Narratives = strs;
    }
}
