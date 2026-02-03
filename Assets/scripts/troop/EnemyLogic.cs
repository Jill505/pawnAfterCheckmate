using UnityEngine;
public class EnemyLogic : MonoBehaviour
{
    public Troop myTroop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTroop  = gameObject.GetComponent<Troop>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
