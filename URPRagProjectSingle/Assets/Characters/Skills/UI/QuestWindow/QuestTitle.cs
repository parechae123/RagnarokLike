using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class QuestTitle : MonoBehaviour,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pp)
    {
        string tempText = string.Empty;
        tempText = GetComponent<TextMeshProUGUI>()?.text;
        if (tempText != string.Empty)
        {
            Quest tempQuest = QuestManager.GetInstance().FIndQuest(ResourceManager.GetInstance().NameSheet.GetUIOriginValue(tempText));
            
            if(tempQuest != null)
            {
                UIManager.GetInstance().QuestInfo.text = $"<size = 25>{tempText}</size>"+'\n'+tempQuest.GetAllDescriptions();
            }
        }

    }
}
