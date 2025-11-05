using Unity.VisualScripting;
using UnityEngine;

public class AK_Audio : MonoBehaviour
{
    public SoundType myST;
    public float myVolume;
    public float audioClipPlayTime = 10f;

    public AudioSource myAudioSource;
    public AudioClip myAudioClip;

    public bool autoDel = false;

    private void Awake()
    {
        if (myAudioClip != null) audioClipPlayTime = myAudioClip.length;
    }
    private void Start()
    {
        //myAudioSource.clip = myAudioClip;
        switch (myST)
        {
            case SoundType.Sfx:
                autoDel = true;

                DontDestroyOnLoad(this);
                break;

            case SoundType.Bgm:
                myAudioSource.loop = true;
                break;
        }

        if (autoDel)
        {
            Invoke("KysSound", audioClipPlayTime);
        }

        myAudioSource.Play();
    }

    public void PlaySound()
    {
        myAudioSource.Play();
    }

    void Update()
    {
        switch (myST) //可以統一註冊到聲音改變時以優化系統 現在我先寫一個暫時的統一音量
        {
            case SoundType.Sfx:
                myVolume = SoundManager.SfxVolume;
                break;

            case SoundType.Bgm:
                myVolume = SoundManager.BgmVolume;
                break;
        }
    }

    public void KysSound()
    {

    }
}