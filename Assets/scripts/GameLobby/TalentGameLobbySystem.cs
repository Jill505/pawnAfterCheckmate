using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TalentGameLobbySystem : MonoBehaviour
{
    public GameObject Main_TreeCanvasObject_NodeList;
    public List<TalentNodeMono> talentNodeMonoList = new List<TalentNodeMono>();
    public Image isSelectingMain_Icon;

    public GameObject StrawMan_TreeCanvasObject_NodeList;
    public List<TalentNodeMono> talentNodeMonoList_StrawMan = new List<TalentNodeMono>();
    public Image isSelectingStrawMan_Icon;

    public Animator nodeInfoAnimator;
    public TextMeshProUGUI nodeName;
    public TextMeshProUGUI nodeDesc;
    public int talentPoint = 0;

    public bool isHovering = false;
    public float informationHoverDownTime = 1f;
    public float informationHoverDownCT = 0f;

    public int talentListNowLoadingSort = 0 ;

    public void Start()
    {
        LoadTalentNodeUnlock();
        SwitchToNodeTree(0);
    }

    private void Update()
    {
        //LoadTalentNodeUnlock();
        if (isHovering == false)
        {
            informationHoverDownCT -= Time.deltaTime;
        }

        if (informationHoverDownCT < 0)
        {
            nodeInfoAnimator.SetBool("showNodeInfo", false);
        }
    }
    public void ShowNodeInfo()
    {
        nodeInfoAnimator.SetBool("showNodeInfo", true);
        isHovering = true;
    }
    public void HideNodeInfo()
    {
        informationHoverDownCT = informationHoverDownTime;
        isHovering = false;
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

        //Main
        if (talentListNowLoadingSort == 0)
        {
            for (int i = 0; i < talentNodeMonoList.Count; i++)
            {
                talentNodeMonoList[i].isUnlock = SaveSystem.SF.talentTreeUnlock[i];
                talentNodeMonoList[i].SyncState();
                talentNodeMonoList[i].DrawLinkLine();
            }
        }

        if (talentListNowLoadingSort == 1)
        {
            for (int i = 0; i < talentNodeMonoList_StrawMan.Count; i++)
            {
                talentNodeMonoList_StrawMan[i].isUnlock = SaveSystem.SF.talentTreeUnlock_Strawman[i];
                talentNodeMonoList_StrawMan[i].SyncState();
                talentNodeMonoList_StrawMan[i].DrawLinkLine();
            }
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

    public void SwitchToNodeTree(int changeToIndex)
    {
        LoadTalentNodeUnlock();

        talentListNowLoadingSort = changeToIndex;

        Main_TreeCanvasObject_NodeList.SetActive(false);
        StrawMan_TreeCanvasObject_NodeList.SetActive(false);

        isSelectingMain_Icon.gameObject.SetActive(false);
        isSelectingStrawMan_Icon.gameObject.SetActive(false);

        if (talentListNowLoadingSort == 0)
        {
            //Load Main
            Main_TreeCanvasObject_NodeList.SetActive(true);
            isSelectingMain_Icon.gameObject.SetActive(true);
        }

        if (talentListNowLoadingSort == 1)
        {
            //Load Strawman
            StrawMan_TreeCanvasObject_NodeList.SetActive(true);
            isSelectingStrawMan_Icon.gameObject.SetActive(true);
        }
    }
}