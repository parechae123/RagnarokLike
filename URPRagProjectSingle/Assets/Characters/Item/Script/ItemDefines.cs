using PlayerDefines.Stat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public enum BasicStatTypes
{
    Str,
    AGI,
    Vit,
    Int,
    Dex,
    Luk,
}
public enum WeaponApixType
{
    CriticalDMG, CriticalChance, ATK, MATK, AttackSpeed, Casting
}
public enum ArmorApixType
{
    MaxMana, ManaRegen, MaxHp, HpRegen, MoveSpeed, Def, Evasion
}
public enum EquipPart
{
    Head, Chest, Pants, Boots, RightHand, LeftHand, TwoHanded
}
public enum WeaponType
{
    Shield,
    Sword,
    Bow,
    Cane/*지팡이*/ ,
    BluntWeapon/*둔기*/,
    Dagger
}
public struct WeaponApixes
{
    public (BasicStatTypes, float) firstLine;//Stat만
    public (WeaponApixType, float)[] abilityApixes;
}
public struct ArmorApixes
{
    public (BasicStatTypes, float) firstLine;//Stat만
    public (ArmorApixType, float)[] abilityApixes;
}
public interface IArmorBase : IItemBase
{
    EquipPart part
    {
        get;
    }
    WeaponApixes apixList
    {
        get;
    }
    //방어력
    float valueOne
    {
        get;
    }
    //특수효과 계수
    float weaponTypeValue
    {
        get;
    }
    float sellValue
    {
        get { return buyValue / 10f; }
    }
    float buyValue
    {
        get;
    }
}

public class Weapons : IItemBase
{
    private bool isEquipedWeapon = false;
    public event Action quickSlotFuncs;
    public Sprite IconIMG
    {
        get { return itemSpirte; }
    }
    Sprite itemSpirte;

    BaseJobType[] equipAbleJobs;

    byte equipLevel;
    public bool isItemUseAble
    {
        get
        {

            return (equipLevel <= Player.Instance.playerLevelInfo.baseLevel) && (CheckJob());
        }
    }

    public string slotNumberInfo
    {
        get { return string.Empty; }
    }
    public SlotType slotType { get { return SlotType.Equipments; } }
    public float BuyValue
    {
        get { return goldValue; }
    }
    public float SellValue
    {
        get { return goldValue / 10f; }
    }
    private float goldValue;
    EquipPart part;
    EquipPart Part
    {
        get { return part; }
    }
    WeaponApixes apixList;
    WeaponType weaponType;
    //ATK or MATK
    float valueOne;
    public float ValueOne
    {
        get { return valueOne; }
    }
    bool isMATKWeapon;
    bool IsMATKWeapon { get { return isMATKWeapon; } }
    //공속 계수 == 1.2 일시 캐릭터 공속* 1.2하여 공속이 느려짐 임시변수로 아직 미사용
    float TypeValue
    {
        get
        {
            switch (weaponType)
            {
                case WeaponType.Shield:
                    return 1.6f;
                case WeaponType.Sword:
                    return 1.2f;
                case WeaponType.Bow:
                    return 1.1f;
                case WeaponType.Cane:
                    return 1.8f;
                case WeaponType.BluntWeapon:
                    return 1.4f;
                case WeaponType.Dagger:
                    return 0.9f;
                default:
                    break;
            }
            return 1;
        }
    }

    public bool CheckJob()
    {
        for (int i = 0; i < equipAbleJobs.Length; i++)
        {
            if (equipAbleJobs[i] == Player.Instance.playerLevelInfo.stat.jobType) return true;
        }
        return false;
    }

    public Weapons(Sprite itemSprite, BaseJobType[] equipJobs, byte equipLevel, float goldValue, EquipPart part, WeaponApixes apixes, WeaponType weaponType, float valueOne,bool isMATKWeapon)
    {
        this.itemSpirte = itemSprite;
        this.equipAbleJobs = equipJobs;
        this.equipLevel = equipLevel;
        this.goldValue = goldValue;
        this.part = part;
        this.apixList = apixes;
        this.weaponType = weaponType;
        this.valueOne = valueOne;
        this.isMATKWeapon = isMATKWeapon; 
        quickSlotFuncs = null;
        quickSlotFuncs += UseItem;
    }
    public Weapons(EquipPart part)
    {
        this.part = part;
    }

    public void UseItem()
    {
        if (isItemUseAble)
        {
            if (isEquipedWeapon)
            {
                isEquipedWeapon = false;
            }
            else
            {
                isEquipedWeapon = true;
            }
        }
    }
}


