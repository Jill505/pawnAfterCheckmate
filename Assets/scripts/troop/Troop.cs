using TMPro;
using UnityEngine;

public class Troop : MonoBehaviour
{
    public GameManager gameManager;
    public SO_Chess myChessData;
    public bool isPlayer = false;

    [Range(0, 9)]
    public int myNowX = 0;
    [Range(0, 9)]
    public int myNowY = 0;

    public int hp = 1;

    public SpriteRenderer mySr;

    public gear holdingGear = gear.noGear;


    public void LoadSOData()
    {
        hp = myChessData.hp;

        mySr.sprite = myChessData.skin;
    }

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        LoadSOData();
    }

    void Update()
    {
        myPosSync();
    }
    public void myPosSync()
    {
        if (gameManager.chessBoardObjectRefArr.GetLength(0) > myNowX && gameManager.chessBoardObjectRefArr.GetLength(1) > myNowY && myNowX >=0 && myNowY >=0)
        {
            Vector2 vec = gameManager.chessBoardObjectRefArr[myNowY, myNowX].transform.position;
            transform.position = vec;
        }
    }

    public void killTroop()
    {
        //TODO: 將自己從註冊表中移除
        gameManager.Troops.Remove(gameObject);
        Destroy(gameObject);
    }
}
