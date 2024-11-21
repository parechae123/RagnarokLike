using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory<T> where T : InventoryItemBase
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
                    slotParent.GetChild(i).GetChild(0).GetComponent<InventorySlots>().SlotItem = value;
                    break;
                }
                if (invenIndex[i].IsEmptySlot)
                {
                    invenIndex[i] = value;
                    invenIndex[i].Amount = value.Amount;
                    slotParent.GetChild(i).GetChild(0).GetComponent<InventorySlots>().SlotItem = value;
                    break;
                }
                else if(invenIndex[i].isStackAble&&value.isStackAble)
                {
                    //TODO : 같은 아이템인지 검사할 요소를 넣어야함 요소검사 VS 아이템 코드검사 둘 중 하나 해야할듯
                    if (invenIndex[i].itemCode == value.itemCode) 
                    {
                        int temp = (int)invenIndex[i].Amount + (int)value.Amount;
                        if (temp > 0 && temp <= sbyte.MaxValue)
                        {
                            invenIndex[i].Amount = (sbyte)temp;
                            slotParent.GetChild(i).GetChild(0).GetComponent<InventorySlots>().SlotItem = invenIndex[i];
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
    }

    public void SwapItem(sbyte from,sbyte to) 
    {
        T tempObj = invenIndex[to];
        invenIndex[to] = invenIndex[from];
        invenIndex[from] = tempObj;
    }
    public void GetItems(T targetItem)
    {
        if (targetItem.itemName == string.Empty|| targetItem.itemName == null) return;
        GetEmptySlot = targetItem;
    }
    public void RemoveItem(T targetItem)
    {
        for (int i = 0; i < invenIndex.Length; i++)
        {
            if (invenIndex[i] == targetItem)
            {
                //invenIndex[i].ResetIMG();
                slotParent.GetChild(i).GetChild(0).GetComponent<InventorySlots>().SlotItem = null;
                invenIndex[i] = null;
                return;
            }
        }
    }
}
