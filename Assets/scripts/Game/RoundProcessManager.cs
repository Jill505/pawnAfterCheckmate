using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class RoundProcessManager : MonoBehaviour
{
    public int currentIndex = 0;

    public MissionType nowMissionType;

    public int TargetSurviveRound = 0;
    public int NowSurviveRound = 0;

    public int Target_TSA_UID = 2;
    public int TargetKillAmount = 0;
    public int NowKillAmount = 0;

    public MissionNode[] myMissionNodes;
    

    public void InitRoundProcess(MissionNode[] missionNodes)
    {
        myMissionNodes = missionNodes;
        LoadMissionNode(0);
    }

    public void ConditionInit()
    {
        NowSurviveRound = 0;
        NowKillAmount = 0;
    }

    public void LoadMissionNode(int index)
    {
        if (index >= myMissionNodes.Length)
        {
            Debug.LogError("你正在嘗試載入超出範圍的節點");
            return; 
        }

        nowMissionType = myMissionNodes[index].missionType;

        ConditionInit();

        if (nowMissionType == MissionType.Survive)
        {
            TargetSurviveRound = myMissionNodes[index].RequireSurviveRound;
        }

        if (nowMissionType == MissionType.KillTarget)
        {
            TargetKillAmount = myMissionNodes[index].KillTargetAmount;
            Target_TSA_UID = myMissionNodes[index].KillTargetUID;
        }
    }

    public void ProcessCheck()
    {
        switch (myMissionNodes[currentIndex].missionType)
        {
            case MissionType.Survive:
                
                break;
        }
    }

    public bool isMissionTypeSurviveFunc()
    {
        if (myMissionNodes[currentIndex].missionType != MissionType.Survive)
        {
            return false;
        }


        return true;    
    }

    public void CheckIsKillTarget(int UID)
    {
        if (UID == Target_TSA_UID)
        {
            NowKillAmount++;
        }
    }

    public void NodeProcessCheck()
    {

    }

    public void GoNextNode()
    {
        currentIndex++;
        LoadMissionNode(currentIndex);
    }
}

[System.Serializable]
public class MissionNode
{
    public MissionType missionType;

    public int RequireSurviveRound;

    public int KillTargetUID;
    public int KillTargetAmount;

    public UnityEvent ActionOnNodeReach;
}