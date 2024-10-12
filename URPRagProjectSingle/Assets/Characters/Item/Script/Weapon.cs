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
    //아이템 타입
    public enum ItemTypes
    {
        Equip,
        Consumable
    }

    //장비 부위
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

    //무기 타입
    public enum WeaponTypes
    {
        Sword,      //검
        Bow,        //활
        Cane        //지팡이
    }

    //방어구 타입
    public enum ArmorTypes
    {
        Robe,       //로브
        Plate       //판금
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
        
        //아이템 장착 메소드 
        //아이템 장착 메소드에는 캐릭터에게 스탯을 이전해주는 기능을 넣어준다
        if (onEquip)
        {
            Debug.Log($"{ItemName}을 장착하였습니다");
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
    ItemEnums.EquipParts partsCheck;   //어느 부위 파츠인지 확인
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