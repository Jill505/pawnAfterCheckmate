using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AK_Audio : MonoBehaviour
{
    public SoundType myST;
    public float myCurrentVolume;
    public float myNormalVolume = 1;
    public float audioClipPlayTime = 10f;

    public bool isSyncPitch;

    public AudioSource myAudioSource;
    public AudioClip myAudioClip;

    public bool autoDel = false;
    public bool dontDestroyOnLoadSwitch = false;

    public Coroutine nowFaceFunc;

    private void Awake()
    {
        if (myAudioClip != null) audioClipPlayTime = myAudioClip.length;
    }
    private void Start()
    {
        myCurrentVolume = myNormalVolume;

        if (dontDestroyOnLoadSwitch)
        {
            DontDestroyOnLoad(gameObject);
        }
        //myAudioSource.clip = myAudioClip;
        switch (myST)
        {
            case SoundType.Sfx:
                autoDel = true;

                DontDestroyOnLoad(this);
                break;

            case SoundType.Bgm:
                myAudioSource.loop = true;
                isSyncPitch = true;
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
                myNormalVolume = SoundManager.SfxVolume;
                break;

            case SoundType.Bgm:
                myNormalVolume = SoundManager.BgmVolume;
                break;
        }

        if (isSyncPitch)
        {
            myAudioSource.pitch = Time.timeScale;
        }

        myAudioSource.volume = myCurrentVolume;
    }

    public void KysSound()
    {

    }

    public void FadeIn(float time)
    {
        Debug.Log(gameObject.name + "fade");
        if (nowFaceFunc != null)
        {
            //return;
        }

        nowFaceFunc = StartCoroutine(FadeInCoroutine(time));
    }
    public IEnumerator FadeInCoroutine(float time)
    {
        Debug.Log("Trigger Fade Coroutine");
        myCurrentVolume = 0;

        while (myCurrentVolume < myNormalVolume)
        {
            Debug.Log("adding ");
            myCurrentVolume += (Time.deltaTime / time);
            yield return null;
        }
    }

    public void FadeOut(float time)
    {
        if (nowFaceFunc != null)
        {
            //return;
        }
        nowFaceFunc = StartCoroutine(FadeOutCoroutine(time));
    }
    public IEnumerator FadeOutCoroutine(float time)
    {
        while (myCurrentVolume > 0)
        {
            myCurrentVolume -= (Time.deltaTime / time);
            yield return null;
        }
    }
}