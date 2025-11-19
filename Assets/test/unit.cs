
using JetBrains.Annotations;
using UnityEngine;

public class unit : MonoBehaviour
{
    public GameManager gameManager;
    public RoundManager roundManager;
    public SpriteRenderer mySr;
    public SoundManager soundManager;

    public Sprite myOriginalSprite;
    public Sprite myHighLightSprite;

    public int myX;
    public int myY;

    public bool selecting = false;
    public bool passable = true; //可通過地塊

    public bool isDeployed = false; //已被占用部署地塊
    public bool isAbleToDeploy = false; //是起始允許部署地塊

    public bool isPlayerAllowMoveSpace; //是玩家可以站在地地方

    public Troop TroopsOnMe;

    private void Awake()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
    }

    public void ApplyPerform(string ID)
    {
        //make sr sprite eul to the current ID
        if (ID == "")
        {
            mySr.sprite = Resources.Load<Sprite>("TerrainSprite/TS_Default");
            myOriginalSprite = Resources.Load<Sprite>("TerrainSprite/TS_Default_highLighted");
        }
        else
        {
            mySr.sprite = Resources.Load<Sprite>("TerrainSprite/" + ID);
            myOriginalSprite = Resources.Load<Sprite>("TerrainSprite/" + ID);
            myHighLightSprite = Resources.Load<Sprite>("TerrainSprite/" + ID + "_HL");
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
                mySr.color = new Color(1, 1, 1, 1f);
                mySr.sprite = myHighLightSprite;
            }
            else
            {
                //Debug.Log("OnMouseEnter");
                mySr.color = new Color(1, 1, 1, 0.2f);
                mySr.sprite = myOriginalSprite;
            }
        }
    }
    private void OnMouseExit()
    {
        if (selecting == true)
        {
            mySr.color = new Color(1, 1, 1, 1f);
            mySr.sprite = myHighLightSprite;
        }
        else
        {
            //Debug.Log("OnMouseEnter");
            //Debug.LogWarning("Exit還原");
            mySr.color = new Color(1, 1, 1, 1f);
            mySr.sprite = myOriginalSprite;
        }
    }
    private void OnMouseDown()
    {
        if (roundManager.roundState == RoundState.MyRound)
        {
            soundManager.PlaySFX("button_press");

            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");

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
                    //我要選擇一個目標
                    roundManager.selectingVector = new Vector2(myX, myY);
                    roundManager.resetUnitSelectState();
                    roundManager.SelectObject = roundManager.gameManager.chessBoardObjectRefArr[myY, myX];
                    //roundManager.SelectObjectTroop = roundManager.SelectObject.GetComponent<Troop>();
                    //如果地塊上有自己的物件
                    if (gameManager.MyTroop.GetComponent<Troop>().myNowX == myX && gameManager.MyTroop.GetComponent<Troop>().myNowY == myY)
                    {
                        Debug.Log("Triggered");
                        Debug.Log("玩家行為");
                        roundManager.SelectObjectTroop = gameManager.MyTroop.GetComponent<Troop>();
                        roundManager.UpdateOnSelectChessAllowMoveVector();
                    }
                }
                else
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
                            specialKillClog = true;
                            switch (TroopsOnMe.myCamp)
                            {
                                case Camp.Enemy:
                                    //若已有對手棋子，對其造成傷害
                                    TroopsOnMe.hp -= gameManager.MyTroop.GetComponent<Troop>().myChessData.AttackStr;

                                    Debug.Log("播放擊殺音效");

                                    //播放音樂音效
                                    if (TroopsOnMe.myChessData.isGoldenTarget)
                                    {
                                        soundManager.PlaySFX("boss_slash_test_2");
                                    }
                                    else if (roundManager.playerHitCombo >= 4)//連殺
                                    {
                                        Debug.Log("播放擊殺音效5");
                                        soundManager.PlaySFX("kill_5");
                                    }
                                    else if (roundManager.playerHitCombo >= 3)
                                    {
                                        Debug.Log("播放擊殺音效4");
                                        soundManager.PlaySFX("kill_4");
                                    }

                                    else if (roundManager.playerHitCombo >= 2)
                                    {
                                        Debug.Log("播放擊殺音效3");
                                        soundManager.PlaySFX("kill_3");
                                    }

                                    else if (roundManager.playerHitCombo >= 1)
                                    {
                                        Debug.Log("播放擊殺音效2");
                                        soundManager.PlaySFX("kill_2");
                                    }

                                    else
                                    {
                                        Debug.Log("播放擊殺音效1");
                                        soundManager.PlaySFX("kill_1");
                                    }

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
                                        TroopsOnMe.killTroop();

                                        gameManager.hintManager.SpawnHintWordPrefab("擊破 - " + TroopsOnMe.myChessData.chessName);

                                        //奪取道具？
                                        if (TroopsOnMe.holdingGear != gear.noGear)
                                        {
                                            roundManager.SelectObjectTroop.holdingGear = TroopsOnMe.holdingGear;
                                            gameManager.hintManager.SpawnHintWordPrefab("搶奪道具 - " + TroopsOnMe.holdingGear);
                                        }
                                    }
                                    else
                                    {
                                        //沒被殺死
                                        //先不管
                                    }
                                    break;

                                case Camp.Bucket: //場地互動道具 如.爆破桶等
                                    TroopsOnMe.hp -= gameManager.MyTroop.GetComponent<Troop>().myChessData.AttackStr;
                                    gameManager.hintManager.SpawnHintWordPrefab("擊破桶子 - " + TroopsOnMe.myChessData.chessName);

                                    //被殺死
                                    roundManager.SelectObjectTroop.myNowX = myX;
                                    roundManager.SelectObjectTroop.myNowY = myY;
                                    TroopsOnMe.killTroop();

                                    //依照種類觸發效果
                                    switch (TroopsOnMe.bucketType)
                                    {
                                        case BucketType.noType:
                                            Debug.Log("無事發生 歲月靜好 你選擇了一個Bucket類 但沒有選擇觸發效果");
                                            break;

                                    }
                                    break;

                                case Camp.Item: //道具

                                    //被殺死
                                    roundManager.SelectObjectTroop.myNowX = myX;
                                    roundManager.SelectObjectTroop.myNowY = myY;
                                    TroopsOnMe.killTroop();

                                    roundManager.SelectObjectTroop.holdingGear = TroopsOnMe.holdingGear;
                                    gameManager.hintManager.SpawnHintWordPrefab("得到道具 - " + TroopsOnMe.myChessData.chessName);
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
                            roundManager.StartSpecialRound(roundManager.playerHitCombo);
                        }
                        else
                        {
                            roundManager.MyRoundEnd();
                        }
                    }
                }
                //回傳GameMaster
                break;

            case RoundState.MySpecialRound:


                break;
        }
    }

    bool _colorClog;
    void Update()
    {
        switch (roundManager.roundState) {
            case RoundState.MyRound:
                //TODO 如果選中目標是玩家棋子 如果我是可移動地塊 標記藍色
                isPlayerAllowMoveSpace = roundManager.IsMeSelectableUnit(new Vector2(myX, myY));
                if (isPlayerAllowMoveSpace)
                {
                    mySr.color = new Color(1, 1, 1, 1f);
                    mySr.sprite = myHighLightSprite;
                    _colorClog = true;
                }
                else
                {
                    if (_colorClog)
                    {
                        _colorClog = false; 
                        mySr.color = new Color(1, 1, 1, 1);
                        mySr.sprite = myOriginalSprite;
                        //Debug.LogWarning("MyRound還原");
                    }
                }
                break;

            case RoundState.EnemyRound:
                if (_colorClog)
                {
                    _colorClog = false; mySr.color = new Color(1, 1, 1, 1);
                    mySr.sprite = myOriginalSprite;
                    //Debug.LogWarning("Enemy還原");
                }
                break;

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
                break;
        }

        syncTroopOnMe();
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
}
