using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TalentGameLobbySystem : MonoBehaviour
{
    public List<TalentNodeMono> talentNodeMonoList = new List<TalentNodeMono>();
    public Animator nodeInfoAnimator;
    public TextMeshProUGUI nodeName;
    public TextMeshProUGUI nodeDesc;
    public int talentPoint = 0;

    public void Start()
    {
        LoadTalentNodeUnlock();
    }

    private void Update()
    {
        //LoadTalentNodeUnlock();
    }
    public void ShowNodeInfo()
    {
        nodeInfoAnimator.SetBool("showNodeInfo", true);
    }
    public void HideNodeInfo()
    {
        nodeInfoAnimator.SetBool("showNodeInfo", false);
    }

    public void ResetTalent()
    {
        for (int i = 0; i < talentNodeMonoList.Count; i++)
        {
            if (talentNodeMonoList[i].isUnlock == true)
            {
                SaveSystem.SF.skillPoint += talentNodeMonoList[i].talentNode.talentNodeUnlockRequirePoint;
                SaveSystem.SF.talentTreeUnlock[talentNodeMonoList[i].talentNode.talentNodeID] = false;    
            }
        }
        SaveSystem.SaveSF();
        LoadTalentNodeUnlock();
    }
    public void LoadTalentNodeUnlock()
    {
        for (int i = 0; i < talentNodeMonoList.Count; i++)
        {
            talentNodeMonoList[i].isUnlock = SaveSystem.SF.talentTreeUnlock[i];
            talentNodeMonoList[i].SyncState();
        }
    }
}