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

    public bool AwakeFunctionClog = false;

    private void Awake()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        //Debug.Log("awake是：" + currentSceneName);
        AwakeFunction();
    }

    public void AwakeFunction()
    {
        if (AwakeFunctionClog == false && levelInfo != null)
        {
            AwakeFunctionClog = true;
            AddSLSOnMe();
        }
        else
        {
            Debug.Log("禁止兩次初始化CLOG 或 levelInfo為null");
        }
    }
    void Start()
    {


        string currentSceneName = SceneManager.GetActiveScene().name;
        //Debug.Log("start是：" + currentSceneName);

        if (currentSceneName == "Fight")
        {
            Debug.Log("目前場景是 Fight");
            GameObject.Find("GameManager").GetComponent<GameManager>().levelConstructor = this;
        }
        else
        {
        }
    }

    public void setLoad()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        //Debug.Log("start是：" + currentSceneName);

        if (currentSceneName == "Fight")
        {
            Debug.Log("目前場景是 Fight");
            GameObject.Find("GameManager").GetComponent<GameManager>().levelConstructor = this;
        }
        else
        {
        }
    }

    public void AddSLSOnMe()
    {
        Debug.Log("ADDSO active");

        if (levelInfo == null)
        {
            //return;
            Debug.Log("Level Info doesn't exist");
        }
        else
        {
            Debug.Log("Level Info does exist");
        }

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
