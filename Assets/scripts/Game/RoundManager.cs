using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class RoundManager : MonoBehaviour
{
    [Header("System")]
    public GameManager gameManager;
    public CameraManager cameraManager;
    public SoundManager soundManager;
    public VFXManager vFXManager;

    [Header("Game Information")]
    public bool GameGoing = false;
    public bool isPlayerRound = false;
    public int leftRoundCount = 5;

    public RoundState roundState = RoundState.Deploy;

    [Header("CONST")]
    public readonly Vector2 EMPTY_VECTOR = new Vector2(-1, -1);

    [Header("Round Variable")]
    public int roundCount = 0;
    public bool goldenTargetSpawned = false;
    public bool finishRoundClog= false;

    public bool playerReviving = false;

    [Header("Functional Variable")]
    public Vector2 selectingVector = new Vector2(-1, -1);
    public GameObject SelectUnit;
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
    //public Text RoundCountShowcase;
    public TextMeshProUGUI RoundCountShowCase_TMP;

    public bool isInspectingATroop = false;
    public Animator troopInspectPenalAnimator;
    public Image TroopSkinShowcase;
    //public Text TroopNameShowcase;
    public TextMeshProUGUI TroopNameTMP;
    public Text TroopSurviveShowcase;
    //public Text TroopDescShowcase;
    public TextMeshProUGUI TroopDescTMP;

    [Header("選擇系統")]
    public List<Vector2> OnSelectChessAllowMoveVector;
    public List<Vector2> OnEnemyChessAllowAttackVector;

    [Header("敵人AI")]
    public Coroutine EnemyAIProcessing;
    public List<Troop> EnemyAITroop;

    public float enemyMoveDur = 0.275f;

    [Header("特殊回合相關宣告")]
    public float specialTime = 1.6f;
    public Coroutine specialCoroutine;
    public float specialTimeCal = 0f;

    public bool isOnSpecialKill;

    public bool specialClogAutoSelectionClog;
    public int playerHitCombo = 0;

    [Header("Trick系統相關")]
    public bool isCastingPlacementTrick = false;
    public bool isCastingTrick_StrawMan = false;

    [Header("Actions")]
    public Action Action_OnRoundEnd = () => { };

    [Header("Enemy Animation Coroutine")]
    public Coroutine EnemyAnimationCoroutine;
    public bool EnemyAnimationCoroutineEnd;

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
                    //RoundStateShowCase.text = "回合狀態：" + deployStateStr;
                }
                else
                {
                    //Call deploy animation.
                }
                break;

            case RoundState.MyRound:
                if (SelectUnit != null) // Has Selecting Object
                {
                    //SelectObject.GetComponent<SpriteRenderer>().color = Color.red;
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

                if (!isOnSpecialKill)
                {
                    //RoundStateShowCase.text = "回合狀態：" + myRoundStateStr;
                }
                else
                {
                    if (specialClogAutoSelectionClog)
                    {
                        //Waiting;
                    }
                    else
                    {
                        resetUnitSelectState();
                        SelectUnit = gameManager.chessBoardObjectRefArr[gameManager.PlayerTroop.myNowY, gameManager.PlayerTroop.myNowX];
                        selectingVector = new Vector2(gameManager.PlayerTroop.myNowX, gameManager.PlayerTroop.myNowY);

                        Debug.Log("Triggered");
                        SelectObjectTroop = gameManager.PlayerTroop;
                        Player_UpdateOnSelectChessAllowMoveVector();

                        specialClogAutoSelectionClog = true;
                    }
                    //RoundStateShowCase.text = "回合狀態：連殺中" + specialTimeCal;
                }
                break;

            case RoundState.Revive:
                resetUnitSelectState();
                SelectUnit = gameManager.chessBoardObjectRefArr[gameManager.PlayerTroop.myNowY, gameManager.PlayerTroop.myNowX];
                selectingVector = new Vector2(gameManager.PlayerTroop.myNowX, gameManager.PlayerTroop.myNowY);

                Debug.Log("Triggered");
                SelectObjectTroop = gameManager.PlayerTroop;
                Player_UpdateOnSelectChessAllowMoveVector();

                break;

                /*
            case RoundState.MySpecialRound: //注意 Special Round 系統已經被CLOG與旗標完全取代
                //自動選擇玩家物件並觸發地塊選擇
                resetUnitSelectState();
                SelectObject = gameManager.chessBoardObjectRefArr[gameManager.Troops[0].GetComponent<Troop>().myNowY, gameManager.Troops[0].GetComponent<Troop>().myNowX];
                if (SelectObject != null) // Has Selecting Object
                {
                    SelectObject.GetComponent<SpriteRenderer>().color = Color.red;
                    //Sync information on the UI board
                }
                else
                {
                    //reset the game board UI information
                }
                RoundStateShowCase.text = "回合狀態：連殺中" + specialTimeCal;
                break; */

            case RoundState.EnemyRound:
                playerHitCombo = 0;
                //RoundStateShowCase.text = "回合狀態：" + enemyRoundStateStr;
                if (EnemyAIProcessing != null)
                {
                }
                else
                {
                    EnemyAIProcessing = StartCoroutine(EnemyRoundCoroutine());
                }
                break;

            case RoundState.Finished:
                if (finishRoundClog == false)
                {
                    finishRoundClog = true;
                    StartCoroutine(RoundFinishCoroutine());
                }

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
            EnemyAnimationCoroutineEnd = true;

            EnemyAITroop[i].MoveToNext();
            EnemyAITroop[i].Action_OnRoundEnd();
            EnemyAITroop[i].Action_PowerActiveOnce();
            EnemyAITroop[i].CleanFunction_Action_PowerActiveOnce();

            yield return new WaitUntil(()=> EnemyAnimationCoroutineEnd);

            yield return new WaitForSeconds(enemyMoveDur); 
        }
        //Var 2 - 每次移動一個目標
        
        roundState = RoundState.Finished;

        EnemyAIProcessing = null;
    }
    public IEnumerator RoundFinishCoroutine()
    {
        EnemyEvolve();

        roundCount++;
        Action_OnRoundEnd();

        gameManager.GameTargetUISet();
        resetUnitSelectState();
        //Debug.Log("AA");
        //新敵人加入戰場

        for (int i = 0; i < gameManager.levelData.enemySpawnEachRound; i++)
        {
            SpawnEnemyInPool();
            yield return new WaitForSeconds(0.2f);
        }

        //勝利狀態判定與生成黃金敵人
        switch (gameManager.levelData.myMissionType)
        {
            case MissionType.Survive:
                if (!goldenTargetSpawned && roundCount == gameManager.levelData.SurviveRound)
                {
                    //Spawn Golden Enemy;
                    GameBoardInsChess GBIC = new GameBoardInsChess();
                    GBIC.chessFile = gameManager.levelData.goldenTarget.chessFile;

                    SpawnEnemy_RandomSpot(GBIC);
                }
                break;
        }

        if (playerReviving)
        {
            roundState = RoundState.Revive;
        }
        else
        {
            roundState = RoundState.MyRound;
        }

        finishRoundClog = false;

        if (gameManager.isBlitzOn)
        {
            gameManager.StartBlitzCoroutine(gameManager.blitzTime);
        }
    }

    public void MakePlayerDie()
    {
        gameManager.PlayerTroop.PlayerDieReaction();
        //開啟特殊管道
        gameManager.PlayerTroop.leftLife -= 1;
        if (gameManager.PlayerTroop.leftLife > 0)
        {
            //Execute revive;
            //TSA_Player TSAP =  gameManager.PlayerTroop.gameManager.GetComponent<TSA_Player>();
            TSA_Player TSAP = FindFirstObjectByType<TSA_Player>();
            if (TSAP == null)
            {
                Debug.Log("Ed Sheeran - Perfect");
            }
            TSAP.SpawnBlackMist();
            playerReviving = true;
        }
        else
        {
            Lose();
        }
    }

    public void WinLoseJudge()
    {

    }

    public GameObject WinCanvas_DeclareForSwap;
    public void Win()
    {
        //WinCanvas_DeclareForSwap.SetActive(true);
        //cameraManager.ExitGameCamera();
        Debug.Log("玩家勝利");

        gameManager.gameBGM.FadeOut(0.5f);
        soundManager.PlaySFX("Holy_Shotgun_original", true);

        StartCoroutine(WaitExitCall());
    }
    IEnumerator WaitExitCall()
    {
        //Make time flow slow and maybe a close up?

        //Time.timeScale = 0.6f;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;

        cameraManager.ExitGameCamera();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    [Header("SPECIAL DECLARE SWAP")]
    public Animator playerDieCanvasAnimator;
    public void Lose()
    {
        playerDieCanvasAnimator.SetTrigger("PlayerDie");
        Debug.Log("Player Death");

        gameManager.gameBGM.FadeOut(0.5f);
        soundManager.PlaySFX("bell_lose", true);

        StartCoroutine(WaitExitCall());
    }

    public void GameStartFunc()
    {
        roundState = RoundState.Deploy;
    }

    public void resetUnitSelectState() //預設清空選擇中物件
    {
        if (SelectUnit != null)
        {
            resetUnitSelectState(SelectUnit);
            SelectUnit = null;
            SelectObjectTroop = null;
            selectingVector = EMPTY_VECTOR;
            OnSelectChessAllowMoveVector.Clear();
        }
    }
    private void resetUnitSelectState(GameObject obj) //將物件選擇效果狀態清空
    {
        obj.GetComponent<SpriteRenderer>().color = Color.white;     
    }

    public void EnemyEvolve()
    {
        for (int i = 1; i < gameManager.Troops.Count; i++)
        {
            gameManager.Troops[i].GetComponent<Troop>().surviveRound++;
            gameManager.Troops[i].GetComponent<Troop>().ChessEvo();
        }
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

    public void Player_UpdateOnSelectChessAllowMoveVector() // 玩家 可移動地塊更新 重要函式！！！
    {
        SelectObjectTroop.UpdateOnSelectChessAllowMoveVector(OnSelectChessAllowMoveVector, SelectObjectTroop);
        //玩家Reduce

    }
    #endregion

    public void RoundSelectClean()
    {
        OnSelectChessAllowMoveVector.Clear();
        resetUnitSelectState();
    }
    public void MyRoundEnd()
    {
        RoundSelectClean();
        gameManager.StopBlitzCoroutine();
        roundState = RoundState.EnemyRound;
    }

    public void EnemyRoundEnd()
    {
    }

    #region UI 功能
    public void SyncUI()
    {
        RoundCountShowCase_TMP.text = "回合數："+roundCount;
        TroopInformationUISync();
    }
    public void TroopInformationUISync()
    {
        if (onFloatingObject != null)
        {
            if (onFloatingObject.GetComponent<unit>().TroopsOnMe != null)
            {
                isInspectingATroop = true;
                troopInspectPenalAnimator.SetBool("isPenalOn", isInspectingATroop);

                Troop ST = onFloatingObject.GetComponent<unit>().TroopsOnMe;
                TroopSkinShowcase.sprite = ST.myChessData.skin;

                TroopNameTMP.text = ST.myChessData.chessName;
                //TroopSurviveShowcase.text = "存活回合：" + ST.surviveRound;
                TroopDescTMP.text = ST.myChessData.chessDesc;

                EnemyAttackRangeShowcase(ST);
                //Debug.Log("AK通知 敵人攻擊範圍 UI Sync");
            }
            else
            {
                isInspectingATroop = false;
                troopInspectPenalAnimator.SetBool("isPenalOn", isInspectingATroop);
            }
        }
    }
    #endregion

    public void DebugSyncEnemyAITroop()
    {

    }

    public void StartSpecialRound(int hit)
    {
        //Set Timer;
        //roundState = RoundState.MySpecialRound;

        resetUnitSelectState();
        //SelectUnit = gameManager.PlayerTroop.gameObject;
        SelectUnit = gameManager.chessBoardObjectRefArr[gameManager.PlayerTroop.myNowY, gameManager.PlayerTroop.myNowX];
        selectingVector = new Vector2(gameManager.PlayerTroop.myNowX, gameManager.PlayerTroop.myNowY);

        Debug.Log("Triggered");
        SelectObjectTroop = gameManager.PlayerTroop;
        Player_UpdateOnSelectChessAllowMoveVector();


        isOnSpecialKill = true;
        float suppT = 0;
        if (hit > 3) suppT = 1f;
        //specialCoroutine = StartCoroutine(SpecialRoundTimer(specialTime + suppT));
    }

    public IEnumerator SpecialRoundTimer(float time)
    {
        specialTimeCal = time;
        while (specialTimeCal > 0)
        {
            specialTimeCal -= Time.deltaTime;
            yield return null;
        }
        //使特殊回合失效
        SpecialRoundEndFunc();
        yield return null;
    }
    public void SpecialRoundEndFunc()
    {
        isOnSpecialKill = false;
        if (specialCoroutine != null)
        {
            StopCoroutine(specialCoroutine);
        }
        roundState = RoundState.EnemyRound;
    }

    public void SpawnEnemy_RandomSpot(GameBoardInsChess p_GBIC)
    {
        List<Vector2> emptyList = gameManager.GetEmptyUnitList();

        if (emptyList.Count == 0)
        {
            Debug.Log("AK ERROR: Round Manager - 無法生成新敵人");
            return;
        }

        Vector2 spawnPos = emptyList[UnityEngine.Random.Range(0, emptyList.Count)];

        GameBoardInsChess GBIC = new GameBoardInsChess();
        GBIC.chessFile = p_GBIC.chessFile;
        GBIC.locationX = (int)spawnPos.x;
        GBIC.locationY = (int)spawnPos.y;

        gameManager.SpawnLevelTroop(GBIC);
    }

    public void PlayerRoundInterrupt()
    {
        MyRoundEnd();
    }

    public void EnemyAttackRangeShowcase(Troop troopShowTarget)
    {
        if (troopShowTarget == null)
        {
            return;
        }
        if (troopShowTarget.myCamp == Camp.Enemy)
        {
            troopShowTarget.EnemyCalculateAttackRange();

            for (int i = 0; i < troopShowTarget.OnSelectChessAllowMoveVector.Count; i++)
            {
                Vector2 tarVec2 = troopShowTarget.OnSelectChessAllowMoveVector[i];
                unit tarUnit = gameManager.GetUnitAt((int)tarVec2.x, (int)tarVec2.y); 
                tarUnit.isEnemyAttackHighLighting = true;
            }
        }
    }

    public void EnemyAttackRangeShowcaseReduce()
    {
        foreach (GameObject tarUnitObject in gameManager.chessBoardObjectRefArr)
        {
            unit tarUnit = tarUnitObject.GetComponent<unit>();
            tarUnit.isEnemyAttackHighLighting = false;
        }
    }

    public void SpawnEnemyInPool()
    {
        //新敵人加入戰場
        if (gameManager.levelData.spawnChessData.Count <= 0)
        {
            Debug.Log("AK ERROR: Round Manager - 該關卡無額外生成敵人資料");
            return;
        }

        int allRan = 0;

        for (int i = 0; i < gameManager.levelData.spawnChessData.Count; i++)
        {
            allRan += gameManager.levelData.spawnChessProbability[i];
        }

        int randomNum = UnityEngine.Random.Range(0, allRan);
        int ranSpawnObjSort = 0;

        int currentSum = 0;

        for (int j = 0; j < gameManager.levelData.spawnChessData.Count; j++)
        {
            currentSum += gameManager.levelData.spawnChessProbability[j];

            if (randomNum < currentSum)
            {
                ranSpawnObjSort = j;
                break;
            }
        }

        SO_Chess SO_C = gameManager.levelData.spawnChessData[ranSpawnObjSort];
        GameBoardInsChess GBIC = new GameBoardInsChess();
        GBIC.chessFile = SO_C;

        SpawnEnemy_RandomSpot(GBIC);
        
    }
}
public enum RoundState
{
    Deploy, //部署
    MyRound, //我的回合
    MySpecialRound, //獎勵及時特殊回合
    EnemyRound, //敵人回合
    AnimatePlay, //動畫進行
    Finished, //完成階段

    Revive,
}