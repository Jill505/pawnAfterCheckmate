using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;
using System.Collections;
using UnityEditor;

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

    [Header("Round Variable")]
    public int roundCount =0;

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
    public Text RoundCountShowcase;

    [Header("選擇系統")]
    public List<Vector2> OnSelectChessAllowMoveVector;
    public List<Vector2> OnEnemyChessAllowAttackVector;

    [Header("敵人AI")]
    public Coroutine EnemyAIProcessing;
    public List<Troop> EnemyAITroop;

    void Start()
    {
        //INN
        roundCount = 0;

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

            case RoundState.Finished:
                roundCount++;
                roundState = RoundState.MyRound;
                break;
        }

        SyncUI();
        DebugSyncEnemyAITroop();
    }
    IEnumerator EnemyRoundCoroutine()
    {
        yield return new WaitForSeconds(1);

        //Var 1 - 逐個移動 每回合開始時告訴玩家行動預告 (如.陷陣之志)
        /*foreach (Troop enemy in EnemyAITroop)
        {
            enemy.MoveToNext();
            yield return new WaitForSeconds(0.3f);
        }*/

        for (int i = 0; i < EnemyAITroop.Count; i++)
        {
            EnemyAITroop[i].MoveToNext();
        }
        //Var 2 - 每次移動一個目標

        roundState = RoundState.Finished;
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
    public bool IsMeSelectableUnit(Vector2 myVec) //玩家可以移動到的地點
    {
        foreach (Vector2 tVec in OnSelectChessAllowMoveVector)
        {
            if(myVec == tVec) return true;
        }

        return false;
    }

    public bool IsMeOnAttackableUnit(Vector2 myVec) //這個地點會不會被攻擊
    {
        foreach (Vector2 tVec in OnEnemyChessAllowAttackVector)
        {
            if (myVec == tVec) return true;
        }

        return false;
    }

    public void UpdateOnSelectChessAllowMoveVector() // 玩家 可移動地塊更新 重要函式！！！
    {
        SelectObjectTroop.UpdateOnSelectChessAllowMoveVector(OnSelectChessAllowMoveVector, SelectObjectTroop);
        #region 隨便啦 註解掉的東西摺疊起來
        /*
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

        switch (SelectObjectTroop.holdingGear)
        {
            case gear.car:
                for (int i = 1; 8 >= i; i++)
                {
                    OnSelectChessAllowMoveVector.Add(new Vector2(currentXY.x + i, currentXY.y));
                    OnSelectChessAllowMoveVector.Add(new Vector2(currentXY.x - i, currentXY.y));
                    OnSelectChessAllowMoveVector.Add(new Vector2(currentXY.x, currentXY.y + i));
                    OnSelectChessAllowMoveVector.Add(new Vector2(currentXY.x, currentXY.y - i));
                }

                SelectObjectTroop.holdingGear = gear.noGear;
                break;
        }*/
        #endregion
    }
    #endregion

    public void MyRoundEnd()
    {
        OnSelectChessAllowMoveVector.Clear();
        resetUnitSelectState();

        roundState = RoundState.EnemyRound;
    }

    public void EnemyRoundEnd()
    {
    }

    #region UI Functions
    public void SyncUI()
    {
        RoundCountShowcase.text = "回合數："+roundCount;
    }
    #endregion

    public void DebugSyncEnemyAITroop()
    {

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