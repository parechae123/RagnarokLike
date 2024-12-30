using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class QuestTitle : MonoBehaviour,IPointerClickHandler
{
    TextMeshProUGUI titleText;
    TextMeshProUGUI TitleText { get { if (titleText == null){ titleText = GetComponent<TextMeshProUGUI>(); } return titleText; } }

    public bool IsEmptyText=> TitleText.text == string.Empty;

    public void SetQuestTitle(string title)
    {
        if (title == string.Empty)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
            TitleText.text = ResourceManager.GetInstance().NameSheet.GetUINameValue(title);
        }
    }

    void OnDisable()
    {
        UIManager.GetInstance().QuestInfo.text = string.Empty;
    }
    public void OnPointerClick(PointerEventData pp)
    {
        string tempText = string.Empty;
        tempText = TitleText?.text;
        if (tempText != string.Empty)
        {
            Quest tempQuest = QuestManager.GetInstance().FIndQuest(ResourceManager.GetInstance().NameSheet.GetUIOriginValue(tempText));
            
            if(tempQuest != null)
            {
                UIManager.GetInstance().QuestInfo.text = $"<size=25>{tempText}</size>"+'\n'+tempQuest.GetAllDescriptions();
            }
        }

    }
}
