using UnityEngine;
using AKTool;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class AKToolBox_Prefab : MonoBehaviour
{
    public Coroutine Delay(float sec, Action func)
    {
        return StartCoroutine(LateFuncCoroutine(sec, func));
    }
    public IEnumerator LateFuncCoroutine(float sec, Action func)
    {
        yield return new WaitForSeconds(sec);
        func();
    }

    public void GlobalLoadGameLevel(SO_Level SOL)
    {
        LevelLoader.retryStatic_SO_Level = SOL;
        LevelLoader.retryGameBool = true;
        SceneManager.LoadScene(1);
    }
}
