using System.Collections;
using Unity.Loading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [Header("Component Ref")]
    public SO_Level loadLevel;
    public GameObject levelConstructorGameObject;
    public GameLobbyCameraController GLCC;

    [Header("Level Canvas")]
    public Text levelShowName;
    public Text levelShowDesc;

    public void LoadLevel()
    {
        if (loadLevel == null)
        {
            Debug.LogError("Ak Error: The Level Scriptable Object didn't ");
        }
        else
        {
            Debug.Log("¸ü¤JÃö¥dID¡G" + loadLevel.levelID);

            GameObject swap = Instantiate(levelConstructorGameObject);
            swap.GetComponent<LevelConstructor>().levelInfo = loadLevel;
            DontDestroyOnLoad(swap);

            StartCoroutine(LoadSceneCoroutine());
            //SceneManager.LoadScene("Fight");
        }
    }

    public Animator canvasMaskAnimator;
    public IEnumerator LoadSceneCoroutine()
    {
        GLCC.OnAutoOrthographicSize = 0.05f;
        canvasMaskAnimator.SetTrigger("MaskT");
        yield return new WaitForSeconds(1.4f);
        SceneManager.LoadScene("Fight");
    }

    public void ShowLevelContext(SO_Level soLevel)
    {
        levelShowName.text = soLevel.name;
        levelShowDesc.text = soLevel.levelDesc;

        loadLevel = soLevel;
    }

    public void IntoLevel(SO_Level level)
    {
        GameObject Constructor = Instantiate(levelConstructorGameObject);
        if (loadLevel != null)
        {
            Constructor.GetComponent<LevelConstructor>().levelInfo = loadLevel;
        }
        else
        {
            Debug.LogError("Ak Error: The level Scriptable Object didn't load or it's NULL");
        }
    }
}
