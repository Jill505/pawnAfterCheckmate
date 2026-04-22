using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

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

    public bool isNodeReachClog = false;

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
        isNodeReachClog = false;

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

    public bool isMissionTypeSurviveFunc()
    {
        if (myMissionNodes[currentIndex].missionType != MissionType.Survive)
        {
            return false;
        }
        return true;    
    }

    public void CheckKillReport(int UID)
    {
        if (UID == Target_TSA_UID)
        {
            NowKillAmount++;
        }
        NodeProcessCheck();
    }

    public void NodeProcessCheck()
    {
        bool checkFlag = false;
        switch (myMissionNodes[currentIndex].missionType)
        {
            case MissionType.KillTarget:
                if (NowKillAmount >= TargetKillAmount) 
                {
                    checkFlag = true;
                }
                break;

            case MissionType.Survive:
                if (NowSurviveRound >= TargetSurviveRound) 
                {
                    checkFlag = true;
                    Debug.Log("Check Flag Form");
                }
                break;

            case MissionType.Special:
                break;
        }

        if (checkFlag)
        {
            Debug.Log("Check Flag_可以怪checkFlag");
            //the node is finish
            //if (isNodeReachClog == true) return;

            Debug.Log("Check Flag Use");
            myMissionNodes[currentIndex].ActionOnNodeReach.Invoke();
            isNodeReachClog = true;
        }
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