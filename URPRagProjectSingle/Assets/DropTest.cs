using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
            new GameObject("DropItems").AddComponent<DropItems>().InitialIzeItem(new Weapons("10001", "shield", GetComponent<SpriteRenderer>().sprite, new BaseJobType[1] { BaseJobType.Novice }, 0, 0, EquipPart.RightHand, 10, true, WeaponType.Shield,
                new IApixBase<WeaponApixType> { statLine = (BasicStatTypes.Str, 10), abilityApixes = new (WeaponApixType, float)[3] { (WeaponApixType.AttackSpeed, 0.3f), (WeaponApixType.CastingSpeed, 0.1f), (WeaponApixType.MATK, 0.3f) } }), transform.position);
        }
    }
    //TODO : ������ ������ �Ű������� �ް� �ٸ� �ɷ�ġ���� ����� �Ϸ��� ������ �߰��Ͽ� ���������� �����ϸ�ɵ�
    public int[] ApixSlotMachine(int apixCount,int apixTypeCount,bool sameAppixAllowed = false)
    {
        HashSet<int> slots = new HashSet<int>();
        int searchCount = 0;
        int[] answer = new int[apixCount];
        for (int i = 0; i < apixCount; i++)
        {
            searchCount++;
            int currApix = UnityEngine.Random.Range(0, apixTypeCount);
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
