using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using AKTool;
using TMPro;

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
    public TextMeshProUGUI levelName_TMP;
    public TextMeshProUGUI GameTarget_TMP;
    public Animator gameTargetAnimator;

    [Header("Calculate Variable")]
    public Vector2 spawnReferencePoint = Vector2.zero;

    [Header("Ref Objects")]
    public GameObject TroopPrefab;
    public GameObject StructurePrefab;

    /// <summary>
    /// 由於XY翻轉問題，直接存取chessBoardObjectRefArr已淘汰，請改用GetChessBoardObjectRefArr
    /// </summary>
    public GameObject[,] chessBoardObjectRefArr;

    //先看看直接修生成可不可以解決，不行就用新的
    /*
    public GameObject[,] GetChessBoardObjectRefArr()
    {
        //解決翻轉問題的補丁
        int width = chessBoardObjectRefArr.GetLength(0);
        int height = chessBoardObjectRefArr.GetLength(1);

        GameObject[,] flipped = new GameObject[height, width];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                flipped[y, x] = chessBoardObjectRefArr[x, y];
            }
        }

        return flipped;
    }*/

    public GameObject GameEReference;
    public GameObject TroopEReference;

    public GameObject PlayerTroopGameObject;
    public Troop PlayerTroop;
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
        string[] langData = new string[] { };
        AK_ToolBox.LoadLangData(config.myMutiLangData, ref langData);

        //levelName.text = "Level Name: " + langData[0];
        levelName_TMP.text = langData[0];
        
    }

    public void GameInitialization(SO_Level config)
    {
        Debug.Log("關卡生成初始化");
        //載入關卡配置 - 音樂
        bGM_name = config.LevelBgmName;

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
        if (SLS == null)
        {
            Debug.Log("CALL SLS NULL");
        }
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

                if (levelConstructor == null)
                {
                    Debug.Log("Level Constructor is null");
                }

                if (levelConstructor.levelInfo == null)
                {
                    Debug.Log("Level Info is null");
                }

                if (levelConstructor.levelInfo.myMutiLangData == null)
                {
                    Debug.Log("Level mutiLang data is null");
                }

                LoadGame(levelConstructor.levelInfo);
                GameInitialization(levelConstructor.levelInfo);

                ChessSpawn(levelConstructor.levelInfo);
                Init_SpawnStructure(levelConstructor.levelInfo);

                if (levelConstructor.SLS != null)
                {
                    Debug.Log("EQU");
                    SLS = levelConstructor.SLS;
                    SLS.DO_GameInit();
                }
                else
                {

                    Debug.Log("N - EQU");
                }

                //考慮加入倒數

                roundManager.roundState = RoundState.MyRound;
                if (isBlitzOn)
                {
                    StartBlitzCoroutine(blitzTime);
                }
            }
        }

        if (SLS == null)
        {
            Debug.Log("CALL SLS NULL after finish");
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
        PlayerTroopGameObject = myTObj;
        PlayerTroop = myTObj.GetComponent<Troop>();

        Init_SpawnLevelTroop(config);
    }
    public void Init_SpawnLevelTroop(SO_Level config)
    {
        for (int i = 0; i < config.chessInsData.Count; i++)
        {
            SpawnLevelTroop(config.chessInsData[i]);
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

    public void Init_SpawnStructure(SO_Level config)
    {
        for (int i = 0; i < config.structureInsData.Count; i++)
        {
            SpawnStructure(config.structureInsData[i]);
        }
    }
    public void SpawnStructure(GameBoardInsStructure GBIS)
    {
        GameObject mySOBJ = Instantiate(StructurePrefab);
        Structure ST = mySOBJ.GetComponent<Structure>();

        ST.mySO_S = GBIS.structureFile;

        ST.myUnit = GetUnitAt(GBIS.locationX, GBIS.locationY);

        ST.LoadSO_Structure();

        ST.SyncMyPositionToUnit();
    }



    public void SwapFunction_BackToLobby()
    {
        SceneManager.LoadScene(1);
    }
    public void GameTargetUISet()
    {
        string TargetStr = "";

        //gameTargetAnimator.SetTrigger("DoJump");

        switch (levelData.myMissionType)
        {
            case (MissionType.Survive):
                if (levelData.SurviveRound - roundManager.roundCount > 0)
                {
                    TargetStr = "目標\n存活 " + (levelData.SurviveRound - roundManager.roundCount) + " 回合";
                    GameTarget_TMP.color = Color.white;
                }
                else
                {
                    TargetStr = "目標\n殺死癥結！";
                    GameTarget_TMP.color = Color.red;
                }
                break;

            case (MissionType.Special):
                //Call SLS
                if (SLS == null)
                {
                    Debug.Log("SLS is null");
                }
                else 
                {
                    Debug.Log("SLS Exist");
                }
                TargetStr = SLS.LevelTargetString();

                break;
        }

        GameTarget_TMP.text = TargetStr;
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
        //Time.timeScale = lateRate;
        yield return new WaitForSecondsRealtime(lateTime);
        Time.timeScale = 1f;
    }

    public unit GetUnitAt(int x, int y)
    {
        unit swapUnit = chessBoardObjectRefArr[y,x].GetComponent<unit>();
        return swapUnit;
    }
    public List<Vector2> GetEmptyUnitList()
    {
        List<Vector2> EmptyUnitVectorList = new List<Vector2>();
        
        foreach (GameObject obj in chessBoardObjectRefArr)
        {
            unit swapUnit = obj.GetComponent<unit>();
            if (swapUnit.TroopsOnMe == null)
            {
                // it's empty;
                EmptyUnitVectorList.Add(new Vector2(swapUnit.myX, swapUnit.myY));
            }
        }
        
        return EmptyUnitVectorList;
    }
    public List<Vector2> GetHasTroopUnitList()
    {

        List<Vector2> EmptyUnitVectorList = new List<Vector2>();

        foreach (GameObject obj in chessBoardObjectRefArr)
        {
            unit swapUnit = obj.GetComponent<unit>();
            if (swapUnit.TroopsOnMe != null)
            {
                // it's empty;
                EmptyUnitVectorList.Add(new Vector2(swapUnit.myX, swapUnit.myY));
            }
        }

        return EmptyUnitVectorList;
    }

    public bool isVectorLegal(Vector2 vec)
    {
        if (vec.x < 0 || vec.y < 0 || vec.x >= levelData.gridSizeX || vec.y >= levelData.gridSizeY)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
