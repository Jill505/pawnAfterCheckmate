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

    public Animator LanguageSelectCanvasAnimator;

    public AudioClip testClip;

    [Header("UI Refs")]
    public Text DiffTextShowcase;

    public Text MusicVolumeTextShowcase;
    public Text SFXVolumeTextShowcase;

    public TMP_Dropdown languageSelectionDropdown;

    public TextMeshProUGUI SelectLangTMP;
    public TextMeshProUGUI ConfirmSelectLangTMP;

    public float CamFluence = 0.8f;
    public Transform[] CamPos = new Transform[3];
    public Transform myCamPos;
    public Transform nowTargetCamPos;

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
        SyncCamPos();
    }

    private void FixedUpdate()
    {

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
            StorySceneManager.StaticSO_Story = Resources.Load<SO_Story>("SO/Story/Story_0");

            SaveSystem.SF.storyRead[0] = true;
            SaveSystem.SaveSF();

            SceneManager.LoadScene(3);

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

    public void langSelect()
    {
        int num = (int)SaveSystem.SF.SelectingLanguage;

        num = num + 1 <= 3 ? num + 1 : 0;
        if (num == 1) num = 2;

        switch (num)
        {
            case 0://繁體中文
                SaveSystem.SF.SelectingLanguage = AK_Language.zh;
                SelectLangTMP.text = "繁體中文";
                ConfirmSelectLangTMP.text = "確認";
                SaveSystem.SaveSF();
                break;

            case 1://簡體中文
                SaveSystem.SF.SelectingLanguage = AK_Language.cn;
                SelectLangTMP.text = "简体中文";
                ConfirmSelectLangTMP.text = "确认";
                SaveSystem.SaveSF();
                break;

            case 2://英文
                SaveSystem.SF.SelectingLanguage = AK_Language.en;
                SelectLangTMP.text = "English";
                ConfirmSelectLangTMP.text = "Confirm";
                SaveSystem.SaveSF();
                break;

            case 3://日文
                SaveSystem.SF.SelectingLanguage = AK_Language.jp;
                SelectLangTMP.text = "日本語";
                ConfirmSelectLangTMP.text = "確認";
                SaveSystem.SaveSF();
                break;
        }
        gameLogScreenLoad.LoadLogScreenLan(); ;
        gameLogScreenLoad.LoadLanLogScreen();
    }

    public void ConfirmLangSelect()
    {
        SceneManager.LoadScene(0);
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

    public void SyncCamPos()
    {
        myCamPos.position = Vector3.Lerp(myCamPos.position, nowTargetCamPos.position, CamFluence * Time.deltaTime);
    }

    public void SwitchTrackingPos(Transform t_transform)
    {
        nowTargetCamPos = t_transform;
    }

    public void LanguageSettingButton()
    {
        LanguageSelectCanvasAnimator.SetBool("Active", true);

        int num = (int)SaveSystem.SF.SelectingLanguage;
        switch (num)
        {
            case 0://繁體中文
                SelectLangTMP.text = "繁體中文";
                ConfirmSelectLangTMP.text = "確認";
                break;

            case 1://簡體中
                SelectLangTMP.text = "简体中文";
                ConfirmSelectLangTMP.text = "确认";
                break;

            case 2://英文
                SelectLangTMP.text = "English";
                ConfirmSelectLangTMP.text = "Confirm";
                break;

            case 3://日文
                SelectLangTMP.text = "日本語";
                ConfirmSelectLangTMP.text = "確認";
                break;
        }
    }
    public void CloseLanguageSetting()
    {
        //Reload the scene 0?
        SceneManager.LoadScene(0);
    }
}
