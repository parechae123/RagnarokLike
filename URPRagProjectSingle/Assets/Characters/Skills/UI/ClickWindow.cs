using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickWindow : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UITypes uiTypes;
    [SerializeField] bool isClose;
    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (ShortCutOBJ item in KeyMapManager.GetInstance().keyMaps.Values)
        {
            if (item.UIType == uiTypes)
            {
                item.subScribFuncs?.Invoke();
                break;
            }
            else continue;
        }
        gameObject.SetActive(false);
    }
}
