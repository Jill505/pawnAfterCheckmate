using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StorySceneManager : MonoBehaviour
{
    [Header("Refs")]
    public StoryManager storyManager;

    static public SO_Story StaticSO_Story;

    public void Start()
    {
        if (StaticSO_Story != null)
        {
            storyManager.StoryToLoad = StaticSO_Story;
            storyManager.LoadStory();
        }
        else
        {
            Debug.LogWarning("沒有故事可載入");
        }
    }
}
