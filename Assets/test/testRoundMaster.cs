using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class testRoundMaster : MonoBehaviour
{
    [Header("本場遊戲資訊")]
    public bool GameGoing = false;
    public bool isPlayerRound = false;
    public int leftRoundCount = 5;

    [Header("UI")]
    public GameObject testGameStartButton;
    public GameObject gameCanvas;
    public GameObject GameEnviroment;
    public Text LInfo;
    public Text RInfo;

    [Header("關卡 Scriptable Object")]
    public SO_Level config;

    [Header("一般世界生成係數")]
    public GameObject GameCellUnit;
    public GameObject GameEReference;
    public float spawnInterval = 1f;
    public Vector2 spawnReferencePoint = Vector2.zero;
    
    [Header("功能係數")]
    public Vector2 selectingVector = new Vector2(-1,-1);
    public GameObject SelectObject;
    public GameObject[,] chessBoardObjectRefArr;



    public void GameStart()
    {
        GameInitialization(config);

        testGameStartButton.SetActive(false);

        gameCanvas.SetActive(true);
        GameEnviroment.SetActive(true);
    }

    public void playerEndDect()
    {
        StartCoroutine(GameAnimationPlay());
        //WaitForIt
    }

    public IEnumerator GameAnimationPlay()
    {
        yield return null;
    }

    public void GameInitialization(SO_Level config)
    {
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
                GameObject obj =  Instantiate(GameCellUnit, spawnReferencePoint, Quaternion.identity);
                obj.transform.SetParent(GameEReference.transform, false);
                obj.GetComponent<unit>().myX = i;
                obj.GetComponent <unit>().myY = j;
                obj.GetComponent<unit>().roundMaster = this;
                chessBoardObjectRefArr[j, i] = obj;
            }
        }
    }

    public void SelectUnit(unit selectUnit)
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class GameConfig
{
    public int allowRoundCount = 5;
    public int mapSizeX = 5;
    public int mapSizeY = 5;

    public string gameTarget = "default";

    public GameConfig(int allowRoundCount, string gameTarget)
    {
        this.allowRoundCount = allowRoundCount;
        this.gameTarget = gameTarget;
    }
}
