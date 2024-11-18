using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApixSlotMachine(3, Enum.GetValues(typeof(WeaponApixType)).Length, false);
            new GameObject("DropItems").AddComponent<DropItems>().InitialIzeItem(AssembleEquips(0),transform.position);
        }
    }

    public Equips AssembleEquips(byte level)
    {
        EquipPart itemPart = (EquipPart)Random(0, Enum.GetValues(typeof(EquipPart)).Length);
        //무기 종류에 속하는 장비일 경우
        ApixesData datas = ResourceManager.GetInstance().ApixDatas.items[level / 5];
        if (itemPart == EquipPart.LeftHand|| itemPart == EquipPart.RightHand || itemPart == EquipPart.TwoHanded)
        {
            WeaponType weaponType = (WeaponType)Random(0, Enum.GetValues(typeof(WeaponType)).Length);
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

            return new Weapons(weaponType.ToString(), weaponType.ToString(), ResourceManager.GetInstance().ItemIconAtlas.GetSprite(itemPart.ToString()), jobs, level, level * 800f, itemPart,
                Random(ResourceManager.GetInstance().ApixDatas.items[level / 5].weaponMinValue, ResourceManager.GetInstance().ApixDatas.items[level / 5].weaponMaxValue),
                weaponType == WeaponType.Cane, weaponType, apix);
        }
        else
        {
            ArmorMat armorType = (ArmorMat)Random(0, Enum.GetValues(typeof(ArmorMat)).Length);
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

            return new Armors(armorType.ToString(), armorType.ToString(), ResourceManager.GetInstance().ItemIconAtlas.GetSprite(itemPart.ToString()),jobs ,  level, level * 800f, itemPart,
                Random(ResourceManager.GetInstance().ApixDatas.items[level / 5].armorMinValue, ResourceManager.GetInstance().ApixDatas.items[level / 5].armorMaxValue),
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
