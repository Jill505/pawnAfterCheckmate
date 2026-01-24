using UnityEngine;

public class unit : MonoBehaviour
{
    public GameManager gameManager;
    public RoundManager roundManager;
    public SoundManager soundManager;
    public TrickManager trickManager;
    public CameraManager cameraManager;
    public VFXManager vFXManager;

    public UnitOutfit myUnitOutfit;

    public int myX;
    public int myY;

    public bool selecting = false;
    public bool passable = true; //可通過地塊

    public bool isAbleBeSelect = false;//可以被選擇中

    public bool isDeployed = false; //已被占用部署地塊
    public bool isAbleToDeploy = false; //是起始允許部署地塊

    public bool isPlayerAllowMoveSpace; //是玩家可以站在地地方

    public Troop TroopsOnMe;

    [Header("Outfit Control stuff")]
    public bool isEnemyAttackHighLighting = false;
    public bool isSkillPlacementHighLighting = false;

    [Header("Placement and trick variables")]
    public bool isPlaceableTarget = false;

    private void Awake()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        trickManager = FindAnyObjectByType<TrickManager>();
        cameraManager = FindAnyObjectByType<CameraManager>();
        vFXManager = FindAnyObjectByType<VFXManager>();
    }


    bool _colorClog;
    void Update()
    {
        CaseSwitcher();
        UpdateLogic();
        syncTroopOnMe();
    }


    public void ApplyPerform(string ID)
    {
        //make sr sprite eul to the current ID
        if (ID == "") //Default Perform
        {
            myUnitOutfit.mySr.sprite = Resources.Load<Sprite>("TerrainSprite/TS_Default");
            myUnitOutfit.myOriginalSprite = Resources.Load<Sprite>("TerrainSprite/TS_Default_highLighted");
        }
        else
        {
            myUnitOutfit.mySr.sprite = Resources.Load<Sprite>("TerrainSprite/" + ID);
            myUnitOutfit.myOriginalSprite = Resources.Load<Sprite>("TerrainSprite/" + ID);
            myUnitOutfit.myHighLightSprite = Resources.Load<Sprite>("TerrainSprite/" + ID + "_HL");

            myUnitOutfit.MyEnemyHighLightSR.sprite = Resources.Load<Sprite>("TerrainSprite/" + ID + "_ENHL");
            myUnitOutfit.MySkillHighLightSR.sprite = myUnitOutfit.myHighLightSprite;
        }
    }

    private void OnMouseEnter()
    {
        //soundManager.PlaySFX("button_float");

        if (roundManager.roundState == RoundState.MyRound)
        {
            int ran = Random.Range(0, 5);
            switch (ran)
            {
                case 0: soundManager.PlaySFX("Wooden_Floor_Walking_Sound_1"); break;
                case 1: soundManager.PlaySFX("Wooden_Floor_Walking_Sound_2"); break;
                case 2: soundManager.PlaySFX("Wooden_Floor_Walking_Sound_5"); break;
                case 3: soundManager.PlaySFX("Wooden_Floor_Walking_Sound_4"); break;
                    //case 4: soundManager.PlaySFX("Wooden_Floor_Walking_Sound_5"); break;
            }

            roundManager.onFloatingObject = roundManager.gameManager.chessBoardObjectRefArr[myY, myX];
            roundManager.onFloatingVector = new Vector2(myX, myY);
            if (selecting == true)
            {
                myUnitOutfit.mySr.color = new Color(1, 1, 1, 1f);
                myUnitOutfit.mySr.sprite = myUnitOutfit.myHighLightSprite;
            }
            else
            {
                //Debug.Log("OnMouseEnter");
                myUnitOutfit.mySr.color = new Color(1, 1, 1, 0.2f);
                myUnitOutfit.mySr.sprite = myUnitOutfit.myOriginalSprite;
            }
        }
    }
    private void OnMouseExit()
    {
        if (selecting == true)
        {
            myUnitOutfit.mySr.color = new Color(1, 1, 1, 1f);
            myUnitOutfit.mySr.sprite = myUnitOutfit.myHighLightSprite;
        }
        else
        {
            //Debug.Log("OnMouseEnter");
            //Debug.LogWarning("Exit還原");
            myUnitOutfit.mySr.color = new Color(1, 1, 1, 1f);
            myUnitOutfit.mySr.sprite = myUnitOutfit.myOriginalSprite;
        }

        roundManager.EnemyAttackRangeShowcaseReduce();
    }
    private void OnMouseDown()
    {
        SelectUnit();
    }
    public void PlaySelectSoundEffect()
    {
        soundManager.PlaySFX("button_press");

        soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
        soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
        soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
    }
    public void SelectUnit()
    {
        if (roundManager.roundState == RoundState.MyRound)
        {
            //Placement Skill Function Works Here.
            PlaySelectSoundEffect();

            if (roundManager.isCastingPlacementTrick)
            {
                //如果自己是可以被放置的範圍 有個旗標變數被打開
                if (isPlaceableTarget && TroopsOnMe == null)
                {
                    //這個地塊是空的
                    //他是系統認定的可放置目標

                    //如果點下去 觸發在自己身上放置目標，具體目標由方法傳入
                    //感覺參數不多可以直接巢狀下去
                    if (roundManager.isCastingTrick_StrawMan)
                    {
                        //代表傳入的是StrawMan
                        //依照等級去Call 
                        GameBoardInsChess GBIC = new GameBoardInsChess();

                        GBIC.chessFile = trickManager.TroopSpawnSwap_SO;
                        GBIC.locationX = myX;
                        GBIC.locationY = myY;

                        gameManager.SpawnLevelTroop(GBIC);

                        roundManager.isCastingPlacementTrick = false;
                        roundManager.isCastingTrick_StrawMan = false;
                        trickManager.ResetTargetPlace();
                        trickManager.myNowHoldTrickNum -= 1;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    //在Casting placement trick狀態下，視作覆蓋玩家移動功能，故不執行Player On MouseDown Event
                    //直接 return結束這一回合 避免夜長夢多
                    return;
                }
            }

            PlayerOnMouseDownEvent();
        }
    }

    public void PlayerOnMouseDownEvent()//此方法與Troop.cs中的EnemyOnMouseDownEvent相似 修改時請考慮到另外一邊
    {
        bool specialKillClog = false;
        switch (roundManager.roundState)
        {
            case RoundState.MyRound:

                //if(是我可以移動的範圍)
                if (roundManager.SelectObjectTroop == null)
                {
                    PlayerMoveToEmptyUnit();
                }
                else
                {
                    PlayerMoveToHasTroopUnit(specialKillClog);
                }
                //回傳GameMaster
                break;
        }
    }
    public void PlayerMoveToEmptyUnit()
    {
        //移動到空地塊上
        roundManager.selectingVector = new Vector2(myX, myY);
        roundManager.resetUnitSelectState();
        roundManager.SelectUnit = roundManager.gameManager.chessBoardObjectRefArr[myY, myX];
        //roundManager.SelectObjectTroop = roundManager.SelectObject.GetComponent<Troop>();
        //如果地塊上有自己的物件
        if (gameManager.PlayerTroop.myNowX == myX && gameManager.PlayerTroop.myNowY == myY)
        {
            Debug.Log("Triggered");
            Debug.Log("玩家行為");
            roundManager.SelectObjectTroop = gameManager.PlayerTroop;
            roundManager.Player_UpdateOnSelectChessAllowMoveVector();
        }
    }
    public void PlayerMoveToHasTroopUnit(bool specialKillClog)
    {
        if (isPlayerAllowMoveSpace)
        {
            Debug.Log("呼叫結束");
            #region 玩家攻擊相關代碼
            //TODO: 如果我身上有Troop 代表對Troop進行攻擊
            if (TroopsOnMe == null)
            {
                //TODO 如果選中目標是玩家棋子 如果我是可移動地塊 將玩家棋子移動到我身上(改變其XY)
                roundManager.SelectObjectTroop.myNowX = myX;
                roundManager.SelectObjectTroop.myNowY = myY;

                //無殺死目標
                roundManager.SpecialRoundEndFunc();
            }
            else
            {
                //開啟連殺
                specialKillClog = true;
                switch (TroopsOnMe.myCamp)
                {
                    case Camp.Enemy:
                        //若已有對手棋子，對其造成傷害
                        TroopsOnMe.hp -= gameManager.PlayerTroop.myChessData.AttackStr;

                        //播放音樂音效
                        PlayKillSoundEffect();

                        //若攻擊未殺死目標，則留在前一格
                        if (TroopsOnMe.hp <= 0)
                        {
                            //被殺死
                            roundManager.SelectObjectTroop.myNowX = myX;
                            roundManager.SelectObjectTroop.myNowY = myY;

                            if (gameManager.isCopySoulOn)
                            {
                                //Player Copy Soul
                                roundManager.SelectObjectTroop.CopySoul(TroopsOnMe);
                            }
                            TroopsOnMe.killTroop(gameObject);

                            gameManager.hintManager.SpawnHintWordPrefab("擊破 - " + TroopsOnMe.myChessData.chessName);

                        }
                        else
                        {
                            //沒被殺死
                            //先不管
                        }
                        break;
                }
            }
            #endregion

            roundManager.RoundSelectClean();

            //TODO 並且呼叫RoundMaster回合完成器
            if (specialKillClog)
            {
                //開始特殊回合
                roundManager.playerHitCombo++;
                //TOT
                vFXManager.SpawnHintGameObject(roundManager.playerHitCombo);
                trickManager.GainEnergyFromKill(roundManager.playerHitCombo);
                roundManager.StartSpecialRound(roundManager.playerHitCombo);
            }
            else
            {
                roundManager.MyRoundEnd();
            }
        }
    }

    public void UpdateLogic()
    {
        switch (roundManager.roundState)
        {
            case RoundState.MyRound:
                //TODO 如果選中目標是玩家棋子 如果我是可移動地塊 標記藍色
                isPlayerAllowMoveSpace = roundManager.IsMeSelectableUnit(new Vector2(myX, myY));
                if (isPlayerAllowMoveSpace)
                {
                    myUnitOutfit.mySr.color = new Color(1, 1, 1, 1f);
                    myUnitOutfit.mySr.sprite = myUnitOutfit.myHighLightSprite;
                    _colorClog = true;
                }
                else
                {
                    if (_colorClog)
                    {
                        _colorClog = false;
                        myUnitOutfit.mySr.color = new Color(1, 1, 1, 1);
                        myUnitOutfit.mySr.sprite = myUnitOutfit.myOriginalSprite;
                        //Debug.LogWarning("MyRound還原");
                    }
                }
                break;

            case RoundState.EnemyRound:
                if (_colorClog)
                {
                    _colorClog = false; 
                    myUnitOutfit.mySr.color = new Color(1, 1, 1, 1);
                    myUnitOutfit.mySr.sprite = myUnitOutfit.myOriginalSprite;
                    //Debug.LogWarning("Enemy還原");
                }
                break;

                /*
            case RoundState.MySpecialRound:
                if (gameManager.MyTroop != null)
                {
                    if (gameManager.MyTroop.TryGetComponent<Troop>(out Troop myTroopOut))
                    {
                        //我要選擇一個目標
                        roundManager.selectingVector = new Vector2(myX, myY);
                        roundManager.resetUnitSelectState();
                        roundManager.SelectObject = roundManager.gameManager.chessBoardObjectRefArr[myY, myX];
                        //roundManager.SelectObjectTroop = roundManager.SelectObject.GetComponent<Troop>();
                        //如果地塊上有自己的物件
                        if (gameManager.MyTroop.GetComponent<Troop>().myNowX == myX && gameManager.MyTroop.GetComponent<Troop>().myNowY == myY)
                        {
                            Debug.Log("Triggered" + gameObject.name);
                            roundManager.SelectObjectTroop = gameManager.MyTroop.GetComponent<Troop>();
                            roundManager.UpdateOnSelectChessAllowMoveVector();
                        }
                    }
                }

                isPlayerAllowMoveSpace = roundManager.IsMeSelectableUnit(new Vector2(myX, myY));
                //Debug.Log(gameObject.name + "觸發鍊" + isPlayerAllowMoveSpace);
                break;*/
        }
    }

    public void CaseSwitcher()
    {
        switch (roundManager.roundState)
        {
            case RoundState.MyRound:
                isAbleBeSelect = true;
                break;
            
            default:
                isAbleBeSelect = false;
                break;
        }
    }

    public void syncTroopOnMe()
    {
        foreach (GameObject gt in gameManager.Troops)
        {
            Troop t = gt.GetComponent<Troop>();
            if (t.myNowX == myX && t.myNowY == myY)
            {
                TroopsOnMe = t;
                return;
            }
        }
        TroopsOnMe = null;
    }

    

    public void PlayKillSoundEffect()
    {
        if (TroopsOnMe.myChessData.isGoldenTarget)
        {
            soundManager.PlaySFX("boss_slash_test_2");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            cameraManager.Shake(1f);
        }
        else if (roundManager.playerHitCombo >= 4)//連殺
        {
            Debug.Log("播放擊殺音效5");
            soundManager.PlaySFX("kill_5");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            cameraManager.Shake(0.6f);
        }
        else if (roundManager.playerHitCombo >= 3)
        {
            Debug.Log("播放擊殺音效4");
            soundManager.PlaySFX("kill_4");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            cameraManager.Shake(0.5f);
        }

        else if (roundManager.playerHitCombo >= 2)
        {
            Debug.Log("播放擊殺音效3");
            soundManager.PlaySFX("kill_3");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            cameraManager.Shake(0.4f);
        }

        else if (roundManager.playerHitCombo >= 1)
        {
            Debug.Log("播放擊殺音效2");
            soundManager.PlaySFX("kill_2");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            cameraManager.Shake(0.4f);
        }

        else
        {
            Debug.Log("播放擊殺音效1");
            soundManager.PlaySFX("kill_1");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            cameraManager.Shake(0.4f);
        }

    }
}
