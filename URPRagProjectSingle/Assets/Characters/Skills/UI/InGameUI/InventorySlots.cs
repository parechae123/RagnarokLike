using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventorySlots : QuickSlot
{
    EquipPart part;
    public EquipPart GetPart
    {
        get { return part; }
    }
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
    bool IsEmptySlot
    {
        get { return itemData == null ? true : false; }
    }
    private void Start()
    {
        
    }

}

