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
    CriticalDMG, CriticalChance, ATK, MATK, AttackSpeed, CastingSpeed,MaxHP
}
public enum ArmorApixType
{
    MaxMana, ManaRegen, MaxHp, HpRegen, MoveSpeed, Def, Evasion
}
public enum EquipPart
{
    Head, Chest, Pants, Boots,Gauntlet, RightHand, LeftHand, TwoHanded
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
public enum ArmorMat
{
    Cloth,
    Leather,
    PlateArmor
}
/// <summary>
/// 
/// </summary>
/// <typeparam name="T">ApixEnumTypeOnly</typeparam>
public interface IApixBase<T> where T : Enum
{
    (BasicStatTypes, float) firstLine { get; set; }
    (T, float)[] abilityApixes
    {
        get;
        set;
    }
}
public interface IArmorBase : IItemBase
{
    EquipPart part
    {
        get;
    }
    IApixBase<ArmorApixType> apixList
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
public class inventoryItemBase : IItemBase
{
    public virtual event Action quickSlotFuncs;
    public virtual Sprite IconIMG
    {
        get { return itemSprite; }
    }
    public string itemName;
    protected Sprite itemSprite;
    protected sbyte amount;
    public virtual sbyte Amount
    {
        get { return amount; }
        set
        {
            if (value == 0)
            {
                ResetEvent();
                amount = value;
            }
            else
            {
                amount = value;
            }
        }
    }

    public virtual string slotNumberInfo
    {
        get { return Amount.ToString(); }
    }
    public virtual SlotType slotType
    {
        get { return SlotType.None; }
    }
    public virtual bool IsItemUseAble
    {
        get { return false; }
    }
    public virtual bool isStackAble
    {
        get { return true; }
    }
    public virtual bool IsEmptySlot
    {
        get { return Amount <= 0; }
    }
    public virtual void UseItem()
    {

    }
    protected void ResetEvent()
    {
        quickSlotFuncs = null;
    }
}
public class Equips : inventoryItemBase
{
    public bool isEquiped = false;
    public override event Action quickSlotFuncs;
    public override Sprite IconIMG
    {
        get { return itemSpirte; }
    }
    protected Sprite itemSpirte;
    public override bool isStackAble => false;

    protected BaseJobType[] equipAbleJobs;


    protected byte equipLevel;
    public bool isItemUseAble
    {
        get
        {

            return (equipLevel <= Player.Instance.playerLevelInfo.baseLevel)&&Amount>0 && (CheckJob());
        }
    }

    public override string slotNumberInfo
    {
        get { return string.Empty; }
    }
    public override SlotType slotType { get { return SlotType.Equipments; } }
    public float BuyValue
    {
        get { return goldValue; }
    }
    public float SellValue
    {
        get { return goldValue / 10f; }
    }
    protected float goldValue;
    protected EquipPart part;
    public EquipPart GetPart
    {
        get { return part; }
    }
    protected float valueOne;
    public float ValueOne
    {
        get { return valueOne; }
    }

    //공속 계수 == 1.2 일시 캐릭터 공속* 1.2하여 공속이 느려짐 임시변수로 아직 미사용
    protected virtual float TypeValue
    {
        get
        {
            return 1;
        }
    }

    public bool CheckJob()
    {
        if (equipAbleJobs.Length <= 0) return false;
        for (int i = 0; i < equipAbleJobs.Length; i++)
        {
            if (equipAbleJobs[i] == Player.Instance.playerLevelInfo.stat.jobType) return true;
        }
        return false;
    }
    /// <summary>
    /// CreateEmptySlot
    /// </summary>
    public Equips()
    {
        Amount = 0;
    }
    public Equips(Sprite itemSprite, BaseJobType[] equipJobs, byte equipLevel, float goldValue, EquipPart part, float valueOne)
    {
        Amount = 1;
        this.itemSpirte = itemSprite;
        this.equipAbleJobs = equipJobs;
        this.equipLevel = equipLevel;
        this.goldValue = goldValue;
        this.part = part;
        this.valueOne = valueOne;
        quickSlotFuncs = null;
        quickSlotFuncs += UseItem;
    }
    public Equips(EquipPart part)
    {
        this.part = part;
        equipAbleJobs = new BaseJobType[1] { BaseJobType.None };
    }

    public override void UseItem()
    {
        if (isItemUseAble)
        {
            Amount--;
            if (isEquiped&&isItemUseAble)
            {
                Debug.Log("ㅇㅇㅇ");
                isEquiped = false;
            }
            else
            {
                isEquiped = true;
            }
        }
    }
}

public class Armors : Equips
{
    IApixBase<ArmorApixType> apixList;
    ArmorMat matType;
    
    public Armors() : base()
    {

    }
    public Armors(EquipPart part) : base(part)
    {
        this.part = part;
    }
    public WeaponApixType GetValueType
    {
        get
        {
            switch (matType)
            {
                case ArmorMat.Cloth:
                    return WeaponApixType.CastingSpeed;
                case ArmorMat.Leather:
                    return WeaponApixType.CriticalDMG;
                case ArmorMat.PlateArmor:
                    return WeaponApixType.MaxHP;
                default:
                    return WeaponApixType.AttackSpeed;
            }
        }
    }
    public Armors(Sprite itemSprite, BaseJobType[] equipJobs, byte equipLevel, float goldValue, EquipPart part, float valueOne,IApixBase<ArmorApixType> apixes,ArmorMat armorMat) : base(itemSprite, equipJobs, equipLevel, goldValue, part, valueOne)
    {
        Amount = 1;
        this.itemSpirte = itemSprite;
        this.equipAbleJobs = equipJobs;
        this.equipLevel = equipLevel;
        this.goldValue = goldValue;
        this.part = part;
        this.valueOne = valueOne;
        this.apixList = apixes;
        this.matType = armorMat;
        ResetEvent();
        quickSlotFuncs += UseItem;
    }
    protected override float TypeValue
    {
        get
        {
            switch (matType)
            {
                case ArmorMat.Cloth:
                    return 0.05f;
                case ArmorMat.Leather:
                    return 0.04f;
                case ArmorMat.PlateArmor:
                    return 0.02f;
                default:
                    break;
            }
            return 0;
        }
    }

}


public class Weapons : Equips
{
    WeaponType weaponType;
    IApixBase<WeaponApixType> apixList;
    bool isMATKWeapon;
    bool IsMATKWeapon { get { return isMATKWeapon; } }
    public Weapons() : base()
    {

    }
    public Weapons(EquipPart equipPart) : base(equipPart)
    {
        this.part = equipPart;
    }
    protected override float TypeValue
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
    
    public Weapons(Sprite itemSprite, BaseJobType[] equipJobs, byte equipLevel, float goldValue, EquipPart part, float valueOne, bool isMATKWeapon,WeaponType weaponType) : base(itemSprite,equipJobs, equipLevel, goldValue,part,valueOne)
    {
        Amount = 1;
        this.itemSpirte = itemSprite;
        this.equipAbleJobs = equipJobs;
        this.equipLevel = equipLevel;
        this.goldValue = goldValue;
        this.part = part;
        this.weaponType = weaponType;
        this.valueOne = valueOne;
        this.isMATKWeapon = isMATKWeapon;
        ResetEvent();
        quickSlotFuncs += UseItem;
    }
}




