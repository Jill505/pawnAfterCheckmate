using JetBrains.Annotations;
using UnityEngine;

public class unit : MonoBehaviour
{
    public testRoundMaster roundMaster;
    public SpriteRenderer mySr;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
        mySr.color = new Color(1,1,1,0.2f);
    }
    private void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
        mySr.color = new Color(1,1,1,1);
    }
    private void OnMouseDown()
    {
        //�^��GameMaster
    }
}
