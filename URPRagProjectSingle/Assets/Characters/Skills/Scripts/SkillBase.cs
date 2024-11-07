using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;
using PlayerDefines.Stat;
using JetBrains.Annotations;

[CreateAssetMenu(fileName = "new Skill", menuName = "Skill/Skill")]
public class SkillBase : ScriptableObject
{
    [Header("스킬 이름")]
    public string skillName;
    public string koreanSkillName;
    public byte skillLevel;
    [Header("데미지,데미지 타입 및 계수타입,스킬타입")]
    public float defaultValue;
    public ValueType damageType;
    public float coefficient;
    public float coolTime;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">시전자</param>
    /// <returns></returns>
    public ValueType coefficientType;
    [Header("스킬 범위")]
    [SerializeField] public byte skillBound;

    [Header("스킬 레벨,최대치,마나 소모값 및 캐스팅 시간")]
    [SerializeField] public float spCost;

    [SerializeField] public float defaultCastingTime;

    [Header("스킬 사거리")]
    [SerializeField] public byte skillRange;
    [Header("스킬 지속시간")]
    [SerializeField] public float skillDuration;

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
        coolTime = targetObject.coolTime;
        skillRange = targetObject.skillRange;
        skillDuration = targetObject.skillDuration;
        buffTypeOne = targetObject.buffTypeOne;
        buffValueOne = targetObject.buffValueOne;
        buffTypeTwo = targetObject.buffTypeTwo;
        buffValueTwo = targetObject.buffValueTwo;
        buffTypeThree = targetObject.buffTypeThree;
        buffValueThree  = targetObject.buffValueThree;
    }

#endif
}


[System.Serializable]
public class SkillBaseInGameData
{
    [Header("스킬 이름")]
    public string skillName;
    public string koreanSkillName;
    public byte skillLevel;
    [Header("데미지,데미지 타입 및 계수타입,스킬타입")]
    public float defaultValue;
    public ValueType damageType;
    public float coefficient;
    public float coolTime;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">시전자</param>
    /// <returns></returns>

    public ValueType coefficientType;
    [Header("스킬 범위")]
    [SerializeField] private byte skillBound;
    public byte SkillBound
    {
        get { return skillBound; }
    }

    [Header("스킬 레벨,최대치,마나 소모값 및 캐스팅 시간")]
    [SerializeField] public float spCost;

    [SerializeField] public float defaultCastingTime;

    [Header("스킬 사거리")]
    [SerializeField] public byte skillRange;
    [Header("스킬 지속시간")]
    [SerializeField] public float skillDuration;

    public (string, float)[] buffSet;


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
                return defaultValue+ (caster.TotalAccuracy* coefficient);
            default:
                return 0;
        }
    }
    public float TotalDamage(PlayerStat caster)
    {
        switch (damageType)
        {
            case ValueType.Physical:
                return defaultValue + (caster.attackDamage * coefficient);
            case ValueType.Magic:
                return defaultValue + (caster.abilityPower * coefficient);
            case ValueType.Heal:
                return -1 * (defaultValue + (caster.abilityPower * coefficient));
            case ValueType.PhysicalRange:
                return defaultValue+ (caster.accuracy* coefficient);
            default:
                return 0;
        }
    }
    public SkillBaseInGameData(SkillBase targetObject)
    {
        skillName = new string(targetObject.skillName);
        koreanSkillName = new string(targetObject.koreanSkillName);
        skillLevel = targetObject.skillLevel;
        defaultValue = targetObject.defaultValue;
        damageType = targetObject.damageType;
        coefficient = targetObject.coefficient;
        coolTime = targetObject.coolTime;
        coefficientType = targetObject.coefficientType;
        skillBound = targetObject.skillBound;
        spCost = targetObject.spCost;
        defaultCastingTime = targetObject.defaultCastingTime;
        skillRange = targetObject.skillRange;
        skillDuration = targetObject.skillDuration;
        if (targetObject.buffTypeOne != "None") buffSet = new (string, float)[1];
        if(targetObject.buffTypeTwo != "None") buffSet = new (string, float)[2];
        if (targetObject.buffTypeThree != "None") buffSet = new (string, float)[3];
        buffSet[0].Item1 = targetObject.buffTypeOne;
        buffSet[0].Item2 = targetObject.buffValueOne;
        buffSet[1].Item1 = targetObject.buffTypeTwo;
        buffSet[1].Item2 = targetObject.buffValueTwo;
        buffSet[2].Item1 = targetObject.buffTypeThree;
        buffSet[2].Item2 = targetObject.buffValueThree;
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
    Physical, Magic, Heal,PhysicalRange,TrueDamage
}
