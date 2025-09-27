using UnityEngine;

public class GameBasicConfigEditMono : MonoBehaviour
{
    public AllGameManager AllGameManager;

    [Header("Ref Objects")]
    public GameObject SettingRefCanvas;
    
    public void OpenSettingCanvas()
    {
        SettingRefCanvas.SetActive(true);
    }
    public void CloseSettingCanvas()
    {
        SettingRefCanvas.SetActive(false);
    }
}
