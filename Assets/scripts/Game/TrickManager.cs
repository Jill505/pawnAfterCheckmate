using UnityEngine;

public class TrickManager : MonoBehaviour
{
    public SO_Trick myTrick;

    public int maxHoldTrick;
    public float myNowEnergy;

    public void GainEnergy(float energyGet)
    {
        myNowEnergy += energyGet;
    }

    public void UseTrick()
    {
        if (myTrick == null) Debug.Log("戲法不存在"); Debug.Break(); return;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
