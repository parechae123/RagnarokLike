using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTest : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public Vector3 defaultPos;
    public bool isSlotActivated
    {
        get 
        {  
            
            return gameObject.activeSelf;
        }
    }
    public List<GameObject> taaa;
    private void Awake()
    {
        defaultPos = transform.position;
    }
    public void OnBeginDrag(PointerEventData pp)
    {
        Debug.Log("��");
        Debug.Log(pp.GetHashCode());
    }
    public void OnDrag(PointerEventData pp)
    {
        transform.position = pp.position;
    }
    public void OnEndDrag(PointerEventData pp)
    {
        // ���콺 ��ġ�� ���� RaycastResult ����Ʈ ����
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        // RaycastAll�� ȣ���Ͽ� raycastResults�� ��� ����
        EventSystem.current.RaycastAll(pp, raycastResults);
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject == this.gameObject)
            {
                continue;
            }

            if (raycastResults[i].gameObject.TryGetComponent<EventTest>(out EventTest outPut))
            {
                outPut.gameObject.SetActive(outPut.isSlotActivated == true ? false : true);
                break;
            }
        }
        transform.position = defaultPos;
    }
}
