using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;
using System.Collections;

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
    public Troop SelectObjectTroop;

    public Vector2 onFloatingVector = new Vector2(-1, -1);
    public GameObject onFloatingObject;

    [Header("Deploy Variables")]
    public bool isStillDeploying = true;

    [Header("UI")]
    public Text RoundStateShowCase;
    public string deployStateStr = "部屬";
    public string myRoundStateStr = "我的回合";
    public string enemyRoundStateStr = "敵人回合";

    [Header("選擇系統")]
    public List<Vector2> OnSelectChessAllowMoveVector;

    [Header("敵人AI")]
    public Coroutine EnemyAIProcessing;

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
                if (EnemyAIProcessing != null)
                {
                }
                else
                {
                    EnemyAIProcessing = StartCoroutine(EnemyRoundCoroutine());
                }
                break;
        }
    }
    IEnumerator EnemyRoundCoroutine()
    {
        yield return new WaitForSeconds(1);
        roundState = RoundState.MyRound;
        EnemyAIProcessing = null;
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
            SelectObjectTroop = null;
            selectingVector = EMPTY_VECTOR;
        }
    }
    private void resetUnitSelectState(GameObject obj) //將物件選擇效果狀態清空
    {
        obj.GetComponent<SpriteRenderer>().color = Color.white;     
    }
    #region 選擇系統
    public bool IsMeSelectableUnit(Vector2 myVec)
    {
        foreach (Vector2 tVec in OnSelectChessAllowMoveVector)
        {
            if(myVec == tVec) return true;
        }

        return false;
    }
    public void UpdateOnSelectChessAllowMoveVector() // 可移動地塊更新 重要函式！！！
    {
        OnSelectChessAllowMoveVector.Clear();
        //get current XY
        Vector2 currentXY = new Vector2(SelectObjectTroop.myNowX, SelectObjectTroop.myNowY);
        //Hor select cal
        for (int i = 1; SelectObjectTroop.myChessData.horBlockMoveAbility >= i; i++)
        {
            OnSelectChessAllowMoveVector.Add(new Vector2(currentXY.x + i, currentXY.y));
            OnSelectChessAllowMoveVector.Add(new Vector2(currentXY.x - i, currentXY.y));
            OnSelectChessAllowMoveVector.Add(new Vector2(currentXY.x, currentXY.y + i));
            OnSelectChessAllowMoveVector.Add(new Vector2(currentXY.x, currentXY.y - i));
        }
    }
    #endregion

    public void MyRoundEnd()
    {
        OnSelectChessAllowMoveVector.Clear();
        resetUnitSelectState();

        roundState = RoundState.EnemyRound;
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