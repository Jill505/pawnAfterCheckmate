
using UnityEngine;

public class unit : MonoBehaviour
{
    public GameManager gameManager;
    public RoundManager roundManager;
    public SpriteRenderer mySr;

    public int myX;
    public int myY;

    public bool selecting = false;
    public bool passable = true; //可通過地塊

    public bool isDeployed = false; //已被占用部署地塊
    public bool isAbleToDeploy = false; //是起始允許部署地塊

    public bool isPlayerAllowMoveSpace; //是玩家可以站在地地方

    public Troop TroopsOnMe;

    public void ApplyPerform(string ID)
    {
        //make sr sprite eul to the current ID
        if (ID == "")
        {
            mySr.sprite = Resources.Load<Sprite>("TerrainSprite/TS_Default");
        }
        else
        {
            mySr.sprite = Resources.Load<Sprite>("TerrainSprite/" + ID);
        }
    }

    private void OnMouseEnter()
    {
        roundManager.onFloatingObject = roundManager.gameManager.chessBoardObjectRefArr[myY, myX];
        roundManager.onFloatingVector = new Vector2(myX, myY);
        if (selecting == true)
        {
            mySr.color = new Color(1, 0, 0, 1f);
        }
        else
        {
            //Debug.Log("OnMouseEnter");
            mySr.color = new Color(1, 1, 1, 0.2f);
        }
    }
    private void OnMouseExit()
    {
        if (selecting == true)
        {
            mySr.color = new Color(1, 0, 0, 1f);
        }
        else
        {
            //Debug.Log("OnMouseEnter");
            mySr.color = new Color(1, 1, 1, 1f);
        }
    }
    private void OnMouseDown()
    {
        switch (roundManager.roundState) {
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
                        roundManager.SelectObjectTroop = gameManager.MyTroop.GetComponent<Troop>();
                        roundManager.UpdateOnSelectChessAllowMoveVector();
                    }
                }
                else 
                {
                    if (isPlayerAllowMoveSpace)
                    {
                        Debug.Log("呼叫結束");
                        //TODO: 如果我身上有Troop 代表對Troop進行攻擊
                        if (TroopsOnMe == null)
                        {
                            //TODO 如果選中目標是玩家棋子 如果我是可移動地塊 將玩家棋子移動到我身上(改變其XY)
                            roundManager.SelectObjectTroop.myNowX = myX;
                            roundManager.SelectObjectTroop.myNowY = myY;
                        }
                        else
                        {
                            //若已有棋子，對其造成傷害
                            TroopsOnMe.hp -= gameManager.MyTroop.GetComponent<Troop>().myChessData.AttackStr;
                            //若攻擊未殺死目標，則留在前一格
                            if (TroopsOnMe.hp <= 0)
                            {
                                //被殺死
                                roundManager.SelectObjectTroop.myNowX = myX;
                                roundManager.SelectObjectTroop.myNowY = myY;
                                TroopsOnMe.killTroop();
                            }
                            else
                            {
                                //沒被殺死
                                //先不管
                            }
                           
                        }

                        //TODO 並且呼叫RoundMaster回合完成器
                        roundManager.MyRoundEnd();
                    }
                }

                //回傳GameMaster
                break;
        }
    }
    bool _colorClog;
    void Update()
    {
        //TODO 如果選中目標是玩家棋子 如果我是可移動地塊 標記藍色
        isPlayerAllowMoveSpace = roundManager.IsMeSelectableUnit(new Vector2(myX,myY));
        if (isPlayerAllowMoveSpace)
        {
            mySr.color = new Color(0, 0, 1, 1f);
            _colorClog = true;
        }
        else
        {
            if (_colorClog)
            {
                _colorClog = false;mySr.color = new Color(1, 1, 1, 1);
            }
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
