using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLogScreenManager : MonoBehaviour
{
    [Header("GameObject Refs")]
    public GameObject SettingCanvas;

    [Header("UI Refs")]
    public Text DiffTextShowcase;

    public Text MusicVolumeTextShowcase;
    public Text SFXVolumeTextShowcase;

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

        SaveSystem.SaveSF();
    }
}
