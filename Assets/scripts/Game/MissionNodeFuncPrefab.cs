using DG.Tweening.Core.Easing;
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
        Debug.Log("Trigger Story activation");
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

    public void SpawnRandomTroop(int number)
    {
        for (int i = 0; i < number; i++)
        {
            RM().SpawnEnemyInPool();
        }
    }

    public void SpawnTargetTroop(SO_Chess sO_Chess)
    {
        GameBoardInsChess GBIC = new GameBoardInsChess();
        GBIC.chessFile = sO_Chess;

        RM().SpawnEnemy_RandomSpot(GBIC);
    }
}
