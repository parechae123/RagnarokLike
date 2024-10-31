using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventorySlots : QuickSlot
{
    public SlotType defaultSlotType;
    public sbyte slotNumber;
    private IItemBase itemData;
    
    private Button btn
    {
        get { return GetComponent<UnityEngine.UI.Button>(); }
    }
    public override IItemBase SlotItem
    {
        get => itemData;
        set
        {
            if (value == null) 
            {
                iconImage.sprite = null;
                btn.interactable = false;
                btn.onClick.RemoveAllListeners();
                itemData = null;
                return;
            } 
            if (value.slotType == SlotType.None) return;
            btn.interactable = false;
            iconImage.sprite = value.IconIMG;

            btn.interactable = true;
            itemData = value;


            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(SlotItem.UseItem);
        }
    }

    public override void OnEndDrag(PointerEventData pp)
    {
        // 마우스 위치에 대한 RaycastResult 리스트 생성
        if (SlotItem == null || SlotItem.GetType() == typeof(EmptyItem)) return;
        if (!SlotItem.IsItemUseAble && SlotItem.slotType == SlotType.Skills) return;
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

            if (raycastResults[i].gameObject.TryGetComponent<InventorySlots>(out InventorySlots targetSlot))
            {
                //드래그가 멈추는 지점의 대상 슬롯이 targetSlot
                if (isStaticSlot)
                {
                    targetSlot.ChangeSlot(SlotItem);
                }
                else
                {
                    if (targetSlot.SlotItem == null)
                    {
                        targetSlot.SlotItem = new EmptyItem(targetSlot.iconImage.sprite);
                    }
                    targetSlot.SwapInvenSlot(this);
                }
                return;
            }
            else if (raycastResults[i].gameObject.TryGetComponent<QuickSlot>(out QuickSlot quickSlots))
            {
                //드래그가 멈추는 지점의 대상 슬롯이 quickSlots
                if (quickSlots.SlotItem == null)
                {
                    quickSlots.SlotItem = new EmptyItem(quickSlots.iconImage.sprite);
                }
                quickSlots.ChangeSlot(SlotItem);
                return;
            }
        }
        RemoveSlot(default);
    }




    public void SwapInvenSlot(InventorySlots item)
    {
        if (isStaticSlot || item.slotType != defaultSlotType) return;
        switch (defaultSlotType)
        {
            case SlotType.Equipments:
                UIManager.GetInstance().equipInven.SwapItem(item.slotNumber, slotNumber);
                break;
            case SlotType.ConsumableItem:
                UIManager.GetInstance().consumeInven.SwapItem(item.slotNumber, slotNumber);
                break;
            case SlotType.MISC:
                break;
        }
        IItemBase tempItemBase = item.SlotItem;
        item.SlotItem = SlotItem;
        SlotItem = tempItemBase;
        item.ListnerUpdate();
        ListnerUpdate();
    }

    public void ListnerUpdate()
    {
        btn.onClick.RemoveAllListeners();
        if(SlotItem != null) btn.onClick.AddListener(SlotItem.UseItem);


    }
    bool IsEmptySlot
    {
        get { return itemData == null ? true : false; }
    }


    #region 초기 설정

    private void Reset()
    {
        slotNumber = (sbyte)transform.parent.GetSiblingIndex();

        switch (transform.parent.parent.name)
        {
            case "CosumeableTab":
                defaultSlotType = SlotType.ConsumableItem;
                break;
            case "EquipTabs":
                defaultSlotType = SlotType.Equipments;
                break;
            case "MiscTabs":
                defaultSlotType = SlotType.MISC;
                break;
            case "Weapon":
                defaultSlotType = SlotType.Equipments;
                break;
            case "Armors":
                defaultSlotType = SlotType.Equipments;
                break;
        } 
    }
    #endregion

}

