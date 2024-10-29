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
    CriticalDMG, CriticalChance, ATK, MATK, AttackSpeed, CastingSpeed,MaxHP, Accuracy
}
public enum ArmorApixType
{
    //완료,   완료,     완료,     완료  완료      미완  완료
    MaxMana, ManaRegen, MaxHp, HpRegen, MoveSpeed, deff, Evasion, magicDeff
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
    PlateArmor,
    None
}
/// <summary>
/// 
/// </summary>
/// <typeparam name="T">ApixEnumTypeOnly</typeparam>
public struct IApixBase<T> where T : Enum
{
    public (BasicStatTypes, int) firstLine;
    public (T, float)[] abilityApixes; 
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
public class InventoryItemBase : IItemBase
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
    public void ResetIMG()
    {
        itemSprite = null;
    }
}
public class Equips : InventoryItemBase
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
    public Equips(string itemName,Sprite itemSprite, BaseJobType[] equipJobs, byte equipLevel, float goldValue, EquipPart part, float valueOne)
    {
        this.itemName = itemName;
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
            Amount = 0;
            if (isEquiped)
            {
                Debug.Log($"{itemName}장착해제");
                Amount = 1;
                isEquiped = false;
            }
            else
            {
                Debug.Log($"{itemName}장착");

                isEquiped = true;
            }
        }
    }
}

public class Armors : Equips
{
    public IApixBase<ArmorApixType> apixList;
    ArmorMat matType;
    public bool magicDeff;
    public Armors() : base()
    {

    }
    public Armors(EquipPart part) : base(part)
    {
        this.part = part;
        this.matType = ArmorMat.None;
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
    public Armors(string itemName,Sprite itemSprite, BaseJobType[] equipJobs, byte equipLevel, float goldValue, EquipPart part, float valueOne,IApixBase<ArmorApixType> apixes,ArmorMat armorMat,bool isMagicDeff) : base(itemName,itemSprite, equipJobs, equipLevel, goldValue, part, valueOne)
    {
        this.itemName = itemName;
        Amount = 1;
        this.itemSpirte = itemSprite;
        this.equipAbleJobs = equipJobs;
        this.equipLevel = equipLevel;
        this.goldValue = goldValue;
        this.part = part;
        this.valueOne = valueOne;
        this.apixList = apixes;
        this.matType = armorMat;
        magicDeff = isMagicDeff;
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
                    return 0.02f;
                case ArmorMat.Leather:
                    return 0.025f;
                case ArmorMat.PlateArmor:
                    return 0.06f;
                default:
                    break;
            }
            return 0;
        }
    }
    public float GetMatValue
    {
        get { return TypeValue; }
    }
    public override void UseItem()
    {
        if (isItemUseAble)
        {
            if (isEquiped)
            {
                Debug.Log($"{itemName}장착해제");
                Amount = 1;
                isEquiped = false;

                switch (part)
                {
                    case EquipPart.Head:
                        UIManager.GetInstance().equipWindowArmors[0].SlotItem = new Armors(part);
                        break;
                    case EquipPart.Chest:
                        UIManager.GetInstance().equipWindowArmors[1].SlotItem = new Armors(part);
                        break;
                    case EquipPart.Pants:
                        UIManager.GetInstance().equipWindowArmors[2].SlotItem = new Armors(part);
                        break;
                    case EquipPart.Boots:
                        UIManager.GetInstance().equipWindowArmors[3].SlotItem = new Armors(part);
                        break;
                    case EquipPart.Gauntlet:
                        UIManager.GetInstance().equipWindowArmors[4].SlotItem = new Armors(part);
                        break;
                    default:
                        break;
                }
                Player.Instance.playerLevelInfo.stat.GetArmorSlot = new Armors(part);
                return;
            }
            else
            {
                Debug.Log($"{itemName}장착");
                Player.Instance.playerLevelInfo.stat.GetArmorSlot = this;
                isEquiped = true;
            }
        }
    }
}


public class Weapons : Equips
{
    WeaponType weaponType;
    public IApixBase<WeaponApixType> apixList;
    bool isMATKWeapon;
    public bool IsMATKWeapon { get { return isMATKWeapon; } }
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
    
    public Weapons(string itemName, Sprite itemSprite, BaseJobType[] equipJobs, byte equipLevel, float goldValue, EquipPart part, float valueOne, bool isMATKWeapon,WeaponType weaponType, IApixBase<WeaponApixType> apix) : base(itemName, itemSprite,equipJobs, equipLevel, goldValue,part,valueOne)
    {
        this.itemName = itemName;
        Amount = 1;
        this.itemSpirte = itemSprite;
        this.equipAbleJobs = equipJobs;
        this.equipLevel = equipLevel;
        this.goldValue = goldValue;
        this.part = part;
        this.weaponType = weaponType;
        this.valueOne = valueOne;
        this.isMATKWeapon = isMATKWeapon;
        apixList = apix;
        ResetEvent();
        quickSlotFuncs += UseItem;
    }
    public override void UseItem()
    {
        //슬롯 전이시 해당 함수가 안넘어감
        if (isItemUseAble)
        {

            if (isEquiped)
            {
                Debug.Log($"{itemName}장착해제");
                Amount = 1;
                isEquiped = false;
                if (part == EquipPart.LeftHand)
                {
                    UIManager.GetInstance().equipWindowWeapons[0].SlotItem = new Weapons(part);
                }
                else
                {
                    UIManager.GetInstance().equipWindowWeapons[1].SlotItem = new Weapons(part);
                }
                Player.Instance.playerLevelInfo.stat.GetWeaponSlot = new Weapons(part);
                return;
            }
            else
            {
                Debug.Log($"{itemName}장착");
                Player.Instance.playerLevelInfo.stat.GetWeaponSlot = this;
                isEquiped = true;
            }
        }
    }
}

public class Consumables : InventoryItemBase
{
    //Amount ==0일시 아이템 제거되도록 구현 필요
}
public class Potions : Consumables
{

}
public class foods : Consumables
{

}
public class buffItems : Consumables
{
    //버프시스템 구현 필요한데 이건 준교수님께 여쭤봐야할듯
}


