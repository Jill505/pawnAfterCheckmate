using UnityEngine;

public class GameLobbyStoryAwaker : MonoBehaviour
{
    static public bool isSceneStoryLoad = false;
    static public SO_Story storyToLoad;

    public StoryManager SM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SM = FindFirstObjectByType<StoryManager>();

        if (isSceneStoryLoad)
        {
            isSceneStoryLoad = false;
            SM.StoryToLoad = storyToLoad;

            SM.LoadStory();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
