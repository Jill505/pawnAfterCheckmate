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
    }
}
