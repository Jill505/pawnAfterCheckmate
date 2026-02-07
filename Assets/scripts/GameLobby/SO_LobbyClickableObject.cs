using UnityEngine;

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
}
