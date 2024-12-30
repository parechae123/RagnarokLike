using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class QuestWindow : MonoBehaviour
{
    private List<QuestTitle> acceptedTitleList = new List<QuestTitle>();
    private TextMeshProUGUI ClearText { get { return transform.Find("ClearedQuests").Find("Cleared").GetComponent<TextMeshProUGUI>(); } }
    void Start()
    {
        RegistGameOBJ();
        //기본적으로 세팅되어 있는 오브젝트를 첫번째 배열로 넣음
        acceptedTitleList.Add(transform.Find("ProgressQuests").Find("Scroll View").Find("Viewport").Find("Content").GetChild(0).GetComponent<QuestTitle>());
    }


    public void OnEnable()
    {
        if(acceptedTitleList.Count>0)SetAcceptedQuests();
        SetClearQuests();
    }
    public void ResetTitles()
    {
        QuestTitle[] requireReset = acceptedTitleList.FindAll(Item => !Item.IsEmptyText).ToArray();
        UIManager.GetInstance().QuestInfo.text = string.Empty;
        for (int i = 0; i < requireReset.Length; i++)
        {
            requireReset[i].SetQuestTitle(string.Empty);
        }
    }
    public void SetAcceptedQuests()
    {

/*        QuestInfo.text = string.Empty;
        Transform acceptedContent = QuestWindow.Find("ProgressQuests").Find("Scroll View").Find("Viewport").Find("Content");
        QuestWindow.Find("ClearedQuests").Find("Scroll View").Find("Viewport").Find("Content").GetChild(0).GetComponent<TextMeshProUGUI>().text = QuestManager.GetInstance().GetCleardQuestNames();
        int acceptedQuestCount = QuestManager.GetInstance().AcceptedQuests.Count;
        int questTitleCount = acceptedContent.childCount;
        for (int i = 0; i < (acceptedQuestCount > questTitleCount ? acceptedQuestCount : questTitleCount); i++)
        {
            if (questTitleCount <= i)
            {
                RectTransform currRT = (RectTransform)(GameObject.Instantiate(acceptedContent.GetChild(i), QuestWindow).transform);
                currRT.anchoredPosition = -(((RectTransform)(acceptedContent.GetChild(i - 1))).rect.height * Vector2.up);
            }
            //TODO : 해당 코드에서 height 늘려주는 방식을 참고해서 Content 오브젝트 유지보수하여 추가작성 필요

        }*/

        ResetTitles();
        for (int i = 0; i < QuestManager.GetInstance().AcceptedQuests.Count; i++)
        {
            if(i< acceptedTitleList.Count - 1) { acceptedTitleList.Add(GameObject.Instantiate(acceptedTitleList[0].gameObject, acceptedTitleList[0].transform.parent).GetComponent<QuestTitle>()); }
            acceptedTitleList[i].SetQuestTitle(QuestManager.GetInstance().AcceptedQuests[i].questID);
        }
    }
    public void SetClearQuests()
    {
        ClearText.text = QuestManager.GetInstance().GetCleardQuestNames();

    }

    public void RegistGameOBJ()
    {
        foreach (KeyCode item in KeyMapManager.GetInstance().keyMaps.Keys)
        {
            if (KeyMapManager.GetInstance().keyMaps[item].UIType == UITypes.QuestWindow)
            {
                ShortCutOBJ temp = KeyMapManager.GetInstance().keyMaps[item];


                temp.subScribFuncs = null;
                temp.subScribFuncs += OnOff;
                KeyMapManager.GetInstance().keyMaps[item] = temp;
                gameObject.SetActive(false);
                break;
            }
            else
            {
                continue;
            }
        }
    }
    public void OnOff() { gameObject.SetActive(!gameObject.activeSelf); }
}
