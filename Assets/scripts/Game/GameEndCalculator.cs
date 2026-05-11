using UnityEngine;
using UnityEngine.UI;

public class GameEndCalculator : MonoBehaviour
{
    public Animator GameEndCanvasAnimator;
    public Image GameEndCanvasBg_L;
    public Image GameEndCanvasBg_R;

    public void EndAnimatorActive(bool isPlayerDie)
    {
        if (isPlayerDie)
        {
            GameEndCanvasBg_L.color = Color.black;
            GameEndCanvasBg_R.color = Color.black;
        }
        else
        {
            GameEndCanvasBg_L.color = Color.white;
            GameEndCanvasBg_R.color = Color.white;
        }
        GameEndCanvasAnimator.SetTrigger("Active");
    }

    public void LoadGameEndFunc()
    {

    }

}
