using System;
using UnityEngine;
using UnityEngine.UI;

public class TroopOutfit : MonoBehaviour
{
    [Header("Manager refs")]
    public RoundManager roundManager;

    public Troop myTroop;

    public GameObject upperShieldImage;
    public GameObject lowerShieldImage;
    public GameObject leftShieldImage;
    public GameObject rightShieldImage;

    public GameObject goldenTargetGlow;
    public GameObject goldenTargetRing;

    public GameObject EnergyHighVFXObject;
    public Animator EnergyHighVFXAnimator;
    public bool energyHighStarterVFXCLog = false;

    public Text hitShieldCountDownShowcase;
    public GameObject hitShieldImage;
    public GameObject hitShieldBrokenImage;

    public Action TroopOutfitUpdate = () => { };

    public Animator myAnimator;

    public GameObject UntouchableHint;

    [Header("Specialize Animation Object")]
    public GameObject BombExplodeAnimation;

    public GameObject DeathParticle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roundManager = FindAnyObjectByType<RoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyOutfit();
    }

    public void ApplyOutfit()
    {
        ShieldOutfit();
        GoldenTargetOutfit();
        hitShieldOutfit();
        ApplyEnergyHighVFX();

        TroopOutfitUpdate();
    }

    public void ShieldOutfit()
    {
        upperShieldImage.SetActive(myTroop.hasUpperShield);
        lowerShieldImage.SetActive(myTroop.hasLowerShield);
        leftShieldImage.SetActive(myTroop.hasLeftShield);
        rightShieldImage.SetActive(myTroop.hasRightShield);
    }

    public void GoldenTargetOutfit()
    {
        goldenTargetRing.SetActive(myTroop.isGoldenTarget);
    }

    public void hitShieldOutfit()
    {
        int requireHit = 0;
        bool check = false;

        for (int i = 0; i < myTroop.myAbilities.Length; i++)
        {
            if (myTroop.myAbilities[i] == ability.HitShield_P)
            {
                requireHit = int.Parse(myTroop.myAbilitiesParameter[i]);
                check = true;
                break;
            }
        }


        if (check)
        {
            //apply
            if (roundManager.playerHitCombo - requireHit < 0)
            {
                //Show
                hitShieldImage.SetActive(true);
                hitShieldCountDownShowcase.gameObject.SetActive(true);
                hitShieldCountDownShowcase.text = "" + (requireHit - roundManager.playerHitCombo);
                hitShieldBrokenImage.SetActive(false);
            }
            else
            {
                //Close
                hitShieldImage.SetActive(false);
                hitShieldCountDownShowcase.gameObject.SetActive(false);
                hitShieldBrokenImage.SetActive(true);
            }
        }
        else
        {
            //neverShow;
        }
    }
    public void Do_BombExplodeAnimation()
    {
        Instantiate(BombExplodeAnimation, transform.position, transform.rotation);
    }

    public void DieVFX(bool gainEnergy = true)
    {
        //perse a rotaion object that object kill it.
        Quaternion burstRotation = new Quaternion();

        if (myTroop.TargetThatMurderMe != null)
        {
            Vector3 dir = transform.position - myTroop.TargetThatMurderMe.transform.position;
            burstRotation = Quaternion.LookRotation(dir.normalized);
        }

        if (gainEnergy == true)
        {
            Instantiate(DeathParticle, transform.position + new Vector3(0, 0, -1), burstRotation);
        }
    }

    public void TriggerUntouchableHint()
    {
        UntouchableHint.SetActive (true);
    }

    public void CleanUntouchableHint()
    {
        UntouchableHint.SetActive(false);
    }

    public void ApplyEnergyHighVFX()
    {
        if (myTroop.energyHigh)
        {
            EnergyHighVFXObject.SetActive(true);
            
            if (!energyHighStarterVFXCLog)
            {
                energyHighStarterVFXCLog = true;
                EnergyHighVFXAnimator.Play("EnergyHighVFX_Active");
            }
        }
        else
        {
            EnergyHighVFXObject.SetActive(false);
            energyHighStarterVFXCLog = false;
        }
    }
}
