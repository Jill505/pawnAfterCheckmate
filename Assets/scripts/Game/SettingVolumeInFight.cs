using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingVolumeInFight : MonoBehaviour
{
    public GameObject Self;

    public Text bgmVolumeText;
    public Text sFXVolumeText;

    public TextMeshProUGUI curveVisualizeLevelButtonText;


    public void Start()
    {
        CurveVisualizeTextSync(SaveSystem.SF.curveVisualizeLevel);
    }

    private void FixedUpdate()
    {
        if (Self.activeInHierarchy)
        {
            bgmVolumeText.text = "BGM volume: " + (int)(SaveSystem.SF.BgmVolume*100f) + "%";
            sFXVolumeText.text = "SFX volume: " + (int)(SaveSystem.SF.SFXVolume*100f) + "%";
        }
    }


    public void CloseSettingPanel()
    {
        Self.SetActive(false);
    }

    public void OpenSettingPanel()
    {
        Self.SetActive(true);
    }

    public void ChangeCurveVisualizeLevel()
    {
        SaveSystem.LoadSF();
        int l = SaveSystem.SF.curveVisualizeLevel;
        l = l + 1 > 3 ? 0 : l + 1;

        CurveVisualizeTextSync(l);

        SaveSystem.SF.curveVisualizeLevel = l;
        SaveSystem.SaveSF();

        FindFirstObjectByType<CameraManager>().LoadCurveSetting();
    }

    public void CurveVisualizeTextSync(int l)
    {

        switch (l)
        {
            case 0:
                curveVisualizeLevelButtonText.text = "關閉";
                break;

            case 1:
                curveVisualizeLevelButtonText.text = "輕微";
                break;

            case 2:
                curveVisualizeLevelButtonText.text = "標準";
                break;
            case 3:
                curveVisualizeLevelButtonText.text = "強烈";
                break;
        }
    }
}
