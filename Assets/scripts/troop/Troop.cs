using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;
using AKTool;

public class Troop : MonoBehaviour
{
    [Header("System Setting")]
    public float chessMoveDuration = 0.2f;

    public int leftLife = 1;
    public bool energyHigh = false;

    [Header("Ref Components")]
    public GameManager gameManager;
    public SO_Chess myChessData;
    public bool isPlayer = false;
    public RoundManager roundManager;
    public SoundManager soundManager;
    public TrickManager trickManager;
    public VFXManager vFXManager;

    public TroopOutfit troopOutfit;

    public Camp myCamp;

    public int surviveRound = 0;

    [Range(0, 9)]
    public int myNowX = 0;
    [Range(0, 9)]
    public int myNowY = 0;

    public int hp = 1;

    public SpriteRenderer mySr;

    public gear holdingGear = gear.noGear;
    public bool isPassable = false;
    public BucketType bucketType;

    [Header("棋子UI顯示")]
    public Text MySurviveRoundShowCase;
    public string myName;
    public string myDesc;

    [Header("能力")]
    public ability[] myAbilities;


    [Header("旗子移動數值")]
    public int horBlockMoveAbility = 0;
    public int verticalBlockMoveAbility = 0;
    public int diagonalBlockMoveAbility = 0;
    public int knightBlockMoveAbility = 0;
    public int AttackStr = 1;

    [Header("盾牌")]
    public bool hasUpperShield = false;
    public bool hasLowerShield = false;
    public bool hasLeftShield = false;
    public bool hasRightShield = false;

    [Header("Actions")]
    public Action Action_OnRoundEnd = () => { };
    public Action Action_PowerActiveOnce = () => { };
    public Action OnDieAction = () => { };

    [Header("殺死我的目標")]
    public GameObject TargetThatMurderMe;

    public void LoadSOData()
    {
        hp = myChessData.hp;

        mySr.sprite = myChessData.skin;

        myCamp = myChessData.myCamp;
        holdingGear = myChessData.spawnGear;

        horBlockMoveAbility = myChessData.horBlockMoveAbility;
        verticalBlockMoveAbility = myChessData.verticalBlockMoveAbility;
        diagonalBlockMoveAbility = myChessData.diagonalBlockMoveAbility;
        knightBlockMoveAbility = myChessData.knightBlockMoveAbility;

        AttackStr = myChessData.AttackStr;

        myAbilities = myChessData.abilities;


        if (myCamp == Camp.Enemy)
        {
            gameObject.AddComponent<EnemyLogic>();
        }

        string[] langData = new string[] { };
        if (myChessData.myMutiLangData == null)
        {
            Debug.LogError("來自物件" + gameObject.name + "myChessData.myMutiLangData缺失");
        }
        AK_ToolBox.LoadLangData(myChessData.myMutiLangData, ref langData);

        myName = langData[0];
        myDesc = langData[1];

        chessMoveDuration = AllGameManager.chessMoveDuration;
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        roundManager = FindFirstObjectByType<RoundManager>();
        soundManager = FindFirstObjectByType<SoundManager>();
        trickManager = FindFirstObjectByType<TrickManager>();
        vFXManager = FindAnyObjectByType<VFXManager>();
        LoadSOData();

        if (myCamp == Camp.Enemy)
        {
            RoundManager RM = FindAnyObjectByType<RoundManager>();
            RM.EnemyAITroop.Add(this);
        }

        for (int i = 0; i < myAbilities.Length; i++)
        {
            TroopAbilityApply(myAbilities[i]);
        }

        OnDieAction += () => { roundManager.leftRoundCount -= 1; Debug.Log("Kill Process works"); };
    }

