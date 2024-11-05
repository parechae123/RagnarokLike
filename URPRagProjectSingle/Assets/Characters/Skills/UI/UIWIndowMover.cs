using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class UIWIndowMover : MonoBehaviour,IDragHandler
{
    [SerializeField]private RectTransform targetWindow;
    [SerializeField]private RectTransform rt;

    private void Awake()
    {
        targetWindow = targetWindow != null ? targetWindow : (RectTransform)transform.parent;
        rt = rt != null ? rt : (RectTransform)transform;
    }

    public void OnDrag(PointerEventData PP)
    {
        targetWindow.position = PP.position;
        
        
    }
    
}
