using UnityEngine;

[CreateAssetMenu(fileName = "SO_Chess", menuName = "Scriptable Objects/SO_Chess")]
public class SO_Chess : ScriptableObject
{
    public string chessName;
    public Sprite photoSticker; //Require = 1:1 picture.

    [TextArea(3, 10)]
    public string chessDesc;

    public int Level = 1;
    public int Exp = 0;

    public int move = 1;
    public int maxHp = 1;
    public int hp = 1;


    public Sprite skin;

    public ability[] abilities;


    public string ReturnSOData()
    {
        string jsonStr = JsonUtility.ToJson(this);
        return jsonStr;
    }
    public void LoadSOData(string jsonStr)
    {
        JsonUtility.FromJsonOverwrite(jsonStr, this);
    }
}