    void Update()
    {
        myUISync();
        myPosSync();
    }
    private void FixedUpdate()
    {

    }
    public void myPosSync()
    {
        if (gameManager.chessBoardObjectRefArr.GetLength(0) > myNowX && gameManager.chessBoardObjectRefArr.GetLength(1) > myNowY && myNowX >= 0 && myNowY >= 0)
        {
            Vector2 vec = gameManager.chessBoardObjectRefArr[myNowY, myNowX].transform.position;

            //transform.position = vec;
            transform.DOMoveX(vec.x, chessMoveDuration);
            transform.DOMoveY(vec.y, chessMoveDuration);
        }
    }

    public void myUISync()
    {
        if (myCamp == Camp.Player)
        {
            MySurviveRoundShowCase.text = "";
        }
        else
        {
            if (MySurviveRoundShowCase != null)
            {
                //MySurviveRoundShowCase.text = (surviveRound + 1) + "";
                int largest = 0;
                largest = Mathf.Max(largest, verticalBlockMoveAbility);
                largest = Mathf.Max(largest, horBlockMoveAbility);
                largest = Mathf.Max(largest, diagonalBlockMoveAbility);
                largest = Mathf.Max(largest, knightBlockMoveAbility);

                MySurviveRoundShowCase.text = (largest) + "";

            }
        }
    }

    public void killTroop(GameObject murderer)
    {
        TargetThatMurderMe = murderer;
        killTroop();
    }
    public void killTroop()
    {
        //TODO: 將自己從註冊表中移除
        if (myChessData.isGoldenTarget)
        {
            //Let Player Win.
            gameManager.FrameSkipping(0.1f, 1.2f);
            roundManager.Win();
        }

        if (myCamp == Camp.Player)
        {
            gameManager.FrameSkipping(0.1f, 2f);
            //這邊是基本上沒有用的
        }

        if (myCamp == Camp.Enemy)
        {
            RoundManager RM = FindAnyObjectByType<RoundManager>();
            RM.EnemyAITroop.Remove(this);
            roundManager.specialClogAutoSelectionClog = true;
            gameManager.FrameSkipping();
            vFXManager.VFX_SlashInHalf(this);


        }

        gameManager.Troops.Remove(gameObject);

        if (OnDieAction != null)
        {
            OnDieAction();
        }

        troopOutfit.DieVFX();


        Destroy(gameObject);
    }

    [Header("敵人邏輯宣告")]
    public List<Vector2> OnSelectChessAllowMoveVector = new List<Vector2>();

    public Vector2 myNextDes;
    #region 敵人邏輯
    public void MoveToNext()
    {
        EnemyLogic();
        myNextDes = ClosestVector();

        for (int i = 0; i < myAbilities.Length; i++)
        {
            if (myAbilities[i] == ability.Retard)
            {
                myNextDes = new Vector2(-1, -1);
                Debug.Log("自動跳過回合");
            }
        }

        if (myNextDes == new Vector2(-1, -1))
        {
            //代表無法移動
            //Debug.Log("無法移動");
        }
        else
        {

            myNowX = (int)myNextDes.x;
            myNowY = (int)myNextDes.y;

            if (myNowX < 0)
            {
                Debug.Log("myNowX - " + myNowX);
                Debug.LogError("Out of Range");
            }
            if (myNowY < 0)
            {
                Debug.Log("myNowY - " + myNowY);
                Debug.LogError("Out of Range");
            }

            //Play sound effect
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");
            soundManager.PlaySFX("Wooden_Floor_Walking_Sound_3");

            //傷害判定
            EnemyOnMouseDownEvent(gameManager.chessBoardObjectRefArr[myNowY, myNowX].GetComponent<unit>());
        }
    }

    public Vector2 ClosestVector()
    {
        Vector2 tar = new Vector2();
        Vector2 compareVec = new Vector2(gameManager.Troops[0].GetComponent<Troop>().myNowX, gameManager.Troops[0].GetComponent<Troop>().myNowY);

        float d = 100000f;

        if (OnSelectChessAllowMoveVector.Count <= 0)
        {
            return new Vector2(-1, -1);
        }
        else
        {
            foreach (Vector2 vec in OnSelectChessAllowMoveVector)
            {
                if (Vector2.Distance(vec, compareVec) < d)
                {
                    d = Vector2.Distance(vec, compareVec);
                    tar = vec;
                }
            }
        }

        return tar;
    }


