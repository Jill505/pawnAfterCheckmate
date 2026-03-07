using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AKTool;
using UnityEngine.EventSystems;

public class TalentNodeMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Floating Show Desc
    [Header("Lang Data")]
    public TextAsset mutiLangData;

    [Header("Manager Refs")]
    public TalentGameLobbySystem talentGameLobbySystem;

    [Header("UI")]
    public TextMeshProUGUI nodeNameTMP;
    public string nodeNameStr; 
    public TextMeshProUGUI nodeDescTMP;
    public string nodeDescStr;

    public Button talentUnlockButton;
    public Image talentFrameImage;

    [Header("DATA")]
    public TalentNode talentNode;

    public bool allowUnlock = false;
    public bool isUnlock = false;

    public string[] langData = new string[0];

    public Sprite LockFrameSprite;
    public Sprite AllowUnlockFrameSprite;
    public Sprite UnlockFrameSprite;

    public void Start()
    {
        LoadLangData();
        talentGameLobbySystem = FindFirstObjectByType<TalentGameLobbySystem>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("滑鼠移到 Button 上了");
        HoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("滑鼠離開 Button");
        HoverExit();
    }

    void HoverEnter()
    {
        talentGameLobbySystem.nodeName.text = nodeNameStr;
        talentGameLobbySystem.nodeDesc.text= nodeDescStr;
        talentGameLobbySystem.ShowNodeInfo();
        // 你要觸發的方法
    }

    void HoverExit()
    {
        talentGameLobbySystem.HideNodeInfo();
        // 離開時要做的事
    }

    public void OnClick() //玩家嘗試與之互動
    {
        if (SaveSystem.SF.talentTreeUnlock[talentNode.talentNodeID])
        {
            Debug.Log("You already unlock the node!");
            return;
        }

        if (SaveSystem.SF.skillPoint >= talentNode.talentNodeUnlockRequirePoint)
        {
            // it is clickable
            SaveSystem.SF.talentTreeUnlock[talentNode.talentNodeID] = true;
            SaveSystem.SF.skillPoint -= talentNode.talentNodeUnlockRequirePoint;
            SaveSystem.SaveSF();
            //Reload all trees
            talentGameLobbySystem.LoadTalentNodeUnlock();
        }
        else
        {
            //也許讓Node震動 並提示點數不足？
        }
    }

    public void LoadLangData()
    {
        AK_ToolBox.LoadLangData(mutiLangData, ref langData);
        //nodeNameTMP.text = langData[0];
        nodeNameStr = langData[0];  
        //nodeDescTMP.text = langData[1];
        nodeDescStr = langData[1];
    }

    public void SyncState() //同步狀態
    {
        //如果前置節點沒有被開啟 那自己不能被開啟
        //如果自己的ID是0 那可以允許被點擊

        if (SaveSystem.SF.talentTreeUnlock[talentNode.talentNodeID])
        {
            talentUnlockButton.interactable = true;
            talentFrameImage.sprite = UnlockFrameSprite;
            //talentFrameImage.color = new Color(1, 1, 1, 1);
            return;
        }

        bool isAllowUnlock = false;

        if (talentNode.talentNodeID == 0)
        {
            isAllowUnlock = true;   
        }
        else
        {
            Debug.Log("Number- " + talentNode.allowUnlockConditionNodeID.Length);
            for (int i = 0; i < talentNode.allowUnlockConditionNodeID.Length; i++)
            {
                if (SaveSystem.SF.talentTreeUnlock[talentNode.allowUnlockConditionNodeID[i]])
                {
                    isAllowUnlock = true;
                    //break;
                }
                else
                {
                    isAllowUnlock = false;
                }
            }
        }
        if (isAllowUnlock)
        {
            allowUnlock = true;
            talentUnlockButton.interactable = true;
            talentFrameImage.sprite = AllowUnlockFrameSprite;
            //talentFrameImage.color = new Color(1, 1, 1, 1f);
            //Debug.Log("System call me Unlock!");
        }
        else
        {
            allowUnlock = false;
            talentUnlockButton.interactable = false;
            talentFrameImage.sprite = LockFrameSprite;

            //talentFrameImage.color = new Color(0.1f, 0.1f, 0.1f, 0.4f);

            //Debug.Log("System call me lock!");
        }
    }
}
[System.Serializable]
public class TalentNode
{
    public int talentNodeID = 0;
    public int talentNodeUnlockRequirePoint = 1;
    public int[] allowUnlockConditionNodeID = new int[0];
}