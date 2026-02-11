using System.Collections.Generic;
using TMPro;
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
    //public Text trickNameShowCase;
    public TextMeshProUGUI trickNameShowcase_TMP;
    public Button trickButton;
    public Image trickButtonImage;

    public Image processImage;
    public float targetAmount_forShow;
    //public Text trickHoldTrickNumTextShowcase;
    public TextMeshProUGUI trickHoldNumTMP;
    //public Text trickHoldTrickNumTextShowcase_Bg;

    [Header("Spawn Troop Variables")]
    public SO_Chess TroopSpawnSwap_SO;
    public List<Vector2> AllowSpawnSpace;

    [Header("Rely Refs")]
    public SO_Chess StrawMan_SO;

    public void Start()
    {
        LoadTrick();
    }
    void Update()
    {
        TrickEnergyLogic();
        UILogic();

        if (Input.GetKeyDown(KeyCode.R))
        {
            UseTrick();
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (roundManager.isCastingPlacementTrick)
            {
                CancelCastingPlacementTrick();
            }
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
            case TrickType.StrawMan:
                trickSOFile = Resources.Load<SO_Trick>(trickPath + "StrawMan_SO");
                break;
        }

        //Apply
        if (trickSOFile != null)
        {
            //trickNameShowCase.text = trickSOFile.trickName;
            trickNameShowcase_TMP.text = trickSOFile.name;
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
        roundManager.resetUnitSelectState();

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

            case TrickType.StrawMan:
                DoTrick_StrawMan();
                break;  

            default:
                DoTrick_TestTrick();        
                break;
        }

        //myNowHoldTrickNum -= 1;
    }


    public void DoTrick_StrawMan()
    {
        Debug.Log("Do Straw man trick.");

        TroopSpawnSwap_SO = StrawMan_SO;

        Vector2 refPlayerPos = new Vector2(gameManager.PlayerTroop.myNowX, gameManager.PlayerTroop.myNowY);
        List<Vector2> relativelyVec = new List<Vector2>();

        /*
        foreach (Vector2 vec2 in gameManager.GetEmptyUnitList())
        {
            gameManager.GetUnitAt((int)vec2.x, (int)vec2.y).isPlaceableTarget = true;
        }*/

        if (SaveSystem.SF.strawmanLevel <= 0 || SaveSystem.SF.strawmanLevel >=0)
        {
            relativelyVec.Add(new Vector2(1, 0));
            relativelyVec.Add(new Vector2(-1, 0));
            relativelyVec.Add(new Vector2(0, 1));
            relativelyVec.Add(new Vector2(0, -1));
        }

        if (SaveSystem.SF.strawmanLevel >= 1)
        {
            relativelyVec.Add(new Vector2(2, 0));
            relativelyVec.Add(new Vector2(-2, 0));
            relativelyVec.Add(new Vector2(0, 2));
            relativelyVec.Add(new Vector2(0, -2));
        }


        //Register Spawnable unit
        foreach (Vector2 vec in relativelyVec)
        {
            Vector2 spawnVec = refPlayerPos + vec;
            //座標合法性檢查
            if (spawnVec.x <0 || spawnVec.y < 0 || spawnVec.x >= gameManager.levelData.gridSizeX || spawnVec.y >= gameManager.levelData.gridSizeY)
            {
                continue;
            }
            //座標上單位檢查
            if (gameManager.GetUnitAt((int)spawnVec.x, (int)spawnVec.y).TroopsOnMe != null)
            {
                continue;
            }

            gameManager.GetUnitAt((int)spawnVec.x, (int)spawnVec.y).isPlaceableTarget = true;
        }

        //UpdateTargetPlace(); //高配版 之後優化 

        roundManager.isCastingPlacementTrick = true;
        roundManager.isCastingTrick_StrawMan = true;
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
            //trickHoldTrickNumTextShowcase.text = myNowHoldTrickNum + "";
            trickHoldNumTMP.text = myNowHoldTrickNum + "";
            //trickHoldTrickNumTextShowcase_Bg.text = myNowHoldTrickNum + "";
        }
        else
        {
            targetAmount_forShow = myNowEnergy / trickSOFile.trickRequireEnergy;
            processImage.fillAmount = Mathf.Lerp(processImage.fillAmount, targetAmount_forShow, 0.2f);
            //trickHoldTrickNumTextShowcase.text = myNowHoldTrickNum + "";
            //trickHoldTrickNumTextShowcase_Bg.text = myNowHoldTrickNum + "";
            trickHoldNumTMP.text = myNowHoldTrickNum + "";

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

    public void CancelCastingPlacementTrick()
    {
        roundManager.isCastingPlacementTrick = false;
        roundManager.isCastingTrick_StrawMan = false;
        ResetTargetPlace();
    }

    public void UpdateTargetPlace(List<Vector2> tarVecList)
    {
        foreach (Vector2 vec in tarVecList)
        {
            gameManager.chessBoardObjectRefArr[(int)vec.y, (int)vec.x].GetComponent<unit>().isPlaceableTarget = true;
        }
    }
    public void ResetTargetPlace()
    {
        foreach (GameObject tObj in gameManager.chessBoardObjectRefArr)
        {
            tObj.GetComponent<unit>().isPlaceableTarget = false;
        }
    }
}
