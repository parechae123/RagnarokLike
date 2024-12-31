using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowExit : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData data)
    {
        transform.parent.parent.parent.gameObject.SetActive(!transform.parent.parent.parent.gameObject.activeSelf);
    }
}
