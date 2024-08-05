using System.Collections;
using System.Collections.Generic;
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
        DragManager.GetInstance().slotPositions.Add(defaultPos,this);
    }
    public void OnBeginDrag(PointerEventData pp)
    {
        Debug.Log("¤·");
        Debug.Log(pp.GetHashCode());
    }
    public void OnDrag(PointerEventData pp)
    {
        transform.position = pp.position;
    }
    public void OnEndDrag(PointerEventData pp)
    {
        for (int i = 0; i < pp.hovered.Count; i++)
        {
            if (pp.hovered[i] == this) continue;
            if (DragManager.GetInstance().slotPositions.TryGetValue(pp.hovered[i].transform.position,out EventTest outPut))
            {
                outPut.gameObject.SetActive(outPut.isSlotActivated == true ? false : true);
                break;
            }
        }
        transform.position = defaultPos;

    }
}
public class DragManager : Manager<DragManager>
{
    public Dictionary<Vector3,EventTest> slotPositions = new Dictionary<Vector3, EventTest>();    
}
