
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("System")]
    public RoundManager roundManager;
    public LevelConstructor levelConstructor;
    public SO_Level levelData;
    public SoundManager soundManager;

    [Header("Scene System")]
    public HintManager hintManager;


    [Header("Control Variable")]
    public bool alreadyLoaded = false;

    [Header("UI context")]
    public Text levelName;
    public Text GameTarget;

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

    // Start is called on   ce before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckLevelConstructorCount();
        LevelCheckAndLoad();

        GameTargetUISet();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadGame(SO_Level config)
    {
        levelName.text = "Level Name: " + config.levelName;
    }

    public void GameInitialization(SO_Level config)
    {
        Debug.Log("INN");

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

        Debug.Log("Start Render");
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

                roundManager.roundState = RoundState.MyRound;
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
        
        switch (levelData.myMissionType) {
            case (MissionType.Survive):
                TargetStr = "目標\n存活" + levelData.SurviveRound +  "回合";
                break;
        }

        GameTarget.text = TargetStr;
    }
}
