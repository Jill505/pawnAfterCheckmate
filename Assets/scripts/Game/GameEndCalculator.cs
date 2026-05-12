using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndCalculator : MonoBehaviour
{
    public Animator GameEndCalCanvasAnimator;
    public Animator GameEndCanvasAnimator;
    public Image GameEndCanvasBg_L;
    public Image GameEndCanvasBg_R;

    public Animator GameEndBlackScreenAnimator;

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
        GameEndCalCanvasAnimator.SetTrigger("GameEnd");

        //Set Text
    }

    public void LoadGameEndFunc()
    {

    }

    public void BackToLobbyFunc()
    {
        StartCoroutine(BackToLobbyCoroutine());
    }
    public IEnumerator BackToLobbyCoroutine()
    {
        GameEndBlackScreenAnimator.SetTrigger("Active");
        yield return null;
        yield return new WaitForSeconds(1.6f);
        SceneManager.LoadScene(1);
    }
}
