using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLogScreenManager : MonoBehaviour
{
    [Header("GameObject Refs")]
    public GameObject SettingCanvas;
    public GameObject CreditsCanvas;
    public SoundManager soundManager;

    public AudioClip testClip;

    [Header("UI Refs")]
    public Text DiffTextShowcase;

    public Text MusicVolumeTextShowcase;
    public Text SFXVolumeTextShowcase;

    [Header("URL")]
    public const string websiteUrl = "https://jill505.github.io/PawnAfterSlumber/";

    public void Awake()
    {
        soundManager = FindAnyObjectByType<SoundManager>(); 
    }
    private void Start()
    {
        soundManager.PlayBGM("lobby_demo_1");
    }
    private void Update()
    {
        LobbyUIContextShowcase();
    }
    
    public void StartGameButton()
    {
        StartCoroutine(StartGameButtonCoroutine());
    }
    public IEnumerator StartGameButtonCoroutine()
    {
        yield return null;
        SceneManager.LoadScene(1);
    }

    public void LeaveGame()
    {
        Application.Quit();
    }

    public void OpenSettingCanvas()
    {
        SettingCanvas.SetActive(true);
    }
    public void CloseSettingCanvas()
    {
        SettingCanvas.SetActive(false);
    }

    public void DoDiffSwitch()
    {
        if (SaveSystem.SF.difficulty == 0)
        {
            SaveSystem.SF.difficulty = 1;
        }
        else
        {
            SaveSystem.SF.difficulty = 0;
        }


        SaveSystem.SaveSF();
    }

    public void LobbyUIContextShowcase()
    {
        DiffButtonInformationSync();
        SoundVolumeInformationSync();
    }
    public void DiffButtonInformationSync()
    {
        if (SaveSystem.SF.difficulty == 0)
        {
            DiffTextShowcase.text = "´¶³q¼Ò¦¡";
        }
        else
        {
            DiffTextShowcase.text = "¹ÚÆL¼Ò¦¡";
        }
    }

    public void SoundVolumeInformationSync()
    {
        MusicVolumeTextShowcase.text = (int)(SaveSystem.SF.BgmVolume * 100) + "%";
        SFXVolumeTextShowcase.text = (int)(SaveSystem.SF.SFXVolume* 100) + "%";
    }
    
    public void GameMusicVolumeSetting(float rate)
    {
        SaveSystem.SF.BgmVolume += rate;

        if (SaveSystem.SF.BgmVolume > 1)
        {
            SaveSystem.SF.BgmVolume = 1;
        }
        else
        {
            if (SaveSystem.SF.BgmVolume < 0)
            {
                SaveSystem.SF.BgmVolume = 0;
            }
        }

        GameObject obj = new GameObject();
        AudioSource AS = obj.AddComponent<AudioSource>();
        AS.clip = testClip;
        AS.volume = SaveSystem.SF.BgmVolume;
        AS.Play();
        Destroy(obj, testClip.length);

        SaveSystem.SaveSF();
    }

    public void GameSFXVolumeSetting(float rate)
    {
        SaveSystem.SF.SFXVolume += rate;

        if (SaveSystem.SF.SFXVolume > 1)
        {
            SaveSystem.SF.SFXVolume = 1;
        }
        else
        {
            if (SaveSystem.SF.SFXVolume < 0)
            {
                SaveSystem.SF.SFXVolume = 0;
            }
        }

        GameObject obj = new GameObject();
        AudioSource AS = obj.AddComponent<AudioSource>();
        AS.clip = testClip;
        AS.volume = SaveSystem.SF.SFXVolume;
        AS.Play();
        Destroy(obj, testClip.length);

        SaveSystem.SaveSF();
    }

    public void LinkToWebsite()
    {
        Application.OpenURL(websiteUrl);
    }

    public void OpenCreditsCanvas()
    {
        CreditsCanvas.SetActive(true);
    }
    public void CloseCreditCanvas()
    {
        CreditsCanvas.SetActive(false);
    }
}
