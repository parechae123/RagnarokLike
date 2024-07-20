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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">������</param>
    /// <returns></returns>
    public ValueType coefficientType;
    [Header("��ų ����")]
    [SerializeField] public float skillBound;

    [Header("��ų ����,�ִ�ġ,���� �Ҹ� �� ĳ���� �ð�")]
    [SerializeField] public float spCost;

    [SerializeField] public float defaultCastingTime;

    [Header("��ų ��Ÿ�")]
    [SerializeField] public byte skillRange;
    [Header("��ų ���ӽð�")]
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
    [Header("��ų �̸�")]
    public string skillName;
    public string koreanSkillName;
    public byte skillLevel;
    [Header("������,������ Ÿ�� �� ���Ÿ��,��ųŸ��")]
    public float defaultValue;
    public ValueType damageType;
    public float coefficient;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">������</param>
    /// <returns></returns>

    public ValueType coefficientType;
    [Header("��ų ����")]
    [SerializeField] private float skillBound;

    [Header("��ų ����,�ִ�ġ,���� �Ҹ� �� ĳ���� �ð�")]
    [SerializeField] public float spCost;

    [SerializeField] private float defaultCastingTime;

    [Header("��ų ��Ÿ�")]
    [SerializeField] public byte skillRange;
    [Header("��ų ���ӽð�")]
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
