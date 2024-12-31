using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class QuestWindow : MonoBehaviour
{
    [SerializeField]private List<QuestTitle> acceptedTitleList = new List<QuestTitle>();
    [SerializeField]private List<QuestTitle> clearedTitleList = new List<QuestTitle>();
    [SerializeField]private TextMeshProUGUI ClearText { get { return transform.Find("ClearedQuests").Find("Cleared").GetComponent<TextMeshProUGUI>(); } }

    void Start()
    {
        RegistGameOBJ();
        //기본적으로 세팅되어 있는 오브젝트를 첫번째 배열로 넣음
        acceptedTitleList.Add(transform.Find("ProgressQuests").Find("Scroll View").Find("Viewport").Find("Content").GetChild(0).GetComponent<QuestTitle>());
        clearedTitleList.Add(transform.Find("ClearedQuests").Find("Scroll View").Find("Viewport").Find("Content").GetChild(0).GetComponent<QuestTitle>());
    }


    public void OnEnable()
    {
        if(acceptedTitleList.Count>0)SetQuests(acceptedTitleList,true);
        if(clearedTitleList.Count>0)SetQuests(clearedTitleList,false);
    }
    public void ResetTitles(List<QuestTitle> list, bool isProgress)
    {
        QuestTitle[] requireReset = list.FindAll(Item => !Item.IsEmptyText).ToArray();
        UIManager.GetInstance().QuestInfo.text = string.Empty;
        for (int i = 0; i < requireReset.Length; i++)
        {
            requireReset[i].SetQuestTitle(string.Empty);
        }
    }
    public void SetQuests(List<QuestTitle> list,bool isProgress)
    {

        ResetTitles(list,isProgress);
        for (int i = 0; i < (isProgress ? QuestManager.GetInstance().AcceptedQuests.Count : QuestManager.GetInstance().ClearedQuests.Count); i++)
        {
            if(i>= list.Count ) 
            {
                QuestTitle temp = GameObject.Instantiate(list[0].gameObject, list[0].transform.parent).GetComponent<QuestTitle>();
                temp.transform.localPosition = list[i - 1].transform.localPosition - new Vector3(0f, ((RectTransform)list[0].transform).sizeDelta.y, 0f);
                list.Add(temp);
            }
            list[i].SetQuestTitle(isProgress ? QuestManager.GetInstance().AcceptedQuests[i].questID : QuestManager.GetInstance().ClearedQuests[i].questID);
        }
        ((RectTransform)list[0].transform.parent).sizeDelta = new Vector2(0, list.Count * ((RectTransform)list[0].transform).sizeDelta.y);
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
