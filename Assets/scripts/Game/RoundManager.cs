using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.PackageManager;

public class RoundManager : MonoBehaviour
{
    [Header("System")]
    public GameManager gameManager;
    public CameraManager cameraManager;
    public SoundManager soundManager;

    [Header("Game Information")]
    public bool GameGoing = false;
    public bool isPlayerRound = false;
    public int leftRoundCount = 5;

    public RoundState roundState = RoundState.Deploy;

    [Header("CONST")]
    public readonly Vector2 EMPTY_VECTOR = new Vector2 (-1,-1);

    [Header("Round Variable")]
    public int roundCount =0;
    public bool goldenTargetSpawned = false;

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

    public Image TroopSkinShowcase;
    public Text TroopNameShowcase;
    public Text TroopSurviveShowcase;
    public Text TroopDescShowcase;

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

                if (!isOnSpecialKill)
                {
                    RoundStateShowCase.text = "回合狀態：" + myRoundStateStr;
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
                        SelectObject = gameManager.chessBoardObjectRefArr[gameManager.Troops[0].GetComponent<Troop>().myNowY, gameManager.Troops[0].GetComponent<Troop>().myNowX];
                        selectingVector = new Vector2(gameManager.Troops[0].GetComponent<Troop>().myNowX, gameManager.Troops[0].GetComponent<Troop>().myNowY);

                        Debug.Log("Triggered");
                        SelectObjectTroop = gameManager.MyTroop.GetComponent<Troop>();
                        UpdateOnSelectChessAllowMoveVector();

                        specialClogAutoSelectionClog = true;
                    }
                    RoundStateShowCase.text = "回合狀態：連殺中" + specialTimeCal;
                }
                break;
                
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
                break;

            case RoundState.EnemyRound:
                playerHitCombo = 0;
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

                EnemyEvolve();

                roundCount++;
                gameManager.GameTargetUISet();
                resetUnitSelectState();
                Debug.Log("AA");
                //新敵人加入戰場
                for (int i = 0; i < gameManager.levelData.enemySpawnEachRound; i++)
                {
                    if (gameManager.levelData.spawnChessData.Count <= 0)
                    {
                        Debug.Log("AK ERROR: Round Manager - 該關卡無額外生成敵人資料");
                        return;
                    }
                    int ranSpawnObjSort = Random.Range(0, gameManager.levelData.spawnChessData.Count);

                    SO_Chess SO_C = gameManager.levelData.spawnChessData[ranSpawnObjSort];

                    RandomSpawnEnemy(SO_C, false);
                }
                //RandomSpawnEnemy(gameManager.levelData);

                //勝利狀態判定與生成黃金敵人
                switch (gameManager.levelData.myMissionType)
                {
                    case MissionType.Survive:
                        if (!goldenTargetSpawned && roundCount == gameManager.levelData.SurviveRound) 
                        {
                            //Spawn Golden Enemy;
                            RandomSpawnEnemy(gameManager.levelData.goldenTarget.chessFile, true);
                        }
                        break;
                }

                roundState = RoundState.MyRound;
                if (gameManager.isBlitzOn)
                {
                    gameManager.StartBlitzCoroutine(gameManager.blitzTime);
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
            EnemyAITroop[i].MoveToNext();
            yield return new WaitForSeconds(enemyMoveDur); 
        }
        //Var 2 - 每次移動一個目標

        roundState = RoundState.Finished;
        EnemyAIProcessing = null;
    }

    public void MakePlayerDie()
    {
        gameManager.MyTroop.GetComponent<Troop>().PlayerDieReaction();
        Lose();
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

        Time.timeScale = 0.6f;
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
        if (SelectObject != null)
        {
            resetUnitSelectState(SelectObject);
            SelectObject = null;
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
            gameManager.Troops[i].GetComponent<Troop>().EnemyEvo();
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

    public void UpdateOnSelectChessAllowMoveVector() // 玩家 可移動地塊更新 重要函式！！！
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
        RoundCountShowcase.text = "回合數："+roundCount;
        TroopInformationUISync();
    }
    public void TroopInformationUISync()
    {
        if (onFloatingObject != null)
        {
            if (onFloatingObject.GetComponent<unit>().TroopsOnMe != null)
            {
                Troop ST = onFloatingObject.GetComponent<unit>().TroopsOnMe;
                TroopSkinShowcase.sprite = ST.myChessData.skin;

                TroopNameShowcase.text = ST.myChessData.chessName;
                TroopSurviveShowcase.text = "存活回合：" + ST.surviveRound;
                TroopDescShowcase.text = ST.myChessData.chessDesc;

                EnemyAttackRangeShowcase(ST);
                Debug.Log("AK通知 敵人攻擊範圍 UI Sync");
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
        SelectObject = gameManager.chessBoardObjectRefArr[gameManager.Troops[0].GetComponent<Troop>().myNowY, gameManager.Troops[0].GetComponent<Troop>().myNowX];
        selectingVector = new Vector2(gameManager.Troops[0].GetComponent<Troop>().myNowX, gameManager.Troops[0].GetComponent<Troop>().myNowY);

        Debug.Log("Triggered");
        SelectObjectTroop = gameManager.MyTroop.GetComponent<Troop>();
        UpdateOnSelectChessAllowMoveVector();


        isOnSpecialKill = true;
        float suppT = 0;
        if (hit > 3) suppT = 1f;
        specialCoroutine = StartCoroutine(SpecialRoundTimer(specialTime + suppT));
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

    public void RandomSpawnEnemy(SO_Chess SO_C, bool isSpawnGoldenTarget)
    {
        //getRandomSpawnPos
        int L = gameManager.chessBoardObjectRefArr.Length;
        List<int> sort = new List<int>();
        List<GameObject> refObjs= new List<GameObject>();
        foreach (GameObject tObj in gameManager.chessBoardObjectRefArr)
        {
            refObjs.Add(tObj);
        }
        for(int i=0; i< L; i++) sort.Add(i);
        int ranSpotSort = 0;
        Vector2 tarSpawnVector = EMPTY_VECTOR;
        while (sort.Count > 0)
        {
            ranSpotSort = Random.Range(0, sort.Count);
            if (refObjs[ranSpotSort].gameObject.GetComponent<unit>().TroopsOnMe == null)
            {
                //Target is ran spot
                tarSpawnVector = new Vector2(refObjs[ranSpotSort].gameObject.GetComponent<unit>().myX, refObjs[ranSpotSort].gameObject.GetComponent<unit>().myY);
                break;
            }
            else
            {
                refObjs.RemoveAt(ranSpotSort);
            }
        }
        //Check if there's no empty space
        if (tarSpawnVector == EMPTY_VECTOR)
        {
            Debug.Log("AK ERROR: Round Manager - 無法生成新敵人");
            return;
        }
        //Spawn A chess
        GameBoardInsChess GBIC = new GameBoardInsChess();
        if (isSpawnGoldenTarget)
        {
            //GBIC = sO_Level.goldenTarget;
        }
        else
        {
            GBIC.chessFile = SO_C;
        }
        GBIC.locationX = (int)tarSpawnVector.x;
        GBIC.locationY = (int)tarSpawnVector.y;
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
            troopShowTarget.EnemyLogic();

            for (int i = 0; i < troopShowTarget.OnSelectChessAllowMoveVector.Count; i++)
            {
                Vector2 tarVec2 = troopShowTarget.OnSelectChessAllowMoveVector[i];
                unit tarUnit = gameManager.chessBoardObjectRefArr[(int)tarVec2.y, (int)tarVec2.x].GetComponent<unit>();
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
}
public enum RoundState
{
    Deploy, //部署
    MyRound, //我的回合
    MySpecialRound, //獎勵及時特殊回合
    EnemyRound, //敵人回合
    AnimatePlay, //動畫進行
    Finished, //完成階段
}