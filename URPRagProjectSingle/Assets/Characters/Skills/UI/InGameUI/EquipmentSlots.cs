using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EquipmentSlots : QuickSlot
{
    EquipPart part;
    public EquipPart GetPart
    {
        get { return part; }
    }
    public Equips equips;
    private Button btn
    {
        get { return GetComponent<UnityEngine.UI.Button>(); }
    }
    public override IItemBase SlotItem
    {
        get => equips;
        set
        {
            if (value.slotType == SlotType.None) return;
            btn.interactable = false;
            iconImage.sprite = value.IconIMG;

            btn.interactable = true;
            equips = (Equips)value;
            if (!isStaticSlot)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(SlotItem.UseItem);
            }

        }
    }
    bool IsEmptySlot
    {
        get { return equips == null ? true : !equips.isEquiped; }
    }
    private void Start()
    {
        //임시코드
        SlotItem = new Weapons();
    }

}

