using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class QuickSlot : MonoBehaviour, IDragHandler, IEndDragHandler
{
    #region 변수
    private ItemBase slotItem;
    private ItemBase SlotItem
    {
        get
        {
            return slotItem;
        }
        set
        {
            btn.interactable = false;
            iconImage.sprite = value.IconIMG;
            SlotText.text = value.slotNumberInfo;
            btn.interactable = true;
            slotItem = value;
        }
    }
    private Button btn
    {
        get;
        set;
    }
    private Image iconImage
    {
        get 
        { 
            btn ??= transform.GetComponentInChildren<Button>();
            return btn.image;
        }
    }
    private Text slotText
    {
        get;
        set;
    }
    private Text SlotText
    {
        get 
        { 
            slotText ??= transform.GetComponentInChildren<Text>();
            return slotText;
        }
    }
    public Vector3 defaultPos;
    public bool isStaticSlot;
    #endregion
    private void Awake()
    {
        defaultPos = transform.position;
        btn = transform.GetComponentInChildren<Button>();
        iconImage.sprite = slotItem.IconIMG;
    }
    public void Install(ItemBase tempData, bool isStaticSlot)
    {
        btn = transform.GetComponentInChildren<Button>();
        SlotItem = tempData;
        this.isStaticSlot = isStaticSlot;
        defaultPos = transform.position;
    }
    public void OnDrag(PointerEventData pp)
    {
        transform.position = pp.position;
    }
    public void OnEndDrag(PointerEventData pp)
    {
        // 마우스 위치에 대한 RaycastResult 리스트 생성
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        // RaycastAll을 호출하여 raycastResults에 결과 저장
        EventSystem.current.RaycastAll(pp, raycastResults);
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject == this.gameObject)
            {
                continue;
            }

            if (raycastResults[i].gameObject.TryGetComponent<QuickSlot>(out QuickSlot targetSlot))
            {
                if (isStaticSlot)
                {
                    targetSlot.ChangeSlot(SlotItem);
                }
                else
                {
                    if (targetSlot.SlotItem == null)
                    {
                        targetSlot.ChangeSlot(SlotItem);
                    }
                    else
                    {
                        targetSlot.SwapSlot(ref slotItem);
                    }
                }

                break;
            }
        }
        transform.position = defaultPos;
    }
    /// <summary>
    /// 그냥 추가시
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isSwap"></param>
    public void ChangeSlot(ItemBase item)
    {
        if (isStaticSlot) return;
        SlotItem = item;
    }
    /// <summary>
    /// 슬롯아이템 스왑시
    /// </summary>
    /// <param name="item"></param>
    public void SwapSlot(ref ItemBase item)
    {
        if (isStaticSlot) return;
        ItemBase tempItemBase = item;
        item = slotItem;
        SlotItem = tempItemBase;
    }
}
public interface ItemBase
{
    public Sprite IconIMG
    {
        get { return null; }
    }
    public string slotNumberInfo
    {
        get { return null; }
    }
    public void SlotFunction(Vector3 effectPosition)
    {
        
    }
}