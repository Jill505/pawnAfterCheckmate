using System.IO;
using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
    public SO_PlayerConfig mySoPlayerConfig;
    // 請注意 此系統已廢除

    public void ResetData()
    {
        //TODO: Restart the game and do not let the file load next time.
    }
    public void LoadData()
    {
        Debug.Log("Load Data");

        string path = Application.persistentDataPath + AllGameManager.path;
        if (File.Exists(path))
        {
            mySoPlayerConfig.LoadSOData(File.ReadAllText(path));
        }
        else
        {
            // mySoPlayerConfig is default state;
        }
    }
    public void SaveData()
    {
        Debug.Log("saved data");
        //Let all SO_Chess Save;
        string path = Application.persistentDataPath + AllGameManager.path;
        File.WriteAllText(path, mySoPlayerConfig.ReturnSOData());
    }
    private void Awake()
    {
        //LoadData();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //SaveData();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            mySoPlayerConfig.ranStr += "滴8";
        }
    }
}
