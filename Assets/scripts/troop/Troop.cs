using TMPro;
using UnityEngine;

public class Troop : MonoBehaviour
{
    public SO_Chess myChessData;

    public int myNowX = 0;
    public int myNowY = 0;

    public int hp = 1;

    public SpriteRenderer mySr;

    public gear holdingGear = gear.noGear;

    public void LoadSOData()
    {
        hp = myChessData.hp;

        mySr.sprite = myChessData.skin;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
