using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameLogScreenManager : MonoBehaviour
{
    [Header("GameObject Refs")]
    public GameObject SettingCanvas;
    public GameObject CreditsCanvas;
    public SoundManager soundManager;
    public GameLogScreenLoad gameLogScreenLoad;

    public Animator logScreenAnimator;

    public AudioClip testClip;

    [Header("UI Refs")]
    public Text DiffTextShowcase;

    public Text MusicVolumeTextShowcase;
    public Text SFXVolumeTextShowcase;

    public TMP_Dropdown languageSelectionDropdown;

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
        soundManager.PlaySFX("bell_lose");
        logScreenAnimator.SetTrigger("LoadGame");

        yield return new WaitForSeconds(1.62f);

        SaveSystem.LoadSF();

        if (SaveSystem.SF.storyRead[0] == false)
        {
            SaveSystem.SF.storyRead[0] = true;
            SaveSystem.SaveSF();

            SceneManager.LoadScene(2);

        }
        else
        {
            SceneManager.LoadScene(1);
        }
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
        int diff = SaveSystem.SF.difficulty;

        diff += 1;
        if (diff > 2)
        {
            diff = 0;
        }

        SaveSystem.SF.difficulty = diff;

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
            DiffTextShowcase.text = "簡單模式";
        }
        else if (SaveSystem.SF.difficulty == 1)
        {
            DiffTextShowcase.text = "普通模式";
        }
        else
        {
            DiffTextShowcase.text = "夢魘模式";
        }
    }

    public void SoundVolumeInformationSync()
    {
        MusicVolumeTextShowcase.text = (int)(SaveSystem.SF.BgmVolume * 100) + "%";
        SFXVolumeTextShowcase.text = (int)(SaveSystem.SF.SFXVolume* 100) + "%";
    }
    
    public void GameLanguageSettingOnChange()
    {
        switch(languageSelectionDropdown.value)
        {
            case 0://繁體中文
                SaveSystem.SF.SelectingLanguage = AK_Language.zh;
                SaveSystem.SaveSF();
                break;
                
            case 1://簡體中文
                SaveSystem.SF.SelectingLanguage = AK_Language.cn;
                SaveSystem.SaveSF();
                break;

            case 2://英文
                SaveSystem.SF.SelectingLanguage = AK_Language.en;
                SaveSystem.SaveSF();
                break;

            case 3://日文
                SaveSystem.SF.SelectingLanguage = AK_Language.jp;
                SaveSystem.SaveSF();
                break;
        }
        Debug.Log("更改");
        gameLogScreenLoad.LoadLogScreenLan(); ;
        gameLogScreenLoad.LoadLanLogScreen();
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
