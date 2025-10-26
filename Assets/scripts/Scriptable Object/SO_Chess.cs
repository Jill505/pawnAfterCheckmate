using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(fileName = "SO_Chess", menuName = "Scriptable Objects/SO_Chess")]
public class SO_Chess : ScriptableObject
{
    public string chessName;
    public Camp myCamp;
    public Sprite photoSticker; //Require = 1:1 picture.

    public gear spawnGear = gear.noGear;
    public BucketType bucketType = BucketType.noType;

    [TextArea(3, 10)]
    public string chessDesc;

    public int Level = 1;
    public int Exp = 0;

    public int move = 1;
    public int maxHp = 1;
    public int hp = 1;

    public Sprite skin;

    public ability[] abilities;

    public bool isGoldenTarget;

    [Header("´Ñ¤lªì©l¼Æ­È")]
    public int horBlockMoveAbility = 1;
    public int verticalBlockMoveAbility = 1;
    public int diagonalBlockMoveAbility = 0;
    public int knightBlockMoveAbility = 0;
    public int AttackStr = 1;


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
