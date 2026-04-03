using UnityEngine;

public class TestCore : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            FindFirstObjectByType<TSA_Player>().gameObject.GetComponent<Troop>().leftLife++;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            FindFirstObjectByType<TrickManager>().GainEnergy(50f);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            FindFirstObjectByType<TSA_Player>().myTroop.energyHigh = true;
        }
    }
}
