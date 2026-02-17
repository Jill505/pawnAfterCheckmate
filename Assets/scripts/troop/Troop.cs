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
                MySurviveRoundShowCase.text = (surviveRound +1) + "";
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


        #region 上盾牌
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            //檢測為玩家下方, 即x相同, y小於的區塊, 這個區域內的上盾牌可以發揮作用
            if (Vec2List[i].x == T.myNowX && Vec2List[i].y < T.myNowY)
            {
                //檢測該座標是否存在
                if (Vec2List[i].x < 0 || Vec2List[i].y < 0 || Vec2List[i].x >= gameManager.levelData.gridSizeX || Vec2List[i].y >= gameManager.levelData.gridSizeY) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasUpperShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    Debug.Log("學士路東觸發 - 上");
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
                if (Vec2List[i].x < 0 || Vec2List[i].y < 0 || Vec2List[i].x >= gameManager.levelData.gridSizeX || Vec2List[i].y >= gameManager.levelData.gridSizeY) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasLowerShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    Debug.Log("學士路東觸發 - 下");
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
                if (Vec2List[i].x < 0 || Vec2List[i].y < 0 || Vec2List[i].x >= gameManager.levelData.gridSizeX || Vec2List[i].y >= gameManager.levelData.gridSizeY) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasLeftShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    Debug.Log("學士路東觸發 - 左");
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
                if (Vec2List[i].x < 0 || Vec2List[i].y < 0 || Vec2List[i].x >= gameManager.levelData.gridSizeX || Vec2List[i].y >= gameManager.levelData.gridSizeY) continue;
                //檢測該座標對應區塊是否有單位存在
                Troop ST = gameManager.chessBoardObjectRefArr[(int)Vec2List[i].y, (int)Vec2List[i].x].GetComponent<unit>().TroopsOnMe;
                if (ST == null) continue;

                //檢測該單位是否擁有上盾牌 有則將其於可移動地塊中刪除
                if (ST.hasRightShield)
                {
                    Vec2List.Remove(Vec2List[i]);
                    Debug.Log("學士路東觸發 - 右");
                }
            }
        }
        #endregion

        #region 連擊盾
        for (int i = Vec2List.Count - 1; i >= 0; i--)
        {
            //檢測該座標是否存在
            if (Vec2List[i].x < 0 || Vec2List[i].y < 0 || Vec2List[i].x >= gameManager.levelData.gridSizeX || Vec2List[i].y >= gameManager.levelData.gridSizeY) continue;
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
                            Debug.Log("連擊盾3 未擊破");
                        }
                        break;
                }
            }

        }
        #endregion
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
    }

    public void CleanFunction_Action_PowerActiveOnce()
    {
        Action_PowerActiveOnce = () => { };
    }
}


