using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class UIWIndowMover : MonoBehaviour,IDragHandler
{
    [SerializeField]private RectTransform targetWIndow;
    [SerializeField]private RectTransform rt;

    private void Awake()
    {
        targetWIndow = targetWIndow != null ? targetWIndow : (RectTransform)transform.parent;
        rt = rt != null ? rt : (RectTransform)transform;
    }

    public void OnDrag(PointerEventData PP)
    {
        targetWIndow.position = PP.position;
        
        
    }
    
}
