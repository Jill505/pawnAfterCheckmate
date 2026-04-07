using UnityEngine;

public class BackgroundOnClickCancel : MonoBehaviour
{
    public RoundManager roundManager;
    

    void Start()
    {
        roundManager = FindFirstObjectByType<RoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        if (roundManager.roundState == RoundState.MyRound)
        {
            roundManager.resetUnitSelectState();
        }
    }
}
