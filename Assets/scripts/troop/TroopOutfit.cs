using UnityEngine;

public class TroopOutfit : MonoBehaviour
{
    public Troop myTroop;

    public GameObject upperShieldImage;
    public GameObject lowerShieldImage;
    public GameObject leftShieldImage;
    public GameObject rightShieldImage;

    public GameObject goldenTargetGlow;
    public GameObject goldenTargetRing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        RageOutfit();
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
        goldenTargetGlow.SetActive(myTroop.myChessData.isGoldenTarget);
        goldenTargetRing.SetActive(myTroop.myChessData.isGoldenTarget);
    }

    public void RageOutfit()
    {

    }
}
