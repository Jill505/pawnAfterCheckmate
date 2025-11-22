using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class TrickManager : MonoBehaviour
{
    [Header("Manager Refs")]
    public GameManager gameManager;
    public RoundManager roundManager;

    public TrickType myTrickType;
    public SO_Trick trickSOFile;

    public int myNowHoldTrickNum;
    public float myNowEnergy;
    public bool isMaxContain;

    [Header("Player Energy Config")]
    public float[] energyGetWhenKill;

    [Header("UI system")]
    public Text trickNameShowCase;
    public Button trickButton;
    public Image trickButtonImage;

    public Image processImage;
    public float targetAmount_forShow;
    public Text trickHoldTrickNumTextShowcase;
    public Text trickHoldTrickNumTextShowcase_Bg;
    void Start()
    {
        LoadTrick();
    }
    void Update()
    {
        TrickEnergyLogic();
        UILogic();

        if (Input.GetKeyDown(KeyCode.I))
        {
            GainEnergy(20);
        }
    }

    public void LoadTrick()
    {
        myTrickType = SaveSystem.SF.holdingTrickType;

        string trickPath = "TrickSO/";
        switch (myTrickType)
        {
            case TrickType.noTrick:
                trickSOFile = Resources.Load<SO_Trick>(trickPath + "NoTrick_SO");
                break;
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
    public void GainEnergyFromKill(int killCT)
    {
        if (killCT >= energyGetWhenKill.Length)
        {
            killCT = energyGetWhenKill.Length -1;
        }
        GainEnergy(energyGetWhenKill[killCT]);
    }
    public void GainEnergy(float energyGet)
    {
        if (isMaxContain == true)
        {
            return; 
        }

        myNowEnergy += energyGet;

        if (myNowEnergy >= trickSOFile.trickRequireEnergy)
        {
            myNowEnergy -= trickSOFile.trickRequireEnergy;

            if (myNowHoldTrickNum < trickSOFile.maxTrickAmount)
            {
                myNowHoldTrickNum += 1;
                
            }
        }
    }

    public void UseTrick()
    {
        if (myNowHoldTrickNum <= 0)
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

        myNowHoldTrickNum -= 1;
    }

    public void DoTrick_TestTrick()
    {
        Debug.Log("use test trick from test system.");
    }

    public void UILogic()
    {
        if (roundManager.roundState == RoundState.MyRound && myNowHoldTrickNum > 0)
        {
            trickButton.interactable = true;
        }
        else
        {
            trickButton.interactable = false;
        }

        if (isMaxContain)
        {
            processImage.fillAmount = 1;
            trickHoldTrickNumTextShowcase.text = myNowHoldTrickNum + "";
            trickHoldTrickNumTextShowcase_Bg.text = myNowHoldTrickNum + "";
        }
        else
        {
            targetAmount_forShow = myNowEnergy / trickSOFile.trickRequireEnergy;
            processImage.fillAmount = Mathf.Lerp(processImage.fillAmount, targetAmount_forShow, 0.2f);
            trickHoldTrickNumTextShowcase.text = myNowHoldTrickNum + "";
            trickHoldTrickNumTextShowcase_Bg.text = myNowHoldTrickNum + "";

        }
    }

    public void TrickEnergyLogic()
    {
        if (myNowHoldTrickNum >= trickSOFile.maxTrickAmount)
        {
            isMaxContain = true;
        }
        else
        {
            isMaxContain = false;
        }
    }
}
