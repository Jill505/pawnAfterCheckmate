using UnityEngine;
using UnityEngine.Rendering;

public class unit : MonoBehaviour
{
    public GameManager gameManager;
    public RoundManager roundManager;
    public SpriteRenderer mySr;

    public int myX;
    public int myY;

    public bool selecting = false;
    public bool passable = true;

    public bool isDeployed = false; //已被占用部署地塊
    public bool isAbleToDeploy = false; //是起始允許部署地塊

    public void ApplyPerform(string ID)
    {
        //make sr sprite eul to the current ID
        if (ID == "")
        {
            mySr.sprite = Resources.Load<Sprite>("TerrainSprite/TS_Default");
        }
        else
        {
            mySr.sprite = Resources.Load<Sprite>(ID);
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
        //回傳GameMaster
        roundManager.selectingVector = new Vector2(myX,myY);
        roundManager.resetUnitSelectState();
        roundManager.SelectObject = roundManager.gameManager.chessBoardObjectRefArr[myY, myX];
    }
}
