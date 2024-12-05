using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DropManaging
{
    public Queue<DropItems> items = new Queue<DropItems>();
    public void JustDropItem(InventoryItemBase item,Vector3 pos)
    {
        if (items.Count > 0)
        {
            items.Dequeue().InitialIzeItem(item,pos);
        }
        else
        {
            new GameObject("DropItems").AddComponent<DropItems>().InitialIzeItem(item,pos);
        }
    }
    public void SpawnEquipItem(Vector3 position,byte level,bool isWeaponOnly= false,bool isArmorOnly = false)
    {
        ApixSlotMachine(3, Enum.GetValues(typeof(WeaponApixType)).Length, false);
        if(items.Count > 0)
        {
            items.Dequeue().InitialIzeItem(AssembleEquips(level,isWeaponOnly,isArmorOnly), position);
        }
        else
        {
            new GameObject("DropItems").AddComponent<DropItems>().InitialIzeItem(AssembleEquips(level, isWeaponOnly, isArmorOnly), position);
        }
    }
    /// <summary>
    /// 사용 시 아이템 코드 확인 요망
    /// </summary>
    /// <param name="position"></param>
    /// <param name="item"></param>
    public void SpawnMisc(Vector3 position,MiscData item,bool drop = true)
    {
        if ((item == null)) return;
        ApixSlotMachine(3, Enum.GetValues(typeof(WeaponApixType)).Length, false);
        if (drop) 
        {
            if (items.Count > 0)
            {
                items.Dequeue().InitialIzeItem(new Miscs(item.itemCode.ToString(), item.itemName, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(item.iconName), item.goldValue), position);
            }
            else
            {
                new GameObject("DropItems").AddComponent<DropItems>().InitialIzeItem(new Miscs(item.itemCode.ToString(), item.itemName, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(item.iconName), item.goldValue), position);
            }
        }
        else
        {
            
        }
    }
    /// <summary>
    /// 사용 시 아이템 코드 확인 요망
    /// </summary>
    /// <param name="position"></param>
    /// <param name="item"></param>
    public void SpawnCosume(Vector3 position,PosionData item)
    {
        ApixSlotMachine(3, Enum.GetValues(typeof(WeaponApixType)).Length, false);
        if(items.Count > 0)
        {
            items.Dequeue().InitialIzeItem(new Potions(item.itemCode.ToString(),item.itemName,ResourceManager.GetInstance().ItemIconAtlas.GetSprite(item.iconName),item.goldValue,item.potionType,item.goldValue), position);
        }
        else
        {
            new GameObject("DropItems").AddComponent<DropItems>().InitialIzeItem(new Potions(item.itemCode.ToString(), item.itemName, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(item.iconName), item.goldValue, item.potionType, item.goldValue), position);
        }
    }

    public Weapons GetDefinedWeapon(sbyte itemLevel, bool isTwohanded,WeaponType type) 
    {
        EquipPart itemPart = !isTwohanded ? EquipPart.RightHand: EquipPart.TwoHanded;
        if (isTwohanded) itemPart = EquipPart.TwoHanded;
        if (type == WeaponType.Shield) itemPart = EquipPart.LeftHand;
        else if (type == WeaponType.Bow) itemPart = EquipPart.TwoHanded;

        BaseJobType[] jobs = GetEquipAbleJobs(type);
        int statType = Random(0, Enum.GetValues(typeof(BasicStatTypes)).Length);
        int apixCount = Random(0, 4);
        ApixesData datas = ResourceManager.GetInstance().ApixDatas.items[itemLevel];
        string name = string.Empty;
        IApixBase<WeaponApixType> apix = new IApixBase<WeaponApixType>
        {
            statLine = (datas.statApixes[statType].apixType, Random((int)datas.statApixes[statType].minValue, (int)datas.statApixes[statType].maxValue)),
            abilityApixes = new (WeaponApixType, float)[apixCount]
        };
        int[] weaponApixes = ApixSlotMachine(apixCount, Enum.GetValues(typeof(WeaponApixType)).Length, false);
        for (int i = 0; i < apixCount; i++)
        {
            apix.abilityApixes[i] = ((WeaponApixType)weaponApixes[i], Random(datas.weaponApixes[weaponApixes[i]].minValue, datas.weaponApixes[weaponApixes[i]].maxValue));
        }
        if (apix.abilityApixes.Length != 0) name += ResourceManager.GetInstance().NameSheet.GetApixNameValue(apix.abilityApixes[0].Item1.ToString()) + ' ';
        name += ResourceManager.GetInstance().NameSheet.GetApixNameValue(apix.statLine.Item1.ToString()) + ' ';
        name += ResourceManager.GetInstance().NameSheet.GetLevelNameValue(itemLevel) + ' ';
        name += ResourceManager.GetInstance().NameSheet.GetEquipNameValue(itemPart.ToString()) + ' ';
        name += ResourceManager.GetInstance().NameSheet.GetEquipNameValue(type.ToString());
        Debug.Log(name);
        return new Weapons(type.ToString(), name, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(itemPart.ToString()), jobs, itemLevel, itemLevel * 800f, itemPart,
            Random(ResourceManager.GetInstance().ApixDatas.items[itemLevel].weaponMinValue, ResourceManager.GetInstance().ApixDatas.items[itemLevel].weaponMaxValue),
            type == WeaponType.Cane, type, apix);
    }
    public Armors GetDefinedArmor(sbyte itemLevel, ArmorMat mat,EquipPart part) 
    {
        ApixesData datas = ResourceManager.GetInstance().ApixDatas.items[itemLevel];
        string name = string.Empty;
        int statType = Random(0, Enum.GetValues(typeof(BasicStatTypes)).Length);
        int apixCount = Random(0, 4);
        IApixBase<ArmorApixType> apix = new IApixBase<ArmorApixType>
        {
            statLine = (datas.statApixes[statType].apixType, Random((int)datas.statApixes[statType].minValue, (int)datas.statApixes[statType].maxValue)),
            abilityApixes = new (ArmorApixType, float)[apixCount]
        };
        int[] armorApixes = ApixSlotMachine(apixCount, Enum.GetValues(typeof(ArmorApixType)).Length, false);
        for (int i = 0; i < apixCount; i++)
        {
            apix.abilityApixes[i] = ((ArmorApixType)armorApixes[i], Random(datas.armorApixes[armorApixes[i]].minValue, datas.armorApixes[armorApixes[i]].maxValue));
        }
        BaseJobType[] jobs = new BaseJobType[Enum.GetValues(typeof(BaseJobType)).Length];
        for (int i = 0; i < jobs.Length; i++)
        {
            jobs[i] = (BaseJobType)i;
        }
        if (apix.abilityApixes.Length != 0) name += ResourceManager.GetInstance().NameSheet.GetApixNameValue(apix.abilityApixes[0].Item1.ToString()) + ' ';
        name += ResourceManager.GetInstance().NameSheet.GetApixNameValue(apix.statLine.Item1.ToString()) + ' ';
        name += ResourceManager.GetInstance().NameSheet.GetLevelNameValue(itemLevel) + ' ';
        name += ResourceManager.GetInstance().NameSheet.GetEquipNameValue(mat.ToString() + part.ToString());
        Debug.Log(name);
        return new Armors(mat.ToString() + part.ToString(), name, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(part.ToString()), jobs, itemLevel, itemLevel * 800f, part,
            Random(ResourceManager.GetInstance().ApixDatas.items[itemLevel].armorMinValue, ResourceManager.GetInstance().ApixDatas.items[itemLevel].armorMaxValue),
              apix, mat, mat == ArmorMat.Cloth);
    }
    public Equips AssembleEquips(byte level,bool weaponOnly,bool armorOnly)
    {
        EquipPart itemPart;
        if (weaponOnly)
        {
            itemPart = (EquipPart)Random(5, (int)EquipPart.TwoHanded);
        }
        else if(armorOnly)
        {
            itemPart = (EquipPart)Random(0, (int)EquipPart.Gauntlet);
        }
        else
        {
            itemPart = (EquipPart)Random(0, Enum.GetValues(typeof(EquipPart)).Length);
        }

        
        //무기 종류에 속하는 장비일 경우
        int itemLevel = level / 5;

        if (itemPart == EquipPart.LeftHand|| itemPart == EquipPart.RightHand || itemPart == EquipPart.TwoHanded)
        {
            WeaponType weaponType = (WeaponType)Random(0, Enum.GetValues(typeof(WeaponType)).Length);
            return GetDefinedWeapon((sbyte)itemLevel,itemPart== EquipPart.TwoHanded, weaponType);
        }
        else
        {
            ArmorMat armorType = (ArmorMat)Random(0, Enum.GetValues(typeof(ArmorMat)).Length-1);
            return GetDefinedArmor((sbyte)itemLevel,armorType,itemPart);
        }
    }
    public BaseJobType[] GetEquipAbleJobs(WeaponType type)
    {
        BaseJobType[] jobs = new BaseJobType[0];
        switch (type)
        {
            case WeaponType.Shield:
                jobs = new BaseJobType[2];
                jobs[0] = BaseJobType.Novice;
                jobs[1] = BaseJobType.SwordMan;
                break;
            case WeaponType.Sword:
                jobs = new BaseJobType[2];
                jobs[0] = BaseJobType.Novice;
                jobs[1] = BaseJobType.SwordMan;
                break;
            case WeaponType.Bow:
                jobs = new BaseJobType[1];
                jobs[0] = BaseJobType.Archer;
                break;
            case WeaponType.Cane:
                jobs = new BaseJobType[1];
                jobs[0] = BaseJobType.Mage;
                break;
            case WeaponType.BluntWeapon:
                jobs = new BaseJobType[1];
                jobs[0] = BaseJobType.SwordMan;
                break;
            case WeaponType.Dagger:
                jobs = new BaseJobType[3];
                jobs[0] = BaseJobType.Novice;
                jobs[1] = BaseJobType.Archer;
                jobs[2] = BaseJobType.Mage;
                break;
        }
        return jobs;
    }

    public int Random(int min,int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public float Random(float min,float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public int[] ApixSlotMachine(int apixCount,int apixTypeCount,bool sameAppixAllowed = false)
    {
        HashSet<int> slots = new HashSet<int>();
        int searchCount = 0;
        int[] answer = new int[apixCount];
        for (int i = 0; i < apixCount; i++)
        {
            searchCount++;
            int currApix = Random(0, apixTypeCount);
            if( slots.Contains(currApix)&&!sameAppixAllowed)
            {
                if(searchCount > 50|| apixCount>= apixTypeCount) sameAppixAllowed = true;
                i--;
                continue;
            }
            slots.Add(currApix);
            answer[i] = currApix;
        }
        return answer;
    }
}
