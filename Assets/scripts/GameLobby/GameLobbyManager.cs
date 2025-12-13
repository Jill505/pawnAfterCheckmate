
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEditor.Rendering.Universal;

public class GameLobbyManager : MonoBehaviour
{
    [Header("Manage Component Refs")]
    public ScrollViewGameLobby scrollViewManger;
    public GameLobbyCameraController cameraController;
    public LevelLoader levelLoader;
    public GameLobbyUIManager gameLobbyUIManager;

    [Header("GameObject Refs")]
    public GameObject LevelInspectCanvas;

    [Header("Level SO")]
    public LobbyGameStage[] myGameStages;
    public int nowStageIndex = 0;
    public int nowLevelIndex = 0;

    [Header("ESC Stuff")]
    public GameObject ECS_Canvas;
    public bool isESCIng;
    
    [Header("UI Stuff")]
    public Button LoadLevelButton;
    public SpriteRenderer backgroundImageSpriteRenderer;

    public Text LevelName;
    public Text LevelDesc;

    public Image playerChoosingTrick;
    public Text playerTrickName;
    public Text playerTrickDesc;
    public SO_Trick trickSOFile;

    public TrickType[] nowShowTrickArray;
    static public int nowSelectingTrickIndex;
    public int nowSelectingTrickIndexInspect;

    [Header("Load Level Button")]
    public Button GoNextLevelButton;
    public Button GoLastLevelButton;

    private void Awake()
    {

    }

    public void Start()
    {
        nowStageIndex = SaveSystem.SF.saveStageIndex;
        nowLevelIndex = SaveSystem.SF.saveLevelIndex;
        nowSelectingTrickIndex = SaveSystem.SF.nowSelectingTrickIndex;

        if (scrollViewManger != null)
        {
            scrollViewManger.AllowScroll = true;
            scrollViewManger.AllowZoom = true;
        }

        //DO level information load;
        DoSwitchLobbyLevel(nowStageIndex, nowLevelIndex);
        SelectTrick(nowShowTrickArray[nowSelectingTrickIndex]);

        AllowNextAndLastButtonInteractable();
    }

    public void Update()
    {
        nowSelectingTrickIndexInspect = nowSelectingTrickIndex;
        ESC_CanvasControl();
    }

    #region UI Related

    public void AllowNextAndLastButtonInteractable()
    {
        int levelLength = myGameStages[nowStageIndex].levels.Length;
        if (nowLevelIndex <= 0)
        {
            GoLastLevelButton.interactable = false;
        }
        else
        {
            GoLastLevelButton.interactable = true;
        }

        if (nowLevelIndex >= levelLength - 1)
        {
            GoNextLevelButton.interactable = false;
        }
        else
        {
            GoNextLevelButton.interactable = true;
        }
    }

    public void DoSwitchLobbyLevel(int StageIndex, int LevelIndex)
    {
        gameLobbyUIManager.OnFocus = false;
        cameraController.allowFloatingCamera = true;
        cameraController.targetOrthographic = cameraController.NormalOrthographic;
        gameLobbyUIManager.Do_LevelNameFadeIn();
        LoadLobbyLevel(myGameStages[StageIndex].levels[LevelIndex]);

        AllowNextAndLastButtonInteractable();
    }

    public void DoSwitchLobbyLevelNext()
    {
        nowLevelIndex += 1;
        if (nowLevelIndex >= myGameStages[nowStageIndex].levels.Length - 1)
        {
            nowLevelIndex = myGameStages[nowStageIndex].levels.Length - 1;
        }

        DoSwitchLobbyLevel(nowStageIndex, nowLevelIndex);
    }

    public void DoSwitchLobbyLevelLast()
    {
        nowLevelIndex -= 1;
        if (nowLevelIndex <= 0)
        {
            nowLevelIndex = 0;
        }

        DoSwitchLobbyLevel(nowStageIndex, nowLevelIndex);
    }



    public void LoadLobbyLevel(SO_LobbyLevel SO_L)
    {
        if (SO_L.mySO_Level != null)
        {
            levelLoader.loadLevel = SO_L.mySO_Level;
            LoadLevelButton.interactable = true;    
        }
        else
        {
            //DO Debug calculation.
            LoadLevelButton.interactable = false;
        }

        backgroundImageSpriteRenderer.sprite = SO_L.backgroundImage;
        LevelName.text = SO_L.mySO_Level.levelName;
        LevelDesc.text = SO_L.mySO_Level.levelDesc;
    }

    public void LoadNormalLobbyContext()
    {
        //Load Player Skill Tool Box
        ShowTrickContext();
    }

    #endregion

    #region TrickSystem
    public void DoTrickSwitchIndexAdd()
    {
        nowSelectingTrickIndex += 1;
        if (nowSelectingTrickIndex >= nowShowTrickArray.Length)
        {
            //make it zero
            nowSelectingTrickIndex = 0;
        }
        SwitchSkillInspect(nowSelectingTrickIndex);
    }
    public void DoTrickSwitchIndexMinus()
    {
        nowSelectingTrickIndex -= 1;   
        if (nowSelectingTrickIndex < 0)
        {
            //make it recursive
            nowSelectingTrickIndex = nowShowTrickArray.Length - 1;
        }
        SwitchSkillInspect(nowSelectingTrickIndex);
    }
    public void SwitchSkillInspect(int index)
    {
        SaveSystem.SF.nowSelectingTrickIndex = index;
        SelectTrick(nowShowTrickArray[index]);
    }

    public void SelectTrick(TrickType switchTargetTrickType)
    {
        SaveSystem.SF.holdingTrickType = switchTargetTrickType;
        SaveSystem.SaveSF();

        ShowTrickContext();
    }

    public void ShowTrickContext()
    {
        TrickType myTrickType = SaveSystem.SF.holdingTrickType;

        string trickPath = "TrickSO/";
        switch (myTrickType)
        {
            case TrickType.noTrick:
                trickSOFile = Resources.Load<SO_Trick>(trickPath + "NoTrick_SO");
                nowSelectingTrickIndex = 0;
                break;
            case TrickType.testTrick:
                trickSOFile = Resources.Load<SO_Trick>(trickPath + "testTrick_SO");
                nowSelectingTrickIndex = 1;
                break;
            case TrickType.StrawMan:
                trickSOFile = Resources.Load<SO_Trick>(trickPath + "StrawMan_SO");
                nowSelectingTrickIndex = 2;
                break;
        }


        //Apply
        if (trickSOFile != null)
        {
            playerTrickName.text = trickSOFile.trickName;
            playerChoosingTrick.sprite = trickSOFile.mySprite;
            playerTrickDesc.text = trickSOFile.trickDesc;
        }
        else
        {
            Debug.LogError("戲法SO資料不存在");
        }
    }
    
    #endregion

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

    public void ESC_CanvasControl()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isESCIng == false)
            {
                ECS_Canvas.SetActive(true);
                isESCIng = true;
            }
            else
            {
                CloseESCTab();
            }
        }
    }
    public void CloseESCTab()
    {
        ECS_Canvas.SetActive(false);
        isESCIng = false;
    }
    public void ESC_backToLobby()
    {
        SceneManager.LoadScene(0);
    }
}
