
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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

    public void LoadIntoTheGame()
    {
        LoadPlayerSaveFile();

        if (SaveSystem.SF.storyRead[0] == false)
        {
            //¼½©ñ°Êµe
            StorySceneManager.ADR_LoadStorySort = 1;
            StorySceneManager.ADR_IsStoryLoading = true;
            SaveSystem.SF.storyRead[0] = true;
            SaveSystem.SaveSF();
            SceneManager.LoadScene(3);
        }
        else
        {
            SceneManager.LoadScene("GameLobby");
        }
    }
    IEnumerator LoadIntoTheGameCoroutine()
    {
        //wait until the animation and load complete
        LoadPlayerSaveFile();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GameLobby");
    }
    
    
    public void LoadPlayerSaveFile()
    {
        //TODO: load player save file eth.
        Debug.LogWarning("Save/Load Function haven't complete yet.");
    }

    public void LeaveGame()
    {
        Application.Quit();
    }
}
