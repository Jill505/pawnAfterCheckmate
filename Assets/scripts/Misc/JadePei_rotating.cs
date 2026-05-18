using UnityEngine;

public class JadePei_rotating : MonoBehaviour
{
    float angel = 180;
    public float rotateSpeed = 12f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, angel, 0);
        angel += Time.deltaTime * rotateSpeed;
     }
}
