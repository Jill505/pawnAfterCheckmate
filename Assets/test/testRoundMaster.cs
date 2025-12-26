using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public Text LInfo; //Show Game Info for example round left, target, game scene name etx.
    public Text RInfo; //Show Troop Info

    [Header("關卡 Scriptable Object")]
    public SO_Level config;

    [Header("一般世界生成係數")]
    public GameObject GameCellUnit;
    public GameObject GameEReference;
    public float spawnInterval = 1f;
    public Vector2 spawnReferencePoint = Vector2.zero;
    public GameObject ChessPrefab;

    [Header("功能係數")]
    public Vector2 selectingVector = new Vector2(-1,-1);
    public GameObject SelectObject;
    
    public Vector2 onFloatingVector = new Vector2(-1, -1);
    public GameObject onFloatingObject;

    public GameObject[,] chessBoardObjectRefArr;


    public void resetUnitSelectState(GameObject obj)
    {
        obj.GetComponent<SpriteRenderer>().color = Color.white;
    }
    public void resetUnitSelectState() //預設清空選擇中物件
    {
        if (SelectObject != null)
        {
            resetUnitSelectState(SelectObject);
        }
    }

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
        LInfo.text += "Level Name: " + config.levelName;

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
                obj.name = "gameGrid"+ "X"+ i + "Y" + j;
                obj.transform.SetParent(GameEReference.transform, false);
                obj.GetComponent<unit>().myX = i;
                obj.GetComponent <unit>().myY = j;
                //obj.GetComponent<unit>().roundMaster = this;
                chessBoardObjectRefArr[j, i] = obj;
            }
        }
    }

    public void SelectUnit(Troop chess)
    {
        RInfo.text = "";
        RInfo.text += "選擇中單位：" + chess.myChessData.chessName;

        RInfo.text += "\n移動力:" + chess.myChessData.move;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectObject != null)
        {
            SelectObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
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
