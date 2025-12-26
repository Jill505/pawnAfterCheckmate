using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelConstructor : MonoBehaviour
{
    [Header("The Level Gonna Be Load")]
    public SO_Level levelInfo;
    public SO_PlayerConfig playerConfig;

    public int playerAllowLoadMaxIndex = 3;
    public int[] playerSelectChessSorts;

    public SpecialLevelScript SLS;

    private void Awake()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        //Debug.Log("awake촑좬" + currentSceneName);
        AddSLSOnMe();
    }
    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        //Debug.Log("start촑좬" + currentSceneName);

        if (currentSceneName == "Fight")
        {
            Debug.Log("Ω첿놓뉴촑 Fight");
            GameObject.Find("GameManager").GetComponent<GameManager>().levelConstructor = this;
        }
        else
        {
        }
    }

    public void setLoad()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        //Debug.Log("start촑좬" + currentSceneName);

        if (currentSceneName == "Fight")
        {
            Debug.Log("Ω첿놓뉴촑 Fight");
            GameObject.Find("GameManager").GetComponent<GameManager>().levelConstructor = this;
        }
        else
        {
        }
    }

    public void AddSLSOnMe()
    {
        switch (levelInfo.SLST)
        {
            case SpecialLevelScriptType.noSLS:
                SLS = null;
                break;
            case SpecialLevelScriptType.Karen:
                var swapSLS =  gameObject.AddComponent<SLS_Karen>();
                swapSLS.sO_Level = levelInfo;

                SLS = swapSLS;
                break;  
        }
    }
}
