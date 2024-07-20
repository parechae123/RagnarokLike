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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">시전자</param>
    /// <returns></returns>
    public ValueType coefficientType;
    [Header("스킬 범위")]
    [SerializeField] public float skillBound;

    [Header("스킬 레벨,최대치,마나 소모값 및 캐스팅 시간")]
    [SerializeField] public float spCost;

    [SerializeField] public float defaultCastingTime;

    [Header("스킬 사거리")]
    [SerializeField] public byte skillRange;
    [Header("스킬 지속시간")]
    [SerializeField] public float skillDuration;
    public float totalDamage(Stats caster)
    {
        switch (damageType)
        {
            case ValueType.Physical:
                return defaultValue + (caster.attackDamage * coefficient);
            case ValueType.Magic:
                return defaultValue + (caster.abilityPower * coefficient);
            case ValueType.heal:
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
        skillRange = targetObject.skillRange;
        skillDuration = targetObject.skillDuration;
    }

#endif
}
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">시전자</param>
    /// <returns></returns>

    public ValueType coefficientType;
    [Header("스킬 범위")]
    [SerializeField] private float skillBound;

    [Header("스킬 레벨,최대치,마나 소모값 및 캐스팅 시간")]
    [SerializeField] public float spCost;

    [SerializeField] private float defaultCastingTime;

    [Header("스킬 사거리")]
    [SerializeField] public byte skillRange;
    [Header("스킬 지속시간")]
    [SerializeField] public float skillDuration;
    public float totalDamage(Stats caster)
    {
        switch (damageType)
        {
            case ValueType.Physical:
                return defaultValue + (caster.attackDamage * coefficient);
            case ValueType.Magic:
                return defaultValue + (caster.abilityPower * coefficient);
            case ValueType.heal:
                return -1 * (defaultValue + (caster.abilityPower * coefficient));
            default:
                return 0;
        }

    }
    public void ScriptableObjectToInGameSkillData(SkillBase targetObject)
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
        skillRange = targetObject.skillRange;
        skillDuration = targetObject.skillDuration;
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
    Physical, Magic, heal
}
