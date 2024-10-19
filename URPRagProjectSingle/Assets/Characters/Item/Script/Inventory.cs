using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory<T> where T : inventoryItemBase
{
    private RectTransform slotParent;
    public Inventory(string invenName)
    {
        slotParent = (RectTransform)GameObject.Find("Canvas").transform.Find("Inventory").Find(invenName);
    }
    public T[] this[int index]
    {
        get { return invenIndex; }
    }
    T[] invenIndex = new T[59];
    private T GetEmptySlot
    {
        set 
        {
            for (int i = 0; i < invenIndex.Length; i++)
            {
                if(invenIndex[i] == null)
                {
                    invenIndex[i] = value;
                    invenIndex[i].Amount = value.Amount;
                    slotParent.GetChild(i).GetChild(0).GetComponent<EquipmentSlots>().SlotItem = value;
                    break;
                }
                if (invenIndex[i].IsEmptySlot)
                {
                    invenIndex[i] = value;
                    invenIndex[i].Amount = value.Amount;
                    slotParent.GetChild(i).GetChild(0).GetComponent<EquipmentSlots>().SlotItem = value;
                    break;
                }
                else if(invenIndex[i].isStackAble)
                {
                    int temp = (int)invenIndex[i].Amount + (int)value.Amount;
                    if (temp > 0&& temp <= sbyte.MaxValue)
                    {
                        invenIndex[i].Amount += value.Amount;
                        slotParent.GetChild(i).GetChild(0).GetComponent<EquipmentSlots>().SlotItem = invenIndex[i];
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            
        }
    }
    public void GetItems(T targetItem)
    {
        GetEmptySlot = targetItem;
    }
}
