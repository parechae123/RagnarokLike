using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryTabs : MonoBehaviour ,IPointerClickHandler
{
    public GameObject targetTab;
    public void OnPointerClick(PointerEventData pp)
    {
        if (targetTab.activeSelf)
        {
            return;
        }
        else
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                InventoryTabs otherTab = transform.parent.GetChild(i).GetComponent<InventoryTabs>();
                otherTab.targetTab.SetActive(false);
                otherTab.SetColor(Color.white);
            }
            targetTab.SetActive(true);
            SetColor(Color.grey);
        }
    }

    public void SetColor(Color imageColor)
    {
        GetComponent<Image>().color = imageColor;
    }
}
