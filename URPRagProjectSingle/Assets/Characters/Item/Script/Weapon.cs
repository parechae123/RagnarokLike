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
    public ItemEnums.BasicStatTypes stat;
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

    public enum BasicStatTypes
    {
        Str,
        AGI,
        Vit,
        Dex,
        Int,
        Luk,
    }
}
public class Weapon : IitemBase, IEquipBase
{
    Affix[] affixes = new Affix[3];

    public event Action quickSlotFuncs;


    public EquipStat equipStat {  get; private set; }


    public bool onEquip;

    public Sprite IconIMG
    {
        get;
    }
    public string slotNumberInfo
    {
        get;
    }
    public SlotType slotType
    {
        get { return SlotType.Equipments; }
    }
    public bool isItemUseAble
    {
        get;
    }


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
        
        //������ ���� �޼ҵ� 
        //������ ���� �޼ҵ忡�� ĳ���Ϳ��� ������ �������ִ� ����� �־��ش�
        if (onEquip)
        {
            Debug.Log($"{ItemName}�� �����Ͽ����ϴ�");
            return;
        }
       /* Player.Instance.playerLevelInfo.stat.HP += equipStat.EquipHp;
        Player.Instance.playerLevelInfo.stat.moveSpeed += equipStat.EquipMoveSpeed;
        Player.Instance.playerLevelInfo.stat.attackDamage += equipStat.EquipAttackDamage;
        Player.Instance.playerLevelInfo.stat.attackSpeed += equipStat.EquipAttackSpeed;
        Player.Instance.playerLevelInfo.stat.charactorAttackRange += equipStat.EquipAttackRange;*/
        
    }
}

interface IEquipBase
{
    EquipStat equipStat { get; }
}


public class EquipStat
{
    ItemEnums.EquipParts partsCheck;   //��� ���� �������� Ȯ��
    public float EquipHp { get; set; }
    public float EquipSp { get; set; }
    private float equipMoveSpeed { get; set; }
    public float EquipMoveSpeed { get { return equipMoveSpeed; } }
    public float EquipAbilityPower { get; set; }
    public float EquipAttackDamage { get; set; }
    public float EquipAttackSpeed { get; set; }
    public byte EquipAttackRange { get; set; }

    public EquipStat(float hp, float sp, float moveSpeed, float attackDamage, float attackSpeed, byte attackRange)
    {

        EquipHp = hp;
        EquipSp = sp;
        equipMoveSpeed = moveSpeed;
        EquipAttackDamage = attackDamage;
        EquipAttackSpeed = attackSpeed;
        EquipAttackRange = attackRange;
    }

}