using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    static public SaveFile SF;

    [Header("存檔可視化")]
    [SerializeField]bool SFSync;
    public SaveFile SFShowCase;

    static public string saveFilePath = "SaveFile" + 0;

    private void Start()
    {
        LoadSF();
    }

    private void Update()
    {
        if (SFSync)
        {
            SFShowCase = SF;
        }
    }

    static public void SaveSF()
    {
        string fullPath = Application.persistentDataPath + saveFilePath;
        File.WriteAllText(fullPath, JsonUtility.ToJson(SF));
    }

    static public void LoadSF()
    {
        string fullPath = Application.persistentDataPath + saveFilePath;

        if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
        {
            Debug.Log("載入失敗：路徑是空的或找不到檔案，嘗試生成一個新的檔案。");
            ResetSF();
        }

        SaveFile sSF = JsonUtility.FromJson<SaveFile>(File.ReadAllText(fullPath));
        SF = sSF;
    }

    static public void ResetSF()
    {
        string fullPath = Application.persistentDataPath + saveFilePath;

        SaveFile sSF = new SaveFile();
        File.WriteAllText(fullPath, JsonUtility.ToJson(sSF));

        SF = sSF;
    }
}

[System.Serializable]
public class SaveFile
{
    public string SaveName;

    public int gameProcess;
    public bool[] storyRead = new bool[100];

    //Stats
}