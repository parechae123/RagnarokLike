using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowExit : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Transform target;
    public void OnPointerClick(PointerEventData data)
    {
        target.gameObject.SetActive(false);
    }
}
