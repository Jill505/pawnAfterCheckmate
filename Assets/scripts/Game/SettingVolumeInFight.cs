using UnityEngine;
using UnityEngine.UI;

public class SettingVolumeInFight : MonoBehaviour
{
    public GameObject Self;

    public Text bgmVolumeText;
    public Text sFXVolumeText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Self.activeInHierarchy)
        {
            bgmVolumeText.text = "BGM volume: " + (int)(SaveSystem.SF.BgmVolume*100f) + "%";
            sFXVolumeText.text = "SFX volume: " + (int)(SaveSystem.SF.SFXVolume*100f) + "%";
        }
    }

    public void AddFiveBgmVolume()
    {
        SaveSystem.SF.BgmVolume += 0.05f;
        SaveSystem.SaveSF();
    }
    public void MinusFiveBgmVolume()
    {
        SaveSystem.SF.BgmVolume -= 0.05f;
        SaveSystem.SaveSF();
    }
    public void AddFiveSfxVolume()
    {
        SaveSystem.SF.SFXVolume += 0.05f;
        SaveSystem.SaveSF();
    }
    public void MinusFiveSfxVolume()
    {
        SaveSystem.SF.SFXVolume -= 0.05f;
        SaveSystem.SaveSF();
    }


    public void CloseSettingPanel()
    {
        Self.SetActive(false);
    }

    public void OpenSettingPanel()
    {
        Self.SetActive(true);
    }
}
