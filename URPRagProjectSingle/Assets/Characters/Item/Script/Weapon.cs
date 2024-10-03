using PlayerDefines.Stat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public struct Affix
{
    public int affixValue;
    public ItemEnums.Stats stat;
}
public class ItemEnums
{
    //������ Ÿ��
    public enum ItemTypes
    {
        Equip,
        Consumable
    }

    //��� ����
    public enum EquipParts
    {
        Head,
        Chest,
        Pants,
        Boots,                                  
        Weapons,
        Shield,
        Accessory
    }

    //���� Ÿ��
    public enum WeaponTypes
    {
        Sword,      //��
        Bow,        //Ȱ
        Cane        //������
    }

    //�� Ÿ��
    public enum ArmorTypes
    {
        Robe,       //�κ�
        Plate       //�Ǳ�
    }

    public enum Stats
    {
        Hp,
        Mp,
        Str,
        Int,
        Dex,
        Luk,
    }
}
public class Weapon : ItemBase
{
    Affix[] affixes = new Affix[3];

    public event Action quickSlotFuncs;
        
    public string ItemName { get; private set; }
    public int ItemID { get; private set; }
    public string ItemType { get; private set; }
    public string ItemEffect { get; private set; }
    public string ItemAffixes { get; private set; }
    public ItemEnums.WeaponTypes WeaponType { get; private set; }
    public ItemEnums.EquipParts EquipPart { get; private set; }

    
    public Weapon(string name, int id, string itemtype, string effect, string affixes, ItemEnums.WeaponTypes weaponType, ItemEnums.EquipParts equipPart)
    {
        ItemName = name;
        ItemID = id;
        ItemType = itemtype;
        ItemAffixes = affixes;
        ItemEffect = effect;
        WeaponType = weaponType;
        EquipPart = equipPart;
    }

    public void UseItem()
    {
        Debug.Log($"{ItemName}�� �����Ͽ����ϴ�");
        //������ ���� �޼ҵ� 
        //������ ���� �޼ҵ忡�� ĳ���Ϳ��� ������ �������ִ� ����� �־��ش�

    }
}


