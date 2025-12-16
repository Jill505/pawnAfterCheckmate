using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("System")]
    public RoundManager roundManager;
    public LevelConstructor levelConstructor;
    public SO_Level levelData;
    public SoundManager soundManager;
    public TrickManager trickManager;

    public SpecialLevelScript SLS;

    [Header("Scene System")]
    public HintManager hintManager;

    [Header("Blitz System")]
    public bool isBlitzOn;
    public float blitzTime = 3f;
    public Image blitzCountDownImage;
    public Text blitzCountDownText;
    public Coroutine blitzCoroutine;

    [Header("CopySoul System")]
    public bool isCopySoulOn;

    [Header("Control Variable")]
    public bool alreadyLoaded = false;

    [Header("UI context")]
    public Text levelName;
    public Text GameTarget;
    public Animator gameTargetAnimator;

    [Header("Calculate Variable")]
    public Vector2 spawnReferencePoint = Vector2.zero;

    [Header("Ref Objects")]
    public GameObject TroopPrefab;

    public GameObject[,] chessBoardObjectRefArr;
    public GameObject GameEReference;
    public GameObject TroopEReference;

    public GameObject MyTroop;
    public List<GameObject> Troops;

    [Header("World Spawn Variable")]
    public float spawnInterval = 1f;

    [Header("Prefabs")]
    public GameObject GameCellUnit;

    [Header("Music")]
    public AK_Audio gameBGM;
    public string bGM_name = "GiveMeYourFastFist";


    // Start is called on   ce before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckLevelConstructorCount();
        LevelCheckAndLoad();

        GameTargetUISet();

        PlayBgm();
    }

    // Update is called once per frame
    public bool bgmOnOffForTest = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            bgmOnOffForTest = !bgmOnOffForTest;
            if (bgmOnOffForTest)
            {
                gameBGM.FadeIn(0.4f);
            }
            else
            {
                gameBGM.FadeOut(0.4f);

            }
        }
    }

    public void LoadGame(SO_Level config)
    {
        levelName.text = "Level Name: " + config.levelName;
    }

    public void GameInitialization(SO_Level config)
    {
        Debug.Log("關卡生成初始化");

        spawnReferencePoint = Vector2.zero;
        chessBoardObjectRefArr = new GameObject[config.gridSizeX, config.gridSizeY];
        //生成
        for (int j = 0; j < config.gridSizeY; j++)//Y
        {
            spawnReferencePoint.y = j * spawnInterval;
            //Switch row
            for (int i = 0; i < config.gridSizeX; i++)//X
            {
                spawnReferencePoint.x = i * spawnInterval;
                GameObject obj = Instantiate(GameCellUnit, spawnReferencePoint, Quaternion.identity);
                obj.name = "gameGrid" + "X" + i + "Y" + j;
                obj.transform.SetParent(GameEReference.transform, false);
                obj.GetComponent<unit>().myX = i;
                obj.GetComponent<unit>().myY = j;
                obj.GetComponent<unit>().gameManager = this;
                obj.GetComponent<unit>().roundManager = roundManager;
                chessBoardObjectRefArr[j, i] = obj;
            }
        }

        Debug.Log("開始覆蓋場景");
        //渲染覆蓋
        for (int i = 0; i < config.levelContext.Count; i++)
        {
            //Debug.Log("Render config...");
            GameObject tarObj = chessBoardObjectRefArr[config.levelContext[i].locationX, config.levelContext[i].locationY];
            unit tarUnit = tarObj.GetComponent<unit>();
            tarUnit.ApplyPerform(config.levelContext[i].performID);
            tarUnit.passable = config.levelContext[i].passable;
            tarUnit.isAbleToDeploy = config.levelContext[i].isAbleToDeploy;
            tarUnit.isDeployed = config.levelContext[i].isDeployed;
        }
    }


    public void LevelCheckAndLoad()
    {
        // Please place this function at Update() rather then Start(), because of the Level Constructor are load at Start(). 
        if (!alreadyLoaded)
        {
            if (levelConstructor == null)
            {
                Debug.LogError("Ak Error: the Fight scene doesn't contain level constructor, please load the scene loader");
                //Debug.Break();
            }
            else
            {
                alreadyLoaded = true;
                //Load Level
                Debug.Log("Level Loaded");
                levelData = levelConstructor.levelInfo;

                LoadGame(levelConstructor.levelInfo);
                GameInitialization(levelConstructor.levelInfo);

                ChessSpawn(levelConstructor.levelInfo);

                if (levelConstructor.SLS != null)
                {
                    SLS = levelConstructor.SLS;
                    SLS.DO_GameInit();
                }

                //考慮加入倒數

                roundManager.roundState = RoundState.MyRound;
                if (isBlitzOn)
                {
                    StartBlitzCoroutine(blitzTime);
                }
            }
        }
    }

    void CheckLevelConstructorCount()
    {
        // 在場景中找到所有 levelConstructor 元件
        LevelConstructor[] constructors = GameObject.FindObjectsByType<LevelConstructor>(FindObjectsSortMode.None);

        // 檢查數量是否超過兩個
        if (constructors.Length > 2)
        {
            Debug.LogError("Ak Error: the level Constructors is larger then 1");
            Debug.Break();
        }
        levelConstructor = constructors[0];
    }

    public void ChessSpawn(SO_Level config)
    {
        Troops.Clear();

        //增加自己
        //set my 
        GameObject myTObj = Instantiate(TroopPrefab);
        Troop myT = myTObj.GetComponent<Troop>();
        myT.myNowX = 4;
        myT.myNowY = 0;
        myT.isPlayer = true;

        myT.myCamp = Camp.Player;

        Troops.Add(myTObj);
        MyTroop = myTObj;

        SpawnLevelTroop(config);
    }
    public void SpawnLevelTroop(SO_Level config)
    {
        for (int i = 0; i < config.chessInsData.Count; i++)
        {
            SpawnLevelTroop(config.chessInsData[i]);
            /*
            GameObject myTObj = Instantiate(TroopPrefab);
            Troop myT = myTObj.GetComponent<Troop>();
            myT.myChessData = config.chessInsData[i].chessFile;
            myT.LoadSOData();

            myT.myNowX = config.chessInsData[i].locationX;
            myT.myNowY = config.chessInsData[i].locationY;

            Troops.Add(myTObj);*/
        }
    }
    public void SpawnLevelTroop(GameBoardInsChess GBIC)
    {
        GameObject myTObj = Instantiate(TroopPrefab);
        Troop myT = myTObj.GetComponent<Troop>();
        myT.myChessData = GBIC.chessFile;
        myT.LoadSOData();

        myT.myNowX = GBIC.locationX;
        myT.myNowY = GBIC.locationY;

        Troops.Add(myTObj);
    }

    public void SwapFunction_BackToLobby()
    {
        SceneManager.LoadScene(1);
    }
    public void GameTargetUISet()
    {
        string TargetStr = "";

        gameTargetAnimator.SetTrigger("DoJump");

        switch (levelData.myMissionType)
        {
            case (MissionType.Survive):
                if (levelData.SurviveRound - roundManager.roundCount > 0)
                {
                    TargetStr = "目標\n存活 " + (levelData.SurviveRound - roundManager.roundCount) + " 回合";
                    GameTarget.color = Color.white;
                }
                else
                {
                    TargetStr = "目標\n殺死癥結！";
                    GameTarget.color = Color.red;
                }
                break;

            case (MissionType.Special):
                //Call SLS
                TargetStr = SLS.LevelTargetString();
                break;
        }

        GameTarget.text = TargetStr;
    }

    public void PlayBgm()
    {
        gameBGM = soundManager.PlayBGM(bGM_name);
        gameBGM.FadeIn(2f);
    }

    public void StartBlitzCoroutine(float allowReactTime)
    {
        blitzCoroutine = StartCoroutine(BlitzCoroutine(allowReactTime));
    }
    public void StopBlitzCoroutine()
    {
        if (blitzCoroutine != null)
        {
            StopCoroutine(blitzCoroutine);
        }
    }
    IEnumerator BlitzCoroutine(float allowReactTime)
    {
        float CT = allowReactTime;
        while (CT > 0)
        {
            CT -= Time.deltaTime;
            SyncBlitzUI(CT / allowReactTime, CT);
            yield return null;
        }
        //Call Complete Round;
        roundManager.PlayerRoundInterrupt();
        yield return null;
    }
    public void SyncBlitzUI(float percentage, float countDown)
    {
        blitzCountDownImage.fillAmount = percentage;
        blitzCountDownText.text = "Count Down\n" + countDown.ToString("F2");
    }

    public void FrameSkipping()
    {
        Debug.Log("Frame Skip ");
        //StartCoroutine(FrameSkippingCoroutine(0.3f, 0.4f));
    }
    public void FrameSkipping(float lateRate, float lateTime)
    {
        Debug.Log("Frame Skip 特定");
        StartCoroutine(FrameSkippingCoroutine(lateRate, lateTime));
    }
    IEnumerator FrameSkippingCoroutine(float lateRate, float lateTime)
    {
        Time.timeScale = lateRate;
        yield return new WaitForSecondsRealtime(lateTime);
        Time.timeScale = 1f;
    }
}
