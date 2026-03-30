using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TrickManager : MonoBehaviour
{
    [Header("Manager Refs")]
    public GameManager gameManager;
    public RoundManager roundManager;
    public HintManager hintManager;
    public SoundManager soundManager;

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
        hintManager = FindFirstObjectByType<HintManager>();
        soundManager = FindFirstObjectByType<SoundManager>();
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
                trickSOFile.LoadLangData();
                break;
            case TrickType.testTrick:
                trickSOFile = Resources.Load<SO_Trick>(trickPath + "testTrick_SO");
                trickSOFile.LoadLangData();
                break;
            case TrickType.StrawMan:
                trickSOFile = Resources.Load<SO_Trick>(trickPath + "StrawMan_SO");
                trickSOFile.LoadLangData();
                break;
            case TrickType.SelfEnergyHigh:
                trickSOFile = Resources.Load<SO_Trick>(trickPath + "SelfEnergyHigh_SO");
                trickSOFile.LoadLangData();
                break;  
        }

        //Apply
        if (trickSOFile != null)
        {
            //trickNameShowCase.text = trickSOFile.trickName;
            trickNameShowcase_TMP.text = trickSOFile.trickName;
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

        int additionalKillEnergy = 0;
        if (SaveSystem.SF.talentTreeUnlock[1])
        {
            additionalKillEnergy += 10;
        }
        if (SaveSystem.SF.talentTreeUnlock[8])
        {
            additionalKillEnergy += 5;
        }
        if (SaveSystem.SF.talentTreeUnlock[11])
        {
            additionalKillEnergy += 5;
        }


        GainEnergy(energyGetWhenKill[killCT] + additionalKillEnergy);
    }
    public void GainEnergy(float energyGet)
    {
        if (isMaxContain == true)
        {
            return; 
        }

        hintManager.SpawnHintWordPrefab("Gain Energy- " + energyGet);

        myNowEnergy += energyGet;

        if (myNowEnergy >= trickSOFile.trickRequireEnergy)
        {
            myNowEnergy -= trickSOFile.trickRequireEnergy;

            if (myNowHoldTrickNum < trickSOFile.maxTrickAmount)
            {
                myNowHoldTrickNum += 1;
                soundManager.PlaySFX("skill_fullfill(no_tss)");
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

        int HorLen = 0;
        int VerLen = 0;
        int diaLen = 0;

        HorLen += 1;
        VerLen += 1;
        diaLen += 1;

        if (SaveSystem.SF.strawmanLevel >= 0)
        {
            HorLen += 1;
            VerLen += 1;
        }

        if (SaveSystem.SF.talentTreeUnlock[3])
        {
            HorLen += 1;
            VerLen += 1;
        }

        if (SaveSystem.SF.talentTreeUnlock[7])
        {
            diaLen += 1;
            HorLen += 1;
            VerLen += 1;
        }

        if (SaveSystem.SF.talentTreeUnlock[19])
        {
            diaLen += 1;
            HorLen += 1;
            VerLen += 1;
        }

        for (int i = 1; i < HorLen + 1; i++)
        {
            relativelyVec.Add(new Vector2(i, 0));
            relativelyVec.Add(new Vector2(-i, 0));
        }

        for (int i = 1; i < VerLen +1; i++)
        {
            relativelyVec.Add(new Vector2(0, i));
            relativelyVec.Add(new Vector2(0, -i));
        }

        for (int i = 1; i < diaLen +1; i++)
        {
            relativelyVec.Add(new Vector2(i, i));
            relativelyVec.Add(new Vector2(i, -i));
            relativelyVec.Add(new Vector2(-i, i));
            relativelyVec.Add(new Vector2(-i, -i));
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
            if (gameManager.GetUnitAt((int)spawnVec.x, (int)spawnVec.y).StructureOnMe != null)
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
