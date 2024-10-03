using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IuiInterface
{
    KeyCode uiOpenKey
    {
        get;
    }
}
public enum UITypes
{ 
    none,CombKey,SkillTreeWindow,StatusWindow,InventoryWindow,EquipmentWindow,QuickSlotOne, QuickSlotTwo, QuickSlotThree, QuickSlotFour, QuickSlotFive, QuickSlotSix, QuickSlotSeven, QuickSlotEight,QuickSlotNine
}