using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnergyHighSpot : MonoBehaviour
{
    public Structure myStructure;
    public RoundManager roundManager;

    public Light2D swapLightVFX;

    public int SpawnCoolDown =2;
    public int CurrentCoolDown;

    public bool isHaveEnergy = true;

    public void Awake()
    {
        roundManager = FindFirstObjectByType<RoundManager>();
        swapLightVFX = gameObject.AddComponent<Light2D>();
    }

    public void Start()
    {
        //Register Each Round pass function into the round finish actionj
        roundManager.Action_OnRoundEnd += EachRoundPass;
        myStructure.OnTroopStepOnMe += GiveTroopEnergyHigh;

        StartCoroutine(ShineBlueCoroutine());
    }

    public void FixedUpdate()
    {
        if (isHaveEnergy)
        {
            swapLightVFX.enabled = true;
        }
        else
        {
            swapLightVFX.enabled = false;
        }
    }

    public void EachRoundPass()
    {
        CurrentCoolDown -=1 ;
        if (CurrentCoolDown <= 0)
        {
            CurrentCoolDown = 0 ;
            isHaveEnergy = true ;
        }
    }

    public void GiveTroopEnergyHigh(Troop t)
    {
        if (isHaveEnergy)
        {
            t.energyHigh = true;
            isHaveEnergy = false;
            CurrentCoolDown = SpawnCoolDown;
        }
    }

    public void OnDestroy()
    {
        //remove function form round fin action
        roundManager.Action_OnRoundEnd -= EachRoundPass;
    }

    public IEnumerator ShineBlueCoroutine()
    {
        swapLightVFX.color = Color.cyan;

        bool upDownVar = true;
        while (true)
        {
            if (upDownVar)
            {
                swapLightVFX.intensity += Time.deltaTime * 30f;
            }
            else
            {
                swapLightVFX.intensity -= Time.deltaTime * 30f;
            }

            if (swapLightVFX.intensity > 20)
            {
                upDownVar = false;
            }
            if (swapLightVFX.intensity < 0)
            {
                upDownVar = true;
            }
            yield return null;
        }
    }
}
