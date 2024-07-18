using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;
using PlayerDefines.Stat;

[CreateAssetMenu(fileName = "new Skill", menuName = "Skill/Skill")]
public class SkillBase : ScriptableObject
{
    [Header("스킬 이름")]
    public string skillName;
    public string koreanSkillName;
    [Header("데미지,데미지 타입 및 계수타입,스킬타입")]
    public float defaultValue;
    public ValueType damageType;
    public float coefficient;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">시전자</param>
    /// <returns></returns>
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
    public ValueType coefficientType;
    [Header("스킬 유형")]
    [HideInInspector][SerializeField] public ObjectiveType objectiveType;
    [HideInInspector][SerializeField] public SkillPosition skillPosition;
    public SkillType skillType;
    [Header("스킬 범위")]
    [HideInInspector][SerializeField] private float skillBound;

    [Header("스킬 레벨,최대치,마나 소모값 및 캐스팅 시간")]
    public byte skillLevelInfo;
    [HideInInspector][SerializeField] public float spCost;

    [HideInInspector][SerializeField] private float defaultCastingTime;

    private Animator[] effectOBJs = new Animator[0];//씬 내의 이펙트 오브젝트
    [Header("스킬 이펙트 오브젝트 프리팹, 아이콘")]
    [SerializeField] private GameObject effectOBJPrefab;
    [SerializeField] private Sprite skillIcon;
    [Header("스킬 사거리")]
    [HideInInspector][SerializeField] public byte skillRange;
    [Header("스킬 지속시간")]
    [HideInInspector][SerializeField] public float skillDuration;
    public Sprite SkillIcon
    {
        get { return skillIcon; }
    }


    public Animator GetNonPlayingSkillEffect()
    {
        for (int i = 0; i < effectOBJs.Length; i++)
        {
            if (effectOBJs[i].gameObject.activeSelf)
            {
                continue;
            }
            else
            {
                return effectOBJs[i];
            }
        }
        int tempNum = effectOBJs.Length;
        Array.Resize(ref effectOBJs, tempNum + 1);
        effectOBJs[tempNum] = GameObject.Instantiate(effectOBJPrefab).GetComponent<Animator>();
        return effectOBJs[tempNum];

    }
    public void StartSkillEffect(Vector3 EffectPosition)
    {
        Animator tempAnim = GetNonPlayingSkillEffect();
        tempAnim.transform.position = EffectPosition;
        tempAnim.gameObject.SetActive(true);
        tempAnim.Play(skillName + "Effect");
        DOVirtual.DelayedCall(tempAnim.GetCurrentAnimatorClipInfo(0)[0].clip.length, () =>
        {
            if (tempAnim != null) tempAnim.gameObject.SetActive(false);
        });
    }
}

#region 스킬 커스텀 인스펙터

#if UNITY_EDITOR
[CustomEditor(typeof(SkillBase))]
public class SkillBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SkillBase skill = (SkillBase)target;
        base.OnInspectorGUI();

        serializedObject.Update();
        ObjectiveType objectiveType = (ObjectiveType)serializedObject.FindProperty("objectiveType").intValue;
        SkillPosition skillPosition = (SkillPosition)serializedObject.FindProperty("skillPosition").intValue;
        SkillType skillType = (SkillType)serializedObject.FindProperty("skillType").intValue;
        switch (skillType)
        {
            case SkillType.None:
                skill.objectiveType = ObjectiveType.None;
                break;
            case SkillType.Passive:
                skill.objectiveType = ObjectiveType.None;
                break;
            case SkillType.Active:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spCost"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCastingTime"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skillRange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("objectiveType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skillPosition"));


                break;
            case SkillType.buff:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skillDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spCost"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCastingTime"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skillRange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("objectiveType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skillPosition"));
                break;
            default:
                break;
        }
        switch (objectiveType)
        {
            case ObjectiveType.None:
                break;
            case ObjectiveType.OnlyTarget:

                break;
            case ObjectiveType.Bounded:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skillBound"));
                break;
        }
        switch (skillPosition)
        {
            case SkillPosition.self:
                break;
            case SkillPosition.cursor:

                break;
            default:
                break;
        }


        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion
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
