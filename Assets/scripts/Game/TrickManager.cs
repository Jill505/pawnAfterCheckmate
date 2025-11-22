using UnityEngine;
using UnityEngine.UI;

public class TrickManager : MonoBehaviour
{
    [Header("Manager Refs")]
    public GameManager gameManager;
    public RoundManager roundManager;

    public TrickType myTrickType;
    public SO_Trick trickSOFile;

    public int maxHoldTrick;
    public float myNowEnergy;

    [Header("UI system")]
    public Text trickNameShowCase;
    public Button trickButton;
    public Image trickButtonImage;

    void Start()
    {
        LoadTrick();
    }
    void Update()
    {
        if ( roundManager.roundState == RoundState.MyRound &&  maxHoldTrick > 0)
        {
            trickButton.interactable = true;
        }
    }

    public void LoadTrick()
    {
        myTrickType = SaveSystem.SF.holdingTrickType;

        string trickPath = "TrickSO/";
        switch (myTrickType)
        {
            case TrickType.testTrick:
                trickSOFile = Resources.Load<SO_Trick>(trickPath + "testTrick_SO");
                break;
        }

        //Apply
        if (trickSOFile != null)
        {
            trickNameShowCase.text = trickSOFile.trickName;
            trickButtonImage.sprite = trickSOFile.mySprite;
        }
        else
        {
            Debug.LogError("戲法SO資料不存在");
        }
    }

    public void GainEnergy(float energyGet)
    {
        myNowEnergy += energyGet;
    }

    public void UseTrick()
    {
        if (maxHoldTrick <= 0)
        {
            Debug.Log("無法觸發戲法 - 沒有充能");
            return;
        }

        if (trickSOFile == null)
        {
            Debug.Log("戲法SO資料不存在");
            Debug.Break();
            return;
        }

        switch (myTrickType)
        {
            case TrickType.testTrick:
                DoTrick_TestTrick();
                break;

            default:
                DoTrick_TestTrick();        
                break;
        }

        maxHoldTrick -= 1;
    }

    public void DoTrick_TestTrick()
    {
        Debug.Log("use test trick from test system.");
    }


}
