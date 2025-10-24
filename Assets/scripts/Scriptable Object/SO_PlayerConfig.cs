using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerConfig", menuName = "Scriptable Objects/SO_PlayerConfig")]
public class SO_PlayerConfig : ScriptableObject
{
    [Header("Chess")]
    public SO_Chess[] allChess;
    public int[] squads = new int[3];

    [Header("Game Process")]
    public string ranStr = "我還沒想好這個地方要怎麼做所以先放一個String在這裡";
    public string ReturnSOData()
    {
        string jsonStr = JsonUtility.ToJson(this);
        SaveAllChess();
        return jsonStr;
    }
    public void SaveAllChess()
    {
        for (int i = 0; i < allChess.Length; i++)
        {
            string str = allChess[i].ReturnSOData();
            string path = Application.persistentDataPath + AllGameManager.path + "_myChess_" +i;
            File.WriteAllText(path, str);
        }
    }
    public void LoadSOData(string jsonStr)
    {
        JsonUtility.FromJsonOverwrite(jsonStr, this);
        for (int i = 0; i < allChess.Length; i++)
        {
            string p = Application.persistentDataPath + AllGameManager.path + "_myChess_" + i;
            if (File.Exists(p))
            {
                //allChess[i].LoadSOData(File.ReadAllText(p));
            }
            else
            {
                //Chess is default data
            }
        }
    }
}
