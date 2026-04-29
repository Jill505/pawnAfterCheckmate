using UnityEngine;
using AKTool;
using System;
using System.Collections;

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
}
