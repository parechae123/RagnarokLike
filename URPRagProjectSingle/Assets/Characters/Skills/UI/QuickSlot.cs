using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class QuickSlot : MonoBehaviour, IDragHandler, IEndDragHandler
{
    #region ����
    private ItemBase slotItem;
    public ItemBase SlotItem
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
            if (!isStaticSlot)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(SlotItem.UseItem);
            }
        }
    }
    private Button btn
    {
        get { return transform.GetComponentInChildren<Button>(); }
    }
    private Image iconImage
    {
        get 
        { 
            
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
    public bool isStaticSlot;
    #endregion
    /*    private void Awake()
        {
            defaultPos = transform.position;
            btn = transform.GetComponentInChildren<Button>();
            iconImage.sprite = SlotItem.IconIMG;
        }*/
    private void Awake()
    {

    }
    public void Install(ItemBase tempData, bool isStaticSlot)
    {
        SlotItem = tempData;
        iconImage.sprite = SlotItem.IconIMG;
        this.isStaticSlot = isStaticSlot;
    }
    public void OnDrag(PointerEventData pp)
    {
        if (SlotItem == null || SlotItem.GetType() == typeof(EmptyItem)) return;
        if (!SlotItem.isItemUseAble) return;
        if (pp.button == PointerEventData.InputButton.Left) UIManager.GetInstance().DraggingIcons(pp.position, iconImage.sprite);
        else return;
    }
    public void OnEndDrag(PointerEventData pp)
    {
        // ���콺 ��ġ�� ���� RaycastResult ����Ʈ ����
        if (SlotItem == null || SlotItem.GetType() == typeof(EmptyItem)) return;
        if (!SlotItem.isItemUseAble) return;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        UIManager.GetInstance().IconOnOFF(false);
        // RaycastAll�� ȣ���Ͽ� raycastResults�� ��� ����
        EventSystem.current.RaycastAll(pp, raycastResults);
        
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject == this.gameObject)
            {
                continue;
            }

            if (raycastResults[i].gameObject.TryGetComponent<QuickSlot>(out QuickSlot targetSlot))
            {
                //�巡�װ� ���ߴ� ������ ��� ������ targetSlot
                if (isStaticSlot)
                {
                    targetSlot.ChangeSlot(SlotItem);
                }
                else
                {
                    /*if (targetSlot.SlotItem == null)
                    {
                        targetSlot.ChangeSlot(SlotItem);
                    }
                    else
                    {
                        targetSlot.SwapSlot( this);
                    }*/
                    if (targetSlot.SlotItem == null)
                    {
                        targetSlot.SlotItem = new EmptyItem(targetSlot.iconImage.sprite); 
                    }
                    targetSlot.SwapSlot( this);
                }
                return;
            }
        }
        RemoveSlot(default);
    }
    /// <summary>
    /// �׳� �߰���
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isSwap"></param>
    public void RemoveSlot(Sprite sprite)
    {
        if (isStaticSlot) return;
        SlotItem = new EmptyItem(sprite);
    }
    public void ChangeSlot(ItemBase item)
    {
        
        if (isStaticSlot) return;
        SlotItem = item;
    }
    /// <summary>
    /// ���Ծ����� ���ҽ�
    /// </summary>
    /// <param name="item"></param>
    public void SwapSlot( QuickSlot item)
    {
        if (isStaticSlot) return;
        ItemBase tempItemBase = item.SlotItem;
        item.SlotItem = SlotItem;
        SlotItem = tempItemBase;
        
    }
}


public interface ItemBase
{
    public event Action<Vector3> quickSlotFuncs;
    public Sprite IconIMG
    {
        get { return null; }
    }
    public string slotNumberInfo
    {
        get { return null; }
    }
    public bool isItemUseAble
    {
        get { return false; }
    }
    public void UseItem();
}
public class EmptyItem: ItemBase
{
    public event Action<Vector3> quickSlotFuncs;
    public EmptyItem(Sprite sprite) 
    {
        IconIMG = sprite;
    }
    public Sprite IconIMG
    {
        get;
        set;
    }
    public string slotNumberInfo
    {
        get { return null; }
    }
    public void UseItem()
    {

    }
}