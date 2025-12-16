using UnityEngine;

public class TSA_KarenBorn : MonoBehaviour
{
    public Troop myTroop;
    public SLS_Karen SLSK;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SLSK = FindAnyObjectByType<SLS_Karen>();
        myTroop.OnDieAction += InjectOnDie;
    }
    public void InjectOnDie()
    {
        SLSK.OnKarenTroopDie();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
