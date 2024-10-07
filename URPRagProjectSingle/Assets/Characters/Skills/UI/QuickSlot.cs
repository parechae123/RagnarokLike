using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class QuickSlot : MonoBehaviour, IDragHandler, IEndDragHandler
{
    #region 변수
    public KeyCode slotKey;

    private IitemBase slotItem;
    public IitemBase SlotItem
    {
        get
        {
            return slotItem;
        }
        set
        {
            btn.interactable = false;
            iconImage.sprite = value.IconIMG;

            btn.interactable = true;
            slotItem = value;
            
            if (!isStaticSlot)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(SlotItem.UseItem);
            }
        }
    }
    public SlotType slotType
    {
        get 
        {
            return SlotItem.slotType;
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
    public void GetSlotKey()
    {
        SlotItem.UseItem();
    }
    public void Install(IitemBase tempData, bool isStaticSlot)
    {
        SlotItem = tempData;
        iconImage.sprite = SlotItem.IconIMG;
        this.isStaticSlot = isStaticSlot;
    }
    public void OnDrag(PointerEventData pp)
    {
        if (SlotItem == null || SlotItem.slotType == SlotType.None) return;
        if (!SlotItem.isItemUseAble) return;
        if (pp.button == PointerEventData.InputButton.Left) UIManager.GetInstance().DraggingIcons(pp.position, iconImage.sprite);
        else return;
    }
    public void OnEndDrag(PointerEventData pp)
    {
        // 마우스 위치에 대한 RaycastResult 리스트 생성
        if (SlotItem == null || SlotItem.GetType() == typeof(EmptyItem)) return;
        if (!SlotItem.isItemUseAble) return;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        UIManager.GetInstance().IconOnOFF(false);
        // RaycastAll을 호출하여 raycastResults에 결과 저장
        EventSystem.current.RaycastAll(pp, raycastResults);
        
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].gameObject == this.gameObject)
            {
                return;
            }

            if (raycastResults[i].gameObject.TryGetComponent<QuickSlot>(out QuickSlot targetSlot))
            {
                //드래그가 멈추는 지점의 대상 슬롯이 targetSlot
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
    /// 그냥 추가시
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isSwap"></param>
    public void RemoveSlot(Sprite sprite)
    {
        if (isStaticSlot) return;
        SlotItem = new EmptyItem(sprite);
    }
    public void ChangeSlot(IitemBase item)
    {
        
        if (isStaticSlot) return;
        if (item.slotType == SlotType.Skills)
        {
            SkillInfoInGame temp = (SkillInfoInGame)item;
            SlotItem = new SkillInfoInGame(temp);
            SlotText.text = item.slotNumberInfo;
        }
        else
        {
            SlotItem = item;
            SlotText.text = item.slotNumberInfo;
        }
    }
    /// <summary>
    /// 슬롯아이템 스왑시
    /// </summary>
    /// <param name="item"></param>
    public void SwapSlot( QuickSlot item)
    {
        if (isStaticSlot) return;
        IitemBase tempItemBase = item.SlotItem;
        item.SlotItem = SlotItem;
        SlotItem = tempItemBase;
        
    }
}


public interface IitemBase
{
    event Action quickSlotFuncs;
    Sprite IconIMG
    {
        get;
    }
    string slotNumberInfo
    {
        get;
    }
    bool isItemUseAble
    {
        get;
    }
    SlotType slotType
    {
        get;
    }
    void UseItem();
}
public class EmptyItem: IitemBase
{
    public event Action quickSlotFuncs;
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
    public bool isItemUseAble
    {
        get { return false; }
    }
    public void UseItem()
    {

    }
    public SlotType slotType { get { return SlotType.None; } }
}
public enum SlotType
{
    None,Equipments,ConsumableItem,Skills
}