    public void EnemyLogic()
    {
        //Debug.Log("Enemy Logic Trigger");
        OnSelectChessAllowMoveVector.Clear();
        UpdateOnSelectChessAllowMoveVector(OnSelectChessAllowMoveVector, this);
        //For Mob Reduce
        ReduceOnSelectChessAllowMoveVector();
    }

    public void EnemyCalculateAttackRange()
    {
        OnSelectChessAllowMoveVector.Clear();
        UpdateOnSelectChessAllowMoveVector(OnSelectChessAllowMoveVector, this);
        ReduceOnSelectChessAllowMoveVector_ForLogicCalculationSpecialFunc_RememberToMakeItOverride();
    }

    public void ReduceOnSelectChessAllowMoveVector()
    {
        for (int i = OnSelectChessAllowMoveVector.Count - 1; i >= 0; i--)
        {
            // 先把目前要比的目標存起來，避免移除後再用索引取值
            var target = OnSelectChessAllowMoveVector[i];

            if (target.x < 0 || target.y < 0 || target.x >= gameManager.levelData.gridSizeX || target.y >= gameManager.levelData.gridSizeY)
            {
                OnSelectChessAllowMoveVector.RemoveAt(i);
                continue; // 這個 i 處理完，換下一個
            }

            foreach (var obj in gameManager.chessBoardObjectRefArr)
            {
                if (obj == null) continue;
                if (!obj.TryGetComponent<unit>(out unit u)) continue;
                if (u.TroopsOnMe == null) continue;//二次檢查

                if (u.TroopsOnMe.myCamp == Camp.Player) continue; //玩家保留顯示

                //往下都是不允許攻擊的Reduce

                // 這段GPT改的
                var pos = new Vector2Int(u.myX, u.myY);
                if (pos == Vector2Int.RoundToInt(target))
                {
                    OnSelectChessAllowMoveVector.RemoveAt(i);
                    break; // 避免在同一個 i 上繼續讀取已被縮短的 List
                }

                if (target.x < 0 || target.y < 0 || target.x >= gameManager.levelData.gridSizeX || target.y >= gameManager.levelData.gridSizeY)
                {
                    OnSelectChessAllowMoveVector.RemoveAt(i);
                    //Debug.Log("Out of range解釋");
                    //保險起見的邏輯 其實已經不會執行到
                    break;
                }
            }
        }
    }
    public void ReduceOnSelectChessAllowMoveVector_ForLogicCalculationSpecialFunc_RememberToMakeItOverride()
    {
        for (int i = OnSelectChessAllowMoveVector.Count - 1; i >= 0; i--)
        {
            var target = OnSelectChessAllowMoveVector[i];

            if (target.x < 0 || target.y < 0 || target.x >= gameManager.levelData.gridSizeX || target.y >= gameManager.levelData.gridSizeY)
            {
                OnSelectChessAllowMoveVector.RemoveAt(i);
                continue; // 這個 i 處理完，換下一個
            }


            foreach (var obj in gameManager.chessBoardObjectRefArr)
            {
                if (obj == null) continue;
                if (!obj.TryGetComponent<unit>(out unit u)) continue;
                if (u.TroopsOnMe == null) continue;

                if (u.TroopsOnMe.myCamp == Camp.Player) continue;
                if (u.TroopsOnMe.myCamp == Camp.Enemy) continue;

                //往下都是不允許攻擊的Reduce

                // 這段GPT改的
                var pos = new Vector2Int(u.myX, u.myY);
                if (pos == Vector2Int.RoundToInt(target))
                {
                    OnSelectChessAllowMoveVector.RemoveAt(i);
                    break; // 避免在同一個 i 上繼續讀取已被縮短的 List
                }

                if (target.x < 0 || target.y < 0 || target.x >= gameManager.levelData.gridSizeX || target.y >= gameManager.levelData.gridSizeY)
                {
                    OnSelectChessAllowMoveVector.RemoveAt(i);
                    //Debug.Log("Out of range解釋");
                    break;
                }
            }
        }
    }
    public void EnemyOnMouseDownEvent(unit tarUnit) //此方法與unit.cs中的PlayerOnMouseDownEvent相似 修改時請考慮到另外一邊
    {
        //Debug.Log("tarUnit xy - " + tarUnit.myX + "/" + tarUnit.myY);
        if (tarUnit.TroopsOnMe == null)
        {
            //不攻擊純移動
        }
        else
        {

            switch (tarUnit.TroopsOnMe.myCamp)
            {
                case Camp.Enemy: //友方棋子 不該行動或是造成傷害

                    /*TroopsOnMe.hp -= gameManager.MyTroop.GetComponent<Troop>().myChessData.AttackStr;
                    //若攻擊未殺死目標，則留在前一格
                    if (TroopsOnMe.hp <= 0)
                    {
                        //被殺死
                        roundManager.SelectObjectTroop.myNowX = myX;
                        roundManager.SelectObjectTroop.myNowY = myY;
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
                    }*/
                    break;

                case Camp.Bucket: //場地互動道具 如.爆破桶等
                    tarUnit.TroopsOnMe.hp -= gameManager.PlayerTroop.myChessData.AttackStr;
                    gameManager.hintManager.SpawnHintWordPrefab("MOB擊破桶子 - " + tarUnit.TroopsOnMe.myChessData.chessName);

                    //被殺死
                    tarUnit.TroopsOnMe.killTroop();

                    //依照種類觸發效果 記得可以合併到unit裡面觸發
                    switch (tarUnit.TroopsOnMe.bucketType)
                    {
                        case BucketType.noType:
                            Debug.Log("MOB 無事發生 歲月靜好 你選擇了一個Bucket類 但沒有選擇觸發效果");
                            break;

                    }
                    break;

                case Camp.Item: //道具

                    //被殺死
                    tarUnit.TroopsOnMe.killTroop();
                    roundManager.SelectObjectTroop = this;
                    roundManager.SelectObjectTroop.holdingGear = tarUnit.TroopsOnMe.holdingGear;
                    gameManager.hintManager.SpawnHintWordPrefab("MOB 得到道具 - " + tarUnit.TroopsOnMe.myChessData.chessName);
                    roundManager.SelectObjectTroop = null;
                    break;

                case Camp.Player:
                    //Kill Player
                    roundManager.MakePlayerDie();
                    break;
            }
        }
    }
    #endregion

