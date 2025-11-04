using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public SaveFile SF;

    public string saveFilePath = "SaveFile" + 0;

    private void Start()
    {
        
    }

    public void SaveSF()
    {
        string fullPath = Application.persistentDataPath + saveFilePath;
        File.WriteAllText(fullPath, JsonUtility.ToJson(SF));
    }

    public void LoadSF()
    {
        string fullPath = Application.persistentDataPath + saveFilePath;

        if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
        {
            Debug.Log("載入失敗：路徑是空的或找不到檔案，嘗試生成一個新的檔案。");
            SaveFile sSF = new SaveFile();
            File.WriteAllText(fullPath, JsonUtility.ToJson(sSF));
        }

        SaveFile SF = JsonUtility.FromJson<SaveFile>(fullPath + saveFilePath);
    }
}

[System.Serializable]
public class SaveFile
{
    public string SaveName;

    public int gameProcess;
    public bool[] storyRead;
}