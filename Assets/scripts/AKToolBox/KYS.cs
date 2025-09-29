using UnityEngine;

public class KYS : MonoBehaviour
{
    public float kysT = 10f;

    void Start()
    {
        KillYourself(kysT);
    }
    public void KillYourself(float t)
    {
        BeforeDeath();
        Destroy(gameObject,t); 
    }

    public virtual void BeforeDeath()
    {

    }
}