    public void UpdateOnSelectChessAllowMoveVector(List<Vector2> Vec2List, Troop T) // 可移動地塊更新 重要函式！！！
    {
        //Debug.Log(gameObject.name + "\n camp is:" + myCamp + "\nUpdateOnSelectChessAllowMoveVector Triggered.");
        Vec2List.Clear();
        //get current XY
        Vector2 currentXY = new Vector2(T.myNowX, T.myNowY);
        //Hor select cal
        for (int i = 1; T.horBlockMoveAbility >= i; i++)
        {
            Vec2List.Add(new Vector2(currentXY.x + i, currentXY.y));
            Vec2List.Add(new Vector2(currentXY.x - i, currentXY.y));
        }

        for (int i = 1; T.verticalBlockMoveAbility >= i; i++)
        {
            Vec2List.Add(new Vector2(currentXY.x, currentXY.y + i));
            Vec2List.Add(new Vector2(currentXY.x, currentXY.y - i));
        }

        for (int i = 1; T.diagonalBlockMoveAbility >= i; i++)
        {
            Vec2List.Add(new Vector2(currentXY.x + i, currentXY.y + i));
            Vec2List.Add(new Vector2(currentXY.x + i, currentXY.y - i));
            Vec2List.Add(new Vector2(currentXY.x - i, currentXY.y + i));
            Vec2List.Add(new Vector2(currentXY.x - i, currentXY.y - i));
        }

        for (int i = 1; T.knightBlockMoveAbility >= i; i++)
        {
            Vec2List.Add(new Vector2(currentXY.x - 1, currentXY.y + 2));
            Vec2List.Add(new Vector2(currentXY.x + 1, currentXY.y + 2));
            Vec2List.Add(new Vector2(currentXY.x - 2, currentXY.y + 1));
            Vec2List.Add(new Vector2(currentXY.x - 2, currentXY.y + -1));
            Vec2List.Add(new Vector2(currentXY.x + 2, currentXY.y + 1));
            Vec2List.Add(new Vector2(currentXY.x + 2, currentXY.y - 1));
            Vec2List.Add(new Vector2(currentXY.x + 1, currentXY.y - 2));
            Vec2List.Add(new Vector2(currentXY.x - 1, currentXY.y - 2));
        }

        switch (T.holdingGear)
        {
            case gear.car:
                for (int i = 1; 8 >= i; i++)
                {
                    Vec2List.Add(new Vector2(currentXY.x + i, currentXY.y));
                    Vec2List.Add(new Vector2(currentXY.x - i, currentXY.y));
                    Vec2List.Add(new Vector2(currentXY.x, currentXY.y + i));
                    Vec2List.Add(new Vector2(currentXY.x, currentXY.y - i));
                }

                T.holdingGear = gear.noGear;
                break;
        }

        GeneralReduceRule(Vec2List, T);
    }
    public void GeneralReduceRule(List<Vector2> Vec2List, Troop T)
    {
        //Debug.Log("學士路東觸發");
        //shield reduce - 上盾牌 抵擋來自上方的攻擊 即檢測玩家正下方地塊目標

        #region 能量高漲
        if (T.energyHigh)
        {
            return;
        }
        #endregion

        #region 地形檢測 - 該格可站立
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            if (!gameManager.isVectorLegal(Vec2List[i])) continue;

            Structure ST = gameManager.GetUnitAt((int)Vec2List[i].x, (int)Vec2List[i].y).StructureOnMe;
            if (ST == null) continue;

            if (ST.isAllowStanding)
            {
                //可以站上去
                continue;
            }

            if (ST.isAttackableTarget)
            {
                //進入判斷
                if (ST.isRequireEnergyHigh)
                {
                    if (energyHigh)
                    {
                        //可以破壞
                        continue;
                    }
                    else
                    {
                        Vec2List.Remove(Vec2List[i]);
                    }
                }
                else
                {
                    continue;
                }
            }
            else
            {
                //無敵
                Vec2List.Remove(Vec2List[i]);
            }
        }
        #endregion

