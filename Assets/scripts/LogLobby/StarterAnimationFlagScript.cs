using UnityEngine;

public class StarterAnimationFlagScript : MonoBehaviour
{
    public bool soundEffectFlag;
    public bool clog;

    public void Update()
    {
        if (clog == false && soundEffectFlag == true )
        {
            clog = true;
            FindFirstObjectByType<SoundManager>().PlaySFX("bell_lose");
        }
    }
}
