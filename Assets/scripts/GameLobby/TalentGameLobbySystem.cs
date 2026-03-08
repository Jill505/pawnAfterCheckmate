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
        ClearAllLines();
        for (int i = 0; i < talentNodeMonoList.Count; i++)
        {
            talentNodeMonoList[i].isUnlock = SaveSystem.SF.talentTreeUnlock[i];
            talentNodeMonoList[i].SyncState();
            talentNodeMonoList[i].DrawLinkLine();
        }
    }

    public List<LineRenderer> lines = new List<LineRenderer>();

    public LineRenderer DrawLine(Vector2 a, Vector2 b, Color color, float width = 0.1f)
    {
        GameObject obj = new GameObject("Line2D");
        obj.transform.SetParent(transform);

        LineRenderer line = obj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = width;
        line.endWidth = width;
        line.startColor = color;
        line.endColor = color;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = 100;

        line.SetPosition(0, new Vector3(a.x, a.y, 0f));
        line.SetPosition(1, new Vector3(b.x, b.y, 0f));

        lines.Add(line);
        return line;
    }

    public void ClearAllLines()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] != null)
            {
                Destroy(lines[i].gameObject);
            }
        }

        lines.Clear();
    }
}