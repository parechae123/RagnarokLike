using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

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
                    return;
                }
                if (invenIndex[i].IsEmptySlot)
                {
                    invenIndex[i] = value;
                    invenIndex[i].Amount = value.Amount;
                    slotParent.GetChild(i).GetChild(0).GetComponent<InventorySlots>().SlotItem = value;
                    return;
                }
                else if(invenIndex[i].isStackAble&&value.isStackAble)
                {
                    //TODO : ���� ���������� �˻��� ��Ҹ� �־���� ��Ұ˻� VS ������ �ڵ�˻� �� �� �ϳ� �ؾ��ҵ�
                    if (invenIndex[i].itemCode == value.itemCode) 
                    {
                        int temp = (int)invenIndex[i].Amount + (int)value.Amount;
                        if (temp > 0 && temp <= sbyte.MaxValue)
                        {
                            invenIndex[i].Amount = (sbyte)temp;
                            slotParent.GetChild(i).GetChild(0).GetComponent<InventorySlots>().SlotItem = invenIndex[i];
                            return;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            MonsterManager.GetInstance().Drop.JustDropItem(value, Player.Instance.transform.position);
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
        if (targetItem.itemName == string.Empty|| targetItem.itemName == null||targetItem == null) return;
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
    public bool RemoveItem(string itemCode,sbyte amount)
    {
        T[] tempArray = Array.FindAll(invenIndex, inven => inven.itemCode == itemCode);
        if (tempArray.Sum(item => item.Amount) < amount) return false;
        for(int i = 0; i< tempArray.Length; i++)
        {
            if (tempArray[i].Amount < amount)
            {
                amount -= tempArray[i].Amount;
                invenIndex[i] = null;
                continue;
            }
            else
            {
                tempArray[i].Amount -= amount;
                if (tempArray[i].Amount <= 0) invenIndex[i] = null;
                return true;
            }
        }
        return false;
    }
    public int GetAmount(string itemCode)
    {
        //������ ������ �ڵ带 ã�� �� ������ Amount�� ���� ��������
        return Array.FindAll(invenIndex,inven => inven.itemCode == itemCode).Sum((amount) => amount.Amount);
    }
}
