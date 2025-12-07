
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class GameLobbyManager : MonoBehaviour
{
    [Header("Manage Component Refs")]
    public ScrollViewGameLobby scrollViewManger;
    public GameLobbyCameraController cameraController;
    public LevelLoader levelLoader;

    [Header("GameObject Refs")]
    public GameObject LevelInspectCanvas;


    public void Start()
    {
        if (scrollViewManger != null)
        {
            scrollViewManger.AllowScroll = true;
            scrollViewManger.AllowZoom = true;
        }
    }
    public void StartLevelInspect(LevelWorldButton levelButt)
    {
        //make inspect UI re-locate to the levelCon's pos
        LevelInspectCanvas.SetActive(true);
        levelLoader.ShowLevelContext(levelButt.levelInfo);

        //switch Camera Pos
        cameraController.LerpMoveCamera(levelButt.gameObject.transform.position);
    }
    public void QuitLevelInspect()
    {
        LevelInspectCanvas.SetActive(false);
        scrollViewManger.AllowScroll = true;
        scrollViewManger.AllowZoom = true;
        cameraController.OnAutoCamera = false;
    }

}
