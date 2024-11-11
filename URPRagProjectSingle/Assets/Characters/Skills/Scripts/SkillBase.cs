using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;
using PlayerDefines.Stat;
using JetBrains.Annotations;

[CreateAssetMenu(fileName = "new Skill", menuName = "Skill/Skill")]
public class SkillBase : ScriptableObject
{
    [Header("��ų �̸�")]
    public string skillName;
    public string koreanSkillName;
    public byte skillLevel;
    [Header("������,������ Ÿ�� �� ���Ÿ��,��ųŸ��")]
    public float defaultValue;
    public ValueType damageType;
    public float coefficient;
    public int coolTimeTick;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">������</param>
    /// <returns></returns>
    public ValueType coefficientType;
    [Header("��ų ����")]
    [SerializeField] public byte skillBound;

    [Header("��ų ����,�ִ�ġ,���� �Ҹ� �� ĳ���� �ð�")]
    [SerializeField] public float spCost;

    [SerializeField] public float defaultCastingTime;

    [Header("��ų ��Ÿ�")]
    [SerializeField] public byte skillRange;
    [Header("��ų ���ӽð�")]
    [SerializeField] public int skillDuration;

    [SerializeField] public string buffTypeOne;
    [SerializeField] public float buffValueOne;
    [SerializeField] public string buffTypeTwo;
    [SerializeField] public float buffValueTwo;
    [SerializeField] public string buffTypeThree;
    [SerializeField] public float buffValueThree;

    public float TotalDamage(Stats caster)
    {
        switch (damageType)
        {
            case ValueType.Physical:
                return defaultValue + (caster.attackDamage * coefficient);
            case ValueType.Magic:
                return defaultValue + (caster.abilityPower * coefficient);
            case ValueType.Heal:
                return -1 * (defaultValue + (caster.abilityPower * coefficient));
            default:
                return 0;
        }

    }
#if UNITY_EDITOR

    public void ObjectToScriptableObject(SkillBaseObjectOnly targetObject)
    {
        skillName = targetObject.skillName;
        koreanSkillName = targetObject.koreanSkillName;
        skillLevel = targetObject.skillLevel;
        defaultValue = targetObject.defaultValue;
        damageType = targetObject.damageType;
        coefficient = targetObject.coefficient;
        coefficientType = targetObject.coefficientType;
        skillBound = targetObject.skillBound;
        spCost = targetObject.spCost;
        defaultCastingTime = targetObject.defaultCastingTime;
        coolTimeTick = targetObject.coolTimeTick;
        skillRange = targetObject.skillRange;
        skillDuration = targetObject.skillDuration;
        buffTypeOne = targetObject.buffTypeOne;
        buffValueOne = targetObject.buffValueOne;
        buffTypeTwo = targetObject.buffTypeTwo;
        buffValueTwo = targetObject.buffValueTwo;
        buffTypeThree = targetObject.buffTypeThree;
        buffValueThree = targetObject.buffValueThree;
    }
    public void SaveAsset()
    {
        // ���� ������ Dirty ���·� ǥ���Ͽ� Unity�� �ν��ϵ��� ��
        EditorUtility.SetDirty(this);

        // ������Ʈ�� ����
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif
}


[System.Serializable]
public class SkillBaseInGameData
{
    [Header("��ų �̸�")]
    public string skillName;
    public string koreanSkillName;
    public byte skillLevel;
    [Header("������,������ Ÿ�� �� ���Ÿ��,��ųŸ��")]
    public float defaultValue;
    public ValueType damageType;
    public float coefficient;
    public int coolTimeTick;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">������</param>
    /// <returns></returns>

    public ValueType coefficientType;
    [Header("��ų ����")]
    [SerializeField] protected byte skillBound;
    public byte SkillBound
    {
        get { return skillBound; }
    }

    [Header("��ų ����,�ִ�ġ,���� �Ҹ� �� ĳ���� �ð�")]
    [SerializeField] public float spCost;

    [SerializeField] public float defaultCastingTime;

    [Header("��ų ��Ÿ�")]
    [SerializeField] public byte skillRange;
    [Header("��ų ���ӽð�")]
    [SerializeField] public int skillDuration;



