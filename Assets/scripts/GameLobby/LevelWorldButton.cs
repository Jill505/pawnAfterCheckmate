using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;   

public class LevelWorldButton : MonoBehaviour
{
    public GameLobbyManager gameLobbyManager;

    [Header("Level info")]
    public SO_Level levelInfo;

    [Header("Show Contexts")]
    public Image myShowcaseImage;

    private void Awake()
    {
        if(gameLobbyManager == null) gameLobbyManager = GameObject.Find("GameLobbyManager").GetComponent<GameLobbyManager>();
    }

    public void OnMouseDown()
    {
        //Trigger Manager
        if (levelInfo == null)
        {
            Debug.LogError("Ak Error: The Level world button doesn't contain level info.");
            return;
        }

        gameLobbyManager.StartLevelInspect(this);
    }

    public void OpenCanvasUI()
    {

    }
}
