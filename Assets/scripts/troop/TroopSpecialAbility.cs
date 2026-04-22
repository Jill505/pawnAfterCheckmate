using UnityEngine;

public class TroopSpecialAbility : MonoBehaviour
{
    public Troop myTroop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TSAInit()
    {
        if (myTroop == null)
        {
            //try get
            if (gameObject.TryGetComponent<Troop>(out Troop T))
            {
                myTroop = T;
            }
            else
            {
                Debug.LogError("TSA don't have troop");
            }
        }
    }

    public void EnemyDieStatAdd(int ID)
    {
        SaveSystem.SF.EnemyHistoryKillData[ID] += 1;
        SaveSystem.SaveSF();
    }

    public void DieReport(int ID)
    {
        FindFirstObjectByType<RoundProcessManager>().CheckKillReport(ID);
    }
}