    public float TotalDamage(Stats caster)
    {
        switch (damageType)
        {
            case ValueType.Physical:
                return defaultValue + (caster.TotalAD * coefficient);
            case ValueType.Magic:
                return defaultValue + (caster.TotalAP * coefficient);
            case ValueType.Heal:
                return -1 * (defaultValue + (caster.TotalAP * coefficient));
            case ValueType.PhysicalRange:
                return defaultValue + (caster.TotalAccuracy * coefficient);
            default:
                return 0;
        }
    }
/*    public float TotalDamage(PlayerStat caster)
    {
        switch (damageType)
        {
            case ValueType.Physical:
                return defaultValue + (caster.TotalAD * coefficient);
            case ValueType.Magic:
                return defaultValue + (caster.TotalAP * coefficient);
            case ValueType.Heal:
                return -1 * (defaultValue + (caster.TotalAP * coefficient));
            case ValueType.PhysicalRange:
                return defaultValue + (caster.TotalAccuracy * coefficient);
            default:
                return 0;
        }
    }*/
    public SkillBaseInGameData(SkillBase targetObject)
    {
        skillName = new string(targetObject.skillName);
        koreanSkillName = new string(targetObject.koreanSkillName);
        skillLevel = targetObject.skillLevel;
        defaultValue = targetObject.defaultValue;
        damageType = targetObject.damageType;
        coefficient = targetObject.coefficient;
        coolTimeTick = targetObject.coolTimeTick;
        coefficientType = targetObject.coefficientType;
        skillBound = targetObject.skillBound;
        spCost = targetObject.spCost;
        defaultCastingTime = targetObject.defaultCastingTime;
        skillRange = targetObject.skillRange;
        skillDuration = targetObject.skillDuration;
    }
}
[System.Serializable]
public class BuffSkillBaseInGameData : SkillBaseInGameData
{
    public IBuffs[] buffSet;
    public BuffSkillBaseInGameData(SkillBase targetObject) : base(targetObject)
    {
        skillName = new string(targetObject.skillName);
        koreanSkillName = new string(targetObject.koreanSkillName);
        skillLevel = targetObject.skillLevel;
        defaultValue = targetObject.defaultValue;
        damageType = targetObject.damageType;
        coefficient = targetObject.coefficient;
        coolTimeTick = targetObject.coolTimeTick;
        coefficientType = targetObject.coefficientType;
        skillBound = targetObject.skillBound;
        spCost = targetObject.spCost;
        defaultCastingTime = targetObject.defaultCastingTime;
        skillRange = targetObject.skillRange;
        skillDuration = targetObject.skillDuration;

        if (targetObject.buffTypeOne != "None")
        {
            buffSet = new IBuffs[1];
            SetBuffType(targetObject.buffTypeOne,targetObject.buffValueOne,0);
        }
        if (targetObject.buffTypeTwo != "None")
        {
            Array.Resize(ref buffSet, 2);
            SetBuffType(targetObject.buffTypeTwo, targetObject.buffValueTwo, 1);
        }
        if (targetObject.buffTypeThree != "None")
        {
            Array.Resize(ref buffSet, 3);
            SetBuffType(targetObject.buffTypeThree, targetObject.buffValueThree, 2);
        }
    }

    public void SetBuffType(string typeString,float targetValue,int targetIndex)
    {
        if (typeString.Substring(0, typeString.IndexOf('.')) == "WeaponApixType")
        {
            string apixString = typeString.Substring(typeString.IndexOf('.') + 1, typeString.Length);
            WeaponApixType result;

            bool success = Enum.TryParse(apixString, true, out result);
            if (success)
            {
                buffSet[targetIndex] = new OffensiveBuff(targetValue, result);
            }
            else
            {
                Debug.LogError($"�ùٸ��� �ʽ��ϴ�. ���� buffString : {apixString}");
            }
        }
        else if (typeString.Substring(0, typeString.IndexOf('.')) == "ArmorApixType")
        {
            string apixString = typeString.Substring(typeString.IndexOf('.') + 1, typeString.Length);
            ArmorApixType result;

            bool success = Enum.TryParse(apixString, true, out result);  // Enum.TryParse ���
            if (success)
            {
                buffSet[targetIndex] = new DeffensiveBuff(targetValue, result);
            }
            else
            {
                Debug.LogError($"�ùٸ��� �ʽ��ϴ�. ���� buffString : {apixString}");
            }

        }
        else if (typeString.Substring(0, typeString.IndexOf('.')) == "BasicStatTypes")
        {
            string apixString = typeString.Substring(typeString.IndexOf('.') + 1, typeString.Length- (typeString.IndexOf('.') + 1));
            BasicStatTypes result;

            bool success = Enum.TryParse(apixString, true, out result);  // Enum.TryParse ���
            if (success)
            {
                buffSet[targetIndex] = new StatBuff(targetValue, result);
            }
            else
            {
                Debug.LogError($"�ùٸ��� �ʽ��ϴ�. ���� buffString : {apixString}");
            }
        }
    }
}



public enum SkillType
{
    None, Passive, Active, buff
}
public enum SkillPosition
{
    self, cursor
}
[Serializable]
public enum ObjectiveType
{
    None, OnlyTarget, Bounded
}
public enum ValueType
{
    Physical, Magic, Heal, PhysicalRange, TrueDamage
}