        #region 上盾牌
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            //檢測為玩家下方, 即x相同, y小於的區塊, 這個區域內的上盾牌可以發揮作用
            if (Vec2List[i].x == T.myNowX && Vec2List[i].y < T.myNowY)
            {
                //檢測該座標是否存在
                if (!gameManager.isVectorLegal(Vec2List[i])) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasUpperShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    if (T.isPlayer)
                    {
                        ST.troopOutfit.TriggerUntouchableHint();
                    }
                }
            }
        }
        #endregion

        //shield reduce - 下盾牌 抵擋來自下方的攻擊 即檢測玩家正上方地塊目標
        #region 下盾牌
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            //檢測為玩家下方, 即x相同, y小於的區塊, 這個區域內的上盾牌可以發揮作用
            if (Vec2List[i].x == T.myNowX && Vec2List[i].y > T.myNowY)
            {
                //檢測該座標是否存在
                if (!gameManager.isVectorLegal(Vec2List[i])) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasLowerShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    if (T.isPlayer)
                    {
                        ST.troopOutfit.TriggerUntouchableHint();
                    }
                }
            }
        }
        #endregion

        //shield reduce - 左盾牌 抵擋來自左方的攻擊 即檢測玩家正右方地塊目標
        #region 左盾牌
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            if (Vec2List[i].x > T.myNowX && Vec2List[i].y == T.myNowY)
            {
                //檢測該座標是否存在
                if (!gameManager.isVectorLegal(Vec2List[i])) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasLeftShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    if (T.isPlayer)
                    {
                        ST.troopOutfit.TriggerUntouchableHint();
                    }
                }
            }
        }
        #endregion
        #region 右盾牌
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            if (Vec2List[i].x < T.myNowX && Vec2List[i].y == T.myNowY)
            {
                //檢測該座標是否存在
                if (!gameManager.isVectorLegal(Vec2List[i])) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasRightShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    if (T.isPlayer)
                    {
                        ST.troopOutfit.TriggerUntouchableHint();
                    }
                }
            }
        }
        #endregion

        #region 連擊盾
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            //檢測該座標是否存在
            if (!gameManager.isVectorLegal(Vec2List[i])) continue;
            //檢測該座標對應區塊是否有單位存在
            Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
            if (ST == null) continue;

            //檢測該單位是否擁有連擊盾牌 有則將其於可移動地塊中刪除
            for (int j = 0; j < ST.myAbilities.Length; j++)
            {
                switch (ST.myAbilities[j])
                {
                    case ability.HitShield_1:
                        if (roundManager.playerHitCombo >= 1)
                        {
                            Debug.Log("連擊盾1 擊破");
                        }
                        else
                        {
                            Vec2List.Remove(Vec2List[i]);
                            if (T.isPlayer)
                            {
                                ST.troopOutfit.TriggerUntouchableHint();
                            }
                            Debug.Log("連擊盾1 未擊破");
                        }
                        break;

                    case ability.HitShield_2:
                        if (roundManager.playerHitCombo >= 2)
                        {
                            Debug.Log("連擊盾2 擊破");
                        }
                        else
                        {
                            Debug.Log("連擊盾2 未擊破");
                            Vec2List.Remove(Vec2List[i]);
                            if (T.isPlayer)
                            {
                                ST.troopOutfit.TriggerUntouchableHint();
                            }
                        }
                        break;

                    case ability.HitShield_3:
                        if (roundManager.playerHitCombo >= 3)
                        {
                            Debug.Log("連擊盾3 擊破");
                        }
                        else
                        {
                            Vec2List.Remove(Vec2List[i]);
                            if (T.isPlayer)
                            {
                                ST.troopOutfit.TriggerUntouchableHint();
                            }
                            Debug.Log("連擊盾3 未擊破");
                        }
                        break;
                }
            }

        }
        #endregion

        RemoveBlockedCellsFromList(Vec2List, myNowX, myNowY);
    }

    public void PlayerDieReaction() //千萬別從這個腳本直接呼叫！！
    {
        //gameObject.transform.GetChild(1).gameObject.GetComponent<Troop>().mySr.color = Color.red;
        //gameManager.PlayerTroop.mySr.color = Color.red;
        gameManager.PlayerTroop.mySr.color = new Color(1, 1, 1, 0); //it can't be see;
        troopOutfit.myAnimator.Play("AlphaState");
        vFXManager.VFX_SlashInHalf(this);
    }

    public void TroopAbilityApply(ability abi)
    {
        switch (abi)
        {
            case ability.UpperShield:
                hasUpperShield = true;
                break;

            case ability.LowerShield:
                hasLowerShield = true;
                break;

            case ability.LeftShield:
                hasLeftShield = true;
                break;

            case ability.RightShield:
                hasRightShield = true;
                break;

            case ability.KarenBorn:
                var swapTSA_karenBorn = gameObject.AddComponent<TSA_KarenBorn>();
                swapTSA_karenBorn.myTroop = this;
                break;
            case ability.Rager:
                var swapTSA_rager = gameObject.AddComponent<TSA_Rager>();
                swapTSA_rager.myTroop = this;
                break;
            case ability.SuicideBomb:
                var swapTSA_suicideBomb = gameObject.AddComponent<TSA_SuicideBomb>();
                swapTSA_suicideBomb.myTroop = this;
                break;
            case ability.player:
                var swapTSA_player = gameObject.AddComponent<TSA_Player>();
                swapTSA_player.myTroop = this;
                break;

            case ability.goldenTarget:
                var spawnTSA_goldenTarget = gameObject.AddComponent<TSA_GoldenTarget>();
                spawnTSA_goldenTarget.myTroop = this;
                break;  
        }
    }

    public void ChessEvo()
    {
        if (myCamp == Camp.Enemy)
        {
            for (int i = 0; i < myAbilities.Length; i++)
            {
                EvoFunction(myAbilities[i]);
            }
        }
    }
    public void EvoFunction(ability abi)
    {
        switch (abi)
        {
            case ability.evo_HorMoveAbility:
                horBlockMoveAbility++;
                break;


            case ability.evo_VarMoveAbility:
                verticalBlockMoveAbility++;
                break;


            case ability.evo_XMoveAbility:
                diagonalBlockMoveAbility++;
                break;
        }
    }

    //複製玩法
    public void CopySoul(Troop otherT)
    {
        horBlockMoveAbility = otherT.horBlockMoveAbility;
        verticalBlockMoveAbility = otherT.verticalBlockMoveAbility;
        diagonalBlockMoveAbility = otherT.diagonalBlockMoveAbility;
        knightBlockMoveAbility = otherT.knightBlockMoveAbility;

        hasLeftShield = otherT.hasLeftShield;
        hasRightShield = otherT.hasRightShield;
        hasLowerShield = otherT.hasLowerShield;
        hasUpperShield = otherT.hasUpperShield;

        //Additional Soul
        if (SaveSystem.SF.talentTreeUnlock[4])
        {
            horBlockMoveAbility += 1;
            verticalBlockMoveAbility += 1;
        }

        if (SaveSystem.SF.talentTreeUnlock[12])
        {
            diagonalBlockMoveAbility += 1;
        }
    }

    public void CleanFunction_Action_PowerActiveOnce()
    {
        Action_PowerActiveOnce = () => { };
    }

    //GROK提供
    public void RemoveBlockedCellsFromList(List<Vector2> vec2List, int myX, int myY)
    {
        // 為了避免在迭代過程中修改List導致錯誤
        // 先複製一份出來處理，或使用倒序遍歷
        List<Vector2> toRemove = new List<Vector2>();

        // 對每個候選格子，檢查從玩家到該格子的直線是否被UnPassable擋住
        for (int i = vec2List.Count - 1; i >= 0; i--)
        {
            Vector2 target = vec2List[i];
            int tx = (int)target.x;
            int ty = (int)target.y;

            // 自己位置不用檢查
            if (tx == myX && ty == myY) continue;

            // 取得方向向量
            int dx = tx - myX;
            int dy = ty - myY;

            // 取得步進的最大距離（曼哈頓或切比雪夫都可，這裡用較精確的Bresenham思路）
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            if (steps == 0) continue;

            // 單位向量（浮點）
            float stepX = dx / (float)steps;
            float stepY = dy / (float)steps;

            bool blocked = false;

            // 從玩家旁邊第一格開始檢查，直到目標前一格
            for (int step = 1; step < steps; step++) // 注意不包含目標格自己
            {
                float cx = myX + stepX * step;
                float cy = myY + stepY * step;

                int checkX = Mathf.RoundToInt(cx);
                int checkY = Mathf.RoundToInt(cy);

                // 取得該格的結構
                if (!gameManager.isVectorLegal(new Vector2(checkX, checkY))) continue;
                var unit = gameManager.GetUnitAt(checkX, checkY);
                if (unit == null) continue;

                Structure ST = unit.StructureOnMe;
                if (ST != null && ST.isContainAbility(StructureAbility.UnPassable))
                {
                    blocked = true;
                    break;
                }
            }

            // 如果中間有UnPassable阻擋 → 這個目標格也要移除
            if (blocked)
            {
                toRemove.Add(target);
            }
        }

        // 最後統一移除
        foreach (var v in toRemove)
        {
            vec2List.Remove(v);
        }
    }
}