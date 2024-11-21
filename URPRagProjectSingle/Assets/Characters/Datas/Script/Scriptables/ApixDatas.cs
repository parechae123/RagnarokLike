#region ApixDatas
using Newtonsoft.Json;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;
using System;

[System.Serializable]
public class ApixesData
{
    public byte level;
    public ApixRange<WeaponApixType>[] weaponApixes;
    public ApixRange<ArmorApixType>[] armorApixes;
    public ApixRange<BasicStatTypes>[] statApixes;
    public float armorMaxValue;
    public float armorMinValue;
    public float weaponMaxValue;
    public float weaponMinValue;
}

[CreateAssetMenu(fileName = "Apixes", menuName = "Items/Apixes", order = 0)]
public class ApixDatas : ScriptableObject
{
    [SerializeField] public ApixesData[] items;
    public void GetSheetValue()
    {
        items = new ApixesData[20];
        byte levelMulti = 5;
        for (byte i = 0; i< items.Length; i++)
        {
            items[i] = new ApixesData();
            items[i].level = (byte)(levelMulti * i);
            items[i].weaponMinValue = items[i].level * 4;
            items[i].weaponMaxValue = items[i].level * 6;
            items[i].armorMinValue = items[i].level * 6;
            items[i].armorMaxValue = items[i].level * 12;
            items[i].weaponApixes = new ApixRange<WeaponApixType>[Enum.GetValues(typeof(WeaponApixType)).Length];
            for (int j = 0; j < items[i].weaponApixes.Length; j++)
            {
                items[i].weaponApixes[j].apixType = (WeaponApixType)j;
                switch (items[i].weaponApixes[j].apixType)
                {
                    case WeaponApixType.CriticalDMG:
                        items[i].weaponApixes[j].minValue = 0.5f * items[i].level;  // CriticalDMG 최소값
                        items[i].weaponApixes[j].maxValue = 1.2f * items[i].level;  // CriticalDMG 최대값
                        break;

                    case WeaponApixType.CriticalChance:
                        items[i].weaponApixes[j].minValue = 0.5f * items[i].level; // Critical Chance 최소값
                        items[i].weaponApixes[j].maxValue = 0.2f * items[i].level;  // Critical Chance 최대값
                        break;

                    case WeaponApixType.ATK:
                        items[i].weaponApixes[j].minValue = 1.2f * items[i].level;
                        items[i].weaponApixes[j].maxValue = 1.4f * items[i].level;
                        break;

                    case WeaponApixType.MATK:
                        // MATK도 비슷한 방식으로 공격력에 영향을 미침 (마법 공격력)
                        items[i].weaponApixes[j].minValue = 0.8f * items[i].level;
                        items[i].weaponApixes[j].maxValue = 2.0f * items[i].level;
                        break;

                    case WeaponApixType.AttackSpeed:
                        // AttackSpeed는 기본 1, 0에 가까울수록 빨라지므로 매우 작은 값 사용
                        items[i].weaponApixes[j].minValue = 0.0007f * items[i].level;
                        items[i].weaponApixes[j].maxValue = 0.0035f * items[i].level;
                        break;

                    case WeaponApixType.CastingSpeed:
                        // CastingSpeed도 1이 기본이므로 매우 작은 값 설정
                        items[i].weaponApixes[j].minValue = 0.0007f * items[i].level;
                        items[i].weaponApixes[j].maxValue = 0.0035f * items[i].level;
                        break;

                    case WeaponApixType.MaxHp:
                        // MaxHP는 방어구의 MaxHp와 비슷하게 설정
                        items[i].weaponApixes[j].minValue = 6 * items[i].level;
                        items[i].weaponApixes[j].maxValue = 8 * items[i].level;
                        break;

                    case WeaponApixType.Accuracy:
                        items[i].weaponApixes[j].minValue = 0.9f * items[i].level;
                        items[i].weaponApixes[j].maxValue = 1.2f * items[i].level;
                        break;
                    default:
                        break;
                }
            }
            items[i].armorApixes = new ApixRange<ArmorApixType>[Enum.GetValues(typeof(ArmorApixType)).Length];
            for (int j = 0; j < items[i].armorApixes.Length; j++)
            {
                items[i].armorApixes[j].apixType = (ArmorApixType)j;
                switch (items[i].armorApixes[j].apixType)
                {
                    case ArmorApixType.MaxMana:
                        items[i].armorApixes[j].minValue = 4 * items[i].level;
                        items[i].armorApixes[j].maxValue = 5.5f * items[i].level; 
                        break;

                    case ArmorApixType.ManaRegen:
                        items[i].armorApixes[j].minValue = 0.03f * items[i].level;
                        items[i].armorApixes[j].maxValue = 0.08f * items[i].level;
                        break;

                    case ArmorApixType.MaxHp:
                        items[i].armorApixes[j].minValue = 5 * items[i].level; 
                        items[i].armorApixes[j].maxValue = 7 * items[i].level; 
                        break;

                    case ArmorApixType.HpRegen:
                        items[i].armorApixes[j].minValue = 0.05f * items[i].level;
                        items[i].armorApixes[j].maxValue = 0.12f * items[i].level; 
                        break;

                    case ArmorApixType.MoveSpeed:
                        items[i].armorApixes[j].minValue = 0.015f * items[i].level; 
                        items[i].armorApixes[j].maxValue = 0.04f * items[i].level; 
                        break;

                    case ArmorApixType.deff:
                        items[i].armorApixes[j].minValue = 5f * items[i].level; 
                        items[i].armorApixes[j].maxValue = 8f * items[i].level; 
                        break;

                    case ArmorApixType.Evasion:
                        items[i].armorApixes[j].minValue = 0.04f * items[i].level;
                        items[i].armorApixes[j].maxValue = 0.1f * items[i].level;
                        break;

                    case ArmorApixType.magicDeff:
                        items[i].armorApixes[j].minValue = 5f * items[i].level;
                        items[i].armorApixes[j].maxValue = 8f * items[i].level;
                        break;
                    default:
                        Debug.LogError($"{items[i].armorApixes[j].apixType}설정하지 않은 어픽스가 있습니다.");
                        break;
                }
            }
            items[i].statApixes = new ApixRange<BasicStatTypes>[Enum.GetValues(typeof(BasicStatTypes)).Length];
            for (int j = 0; j < items[i].statApixes.Length; j++)
            {
                items[i].statApixes[j].apixType = (BasicStatTypes)j;
                items[i].statApixes[j].minValue = 1;
                items[i].statApixes[j].maxValue = Mathf.Ceil(i * 0.8f);
            }
        }
        EditorUtility.SetDirty(this);

        // 프로젝트에 저장
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
[System.Serializable]
public struct ApixRange<T> where T : Enum
{
    public T apixType;
    public float minValue;
    public float maxValue;
}

[CustomEditor(typeof(ApixDatas))]
public class ApixSCOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ApixDatas temp = (ApixDatas)target;
        if (GUILayout.Button("Apix 파싱"))
        {
            temp.GetSheetValue();
        }
    }
}
#endregion