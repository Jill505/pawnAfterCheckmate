using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(fileName = "SO_Chess", menuName = "Scriptable Objects/SO_Chess")]
public class SO_Chess : ScriptableObject
{
    [Header("MutiLang")]
    public TextAsset myMutiLangData;

    [Header("Chess Data")]
    public string chessName;
    public Camp myCamp;
    public Sprite photoSticker; //Require = 1:1 picture.

    public gear spawnGear = gear.noGear;

    [TextArea(3, 10)]
    public string chessDesc;

    public int Level = 1;
    public int Exp = 0;

    public int move = 1;
    public int maxHp = 1;
    public int hp = 1;

    public Sprite skin;

    public ability[] abilities;
    public string[] abilitiesParameter;

    public bool isGoldenTarget;

    [Header("棋子初始數值")]
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
#if UNITY_EDITOR
    public void OnValidate()
    {
        int targetLength = abilities != null ? abilities.Length : 0;

        if (abilitiesParameter == null || abilitiesParameter.Length != targetLength)
        {
            string[] newArray = new string[targetLength];

            // 保留原有資料
            if (abilitiesParameter != null)
            {
                int copyLength = Mathf.Min(abilitiesParameter.Length, targetLength);
                for (int i = 0; i < copyLength; i++)
                {
                    newArray[i] = abilitiesParameter[i];
                }
            }

            abilitiesParameter = newArray;
        }
    }
#endif
}
