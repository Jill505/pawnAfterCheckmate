using UnityEngine;

public class MissionNodeFuncPrefab : MonoBehaviour
{
    public RoundProcessManager RPM()
    {
        return FindFirstObjectByType<RoundProcessManager>();
    }

    public StoryManager SM()
    {
        return FindFirstObjectByType<StoryManager>();
    }

    public GameManager GM()
    {
        return FindFirstObjectByType<GameManager>();
    }
    public RoundManager RM()
    {
        return FindFirstObjectByType<RoundManager>();
    }


    public void GoNextNode()
    {
        RPM().GoNextNode();
    }

    public void LoadStory(SO_Story SOS)
    {
        SM().LoadStory(SOS);
    }

    public void PlayerWin()
    {
        RM().Win();
    }

    public void LoadPlayerWinCanvas()
    {

    }

    public void SpawnGoldenTarget()
    {
        RM().SpawnGoldenTarget_Random();
    }
}
