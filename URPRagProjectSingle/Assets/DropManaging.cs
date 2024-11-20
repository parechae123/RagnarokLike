using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManaging
{
    public Queue<DropItems> items = new Queue<DropItems>();
    public void SpawnEquipItem(Vector3 position,byte level)
    {
        ApixSlotMachine(3, Enum.GetValues(typeof(WeaponApixType)).Length, false);
        if(items.Count > 0)
        {
            items.Dequeue().InitialIzeItem(AssembleEquips(level), position);
        }
        else
        {
            new GameObject("DropItems").AddComponent<DropItems>().InitialIzeItem(AssembleEquips(level), position);
        }
    }
    /// <summary>
    /// 사용 시 아이템 코드 확인 요망
    /// </summary>
    /// <param name="position"></param>
    /// <param name="item"></param>
    public void SpawnMisc(Vector3 position,MiscData item)
    {
        ApixSlotMachine(3, Enum.GetValues(typeof(WeaponApixType)).Length, false);
        if(items.Count > 0)
        {
            items.Dequeue().InitialIzeItem(new Miscs(item.itemCode.ToString(),item.itemName,ResourceManager.GetInstance().ItemIconAtlas.GetSprite(item.iconName),item.goldValue), position);
        }
        else
        {
            new GameObject("DropItems").AddComponent<DropItems>().InitialIzeItem(new Miscs(item.itemCode.ToString(), item.itemName, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(item.iconName), item.goldValue), position);
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

    public Equips AssembleEquips(byte level)
    {
        EquipPart itemPart = (EquipPart)Random(0, Enum.GetValues(typeof(EquipPart)).Length);
        //무기 종류에 속하는 장비일 경우
        int itemLevel = level / 5;
        ApixesData datas = ResourceManager.GetInstance().ApixDatas.items[itemLevel];
        string name = string.Empty;
        if (itemPart == EquipPart.LeftHand|| itemPart == EquipPart.RightHand || itemPart == EquipPart.TwoHanded)
        {
            
            WeaponType weaponType = (WeaponType)Random(0, Enum.GetValues(typeof(WeaponType)).Length);
            if (weaponType == WeaponType.Shield) itemPart = EquipPart.LeftHand;
            else if(weaponType != WeaponType.Shield&& itemPart != EquipPart.TwoHanded)
            {
                itemPart = EquipPart.RightHand;
            }

            if (weaponType == WeaponType.Bow) itemPart = EquipPart.TwoHanded;

            BaseJobType[] jobs = GetEquipAbleJobs(weaponType);
            int statType = Random(0, Enum.GetValues(typeof(BasicStatTypes)).Length);
            int apixCount = Random(0, 4);
            IApixBase<WeaponApixType> apix = new IApixBase<WeaponApixType> 
            { 
                statLine = (datas.statApixes[statType].apixType, Random((int)datas.statApixes[statType].minValue, (int)datas.statApixes[statType].maxValue)),
                abilityApixes = new (WeaponApixType, float)[apixCount]
            };
            int[] weaponApixes = ApixSlotMachine(apixCount,Enum.GetValues(typeof(WeaponApixType)).Length,false);
            for (int i = 0; i< apixCount; i++)
            {
                apix.abilityApixes[i] = ((WeaponApixType)weaponApixes[i], Random(datas.weaponApixes[weaponApixes[i]].minValue, datas.weaponApixes[weaponApixes[i]].maxValue));
            }
            if (apix.abilityApixes.Length != 0) name += ResourceManager.GetInstance().NameSheet.GetApixNameValue(apix.abilityApixes[0].Item1.ToString()) + ' ';
            name += ResourceManager.GetInstance().NameSheet.GetApixNameValue(apix.statLine.Item1.ToString()) + ' ';
            name += ResourceManager.GetInstance().NameSheet.GetLevelNameValue(itemLevel)+' ';
            name += ResourceManager.GetInstance().NameSheet.GetEquipNameValue(itemPart.ToString()) + ' ';
            name += ResourceManager.GetInstance().NameSheet.GetEquipNameValue(weaponType.ToString());
            Debug.Log(name);
            return new Weapons(weaponType.ToString(), name, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(itemPart.ToString()), jobs, level, level * 800f, itemPart,
                Random(ResourceManager.GetInstance().ApixDatas.items[itemLevel].weaponMinValue, ResourceManager.GetInstance().ApixDatas.items[itemLevel].weaponMaxValue),
                weaponType == WeaponType.Cane, weaponType, apix);
        }
        else
        {
            ArmorMat armorType = (ArmorMat)Random(0, Enum.GetValues(typeof(ArmorMat)).Length-1);
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
            if(apix.abilityApixes.Length != 0) name += ResourceManager.GetInstance().NameSheet.GetApixNameValue(apix.abilityApixes[0].Item1.ToString()) + ' ';
            name += ResourceManager.GetInstance().NameSheet.GetApixNameValue(apix.statLine.Item1.ToString()) + ' ';
            name += ResourceManager.GetInstance().NameSheet.GetLevelNameValue(itemLevel) + ' ';
            name += ResourceManager.GetInstance().NameSheet.GetEquipNameValue(armorType.ToString() + itemPart.ToString());
            Debug.Log(name);
            return new Armors(armorType.ToString(), name, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(itemPart.ToString()),jobs ,  level, level * 800f, itemPart,
                Random(ResourceManager.GetInstance().ApixDatas.items[itemLevel].armorMinValue, ResourceManager.GetInstance().ApixDatas.items[itemLevel].armorMaxValue),
                  apix, armorType, armorType == ArmorMat.Cloth);
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
