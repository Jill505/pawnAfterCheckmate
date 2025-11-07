using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SoundManager : MonoBehaviour
{
    public List<AK_Audio> Music;

    public List<AK_Audio> SFXs;

    static public float BgmVolume = 1f;
    static public float SfxVolume = 1f;

    public GameObject AudioObject;
    
    void Start()
    {
        
    }

    
    void Update()
    {

    }

    public AK_Audio PlayBGM(string BgmName)
    {
        AudioClip sAC = Resources.Load<AudioClip>("Music/" + BgmName);

        if (sAC == null)
        {
            Debug.LogWarning("AK Warning - 音樂未找到檔案 拒絕播放");
            return null;
        }

        GameObject obj = Instantiate(AudioObject);
        AK_Audio AKA =  obj.GetComponent<AK_Audio>();
        AKA.myAudioSource.clip = sAC;
        AKA.myST = SoundType.Bgm;
        AKA.myAudioClip = sAC;

        AKA.PlaySound();

        return AKA;
    }

    public void PlaySFX(string SFXName)
    {
        AudioClip sAC = Resources.Load<AudioClip>("SFX/" + SFXName);

        if (sAC == null)
        {
            Debug.LogWarning("AK Warning - 音效未找到檔案 拒絕播放");
            //return null;
        }

        GameObject obj = Instantiate(AudioObject);
        AK_Audio AKA = obj.GetComponent<AK_Audio>();
        AKA.myAudioClip = sAC;
        AKA.myAudioSource.clip = sAC;
        AKA.myST = SoundType.Sfx;

        AKA.PlaySound();

        //return AKA;
    }

    public AK_Audio PlaySFX(string SFXName, bool isDontDestroyOnLoad)
    {
        AudioClip sAC = Resources.Load<AudioClip>("SFX/" + SFXName);

        if (sAC == null)
        {
            Debug.LogWarning("AK Warning - 音效未找到檔案 拒絕播放");
            return null;
        }


        GameObject obj = Instantiate(AudioObject);
        AK_Audio AKA = obj.GetComponent<AK_Audio>();
        AKA.myAudioClip = sAC;
        AKA.myAudioSource.clip = sAC;
        AKA.myST = SoundType.Sfx;

        AKA.PlaySound();


        if (isDontDestroyOnLoad)
        {
            AKA.dontDestroyOnLoadSwitch = true;
        }

        return AKA;
    }
}


public enum SoundType
{
    Sfx,
    Bgm
}