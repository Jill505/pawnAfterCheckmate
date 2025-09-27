using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    [Header("System")]
    public GameManager gameManager;

    [Header("Game Information")]
    public bool GameGoing = false;
    public bool isPlayerRound = false;
    public int leftRoundCount = 5;

    public RoundState roundState = RoundState.Deploy;

    [Header("CONST")]
    public readonly Vector2 EMPTY_VECTOR = new Vector2 (-1,-1);

    [Header("Functional Variable")]
    public Vector2 selectingVector = new Vector2(-1, -1);
    public GameObject SelectObject;

    public Vector2 onFloatingVector = new Vector2(-1, -1);
    public GameObject onFloatingObject;

    [Header("Deploy Variables")]
    public bool isStillDeploying = true;

    [Header("UI")]
    public Text RoundStateShowCase;
    public string deployStateStr = "部屬";
    public string myRoundStateStr = "我的回合";
    public string enemyRoundStateStr = "敵人回合";

    void Start()
    {
        GameStartFunc();
    }

    void Update()
    {
        switch (roundState)
        {
            case RoundState.Deploy:

                //wait deploy end.
                if (isStillDeploying)
                {
                    RoundStateShowCase.text = "回合狀態：" + deployStateStr;
                }
                else 
                {
                    //Call deploy animation.
                }
                break;

            case RoundState.MyRound:
                if (SelectObject != null) // Has Selecting Object
                {
                    SelectObject.GetComponent<SpriteRenderer>().color = Color.red;
                    //Sync information on the UI board
                }
                else
                {
                    //reset the game board UI information
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    resetUnitSelectState();
                }

                    RoundStateShowCase.text = "回合狀態：" + myRoundStateStr;
                
                break;
                
            case RoundState.EnemyRound:
                RoundStateShowCase.text = "回合狀態：" + enemyRoundStateStr;
                break;
        }
    }

    public void winLoseJudge()
    {

    }

    public void win()
    {

    }
    public void lose()
    {

    }

    public void GameStartFunc()
    {
        roundState = RoundState.Deploy;
    }

    public void resetUnitSelectState() //預設清空選擇中物件
    {
        if (SelectObject != null)
        {
            resetUnitSelectState(SelectObject);
            SelectObject = null;
            selectingVector = EMPTY_VECTOR;
        }
    }
    private void resetUnitSelectState(GameObject obj) //將物件選擇效果狀態清空
    {
        obj.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
public enum RoundState
{
    Deploy, //部署
    MyRound, //我的回合
    EnemyRound, //敵人回合
    AnimatePlay, //動畫進行
    Finished //完成階段
}