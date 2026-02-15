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

    public Text hitShieldCountDownShowcase;
    public GameObject hitShieldImage;

    public Action TroopOutfitUpdate = () => { };

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
        goldenTargetRing.SetActive(myTroop.myChessData.isGoldenTarget);
    }

    public void hitShieldOutfit()
    {
        int requireHit = 0;
        bool check = false;

        foreach (ability abi in myTroop.myAbilities)
        {
            if (abi == ability.HitShield_1)
            {
                requireHit = 1;
                check = true;
                break;
            }
            else if (abi == ability.HitShield_2)
            {
                requireHit= 2;
                check = true;
                break;
            }
            else if (abi == ability.HitShield_3)
            {
                requireHit = 3;
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
                hitShieldImage.SetActive (true);
                hitShieldCountDownShowcase.gameObject.SetActive (true);
                hitShieldCountDownShowcase.text = "" +(requireHit - roundManager.playerHitCombo);
            }
            else
            {
                //Close
                hitShieldImage.SetActive(false);
                hitShieldCountDownShowcase.gameObject.SetActive(false);
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

    public void DieVFX()
    {
        //perse a rotaion object that object kill it.
        Quaternion burstRotation = new Quaternion();

        if (myTroop.TargetThatMurderMe != null)
        {
            Vector3 dir = transform.position - myTroop.TargetThatMurderMe.transform.position;
            burstRotation = Quaternion.LookRotation(dir.normalized);
        }

        Instantiate(DeathParticle, transform.position + new Vector3(0,0,-1), burstRotation);
    }

    public void SpinSpawnAnimate()
    {
        
    }
}
