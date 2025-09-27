using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelConstructor : MonoBehaviour
{
    [Header("The Level Gonna Be Load")]
    public SO_Level levelInfo;
    public SO_PlayerConfig playerConfig;

    public int playerAllowLoadMaxIndex = 3;
    public int[] playerSelectChessSorts;

    private void Awake()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        //Debug.Log("awake촑좬" + currentSceneName);
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
}
