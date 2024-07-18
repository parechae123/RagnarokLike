using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;
using PlayerDefines.Stat;

[CreateAssetMenu(fileName = "new Skill", menuName = "Skill/Skill")]
public class SkillBase : ScriptableObject
{
    [Header("��ų �̸�")]
    public string skillName;
    public string koreanSkillName;
    [Header("������,������ Ÿ�� �� ���Ÿ��,��ųŸ��")]
    public float defaultValue;
    public ValueType damageType;
    public float coefficient;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="caster">������</param>
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
    [Header("��ų ����")]
    [HideInInspector][SerializeField] public ObjectiveType objectiveType;
    [HideInInspector][SerializeField] public SkillPosition skillPosition;
    public SkillType skillType;
    [Header("��ų ����")]
    [HideInInspector][SerializeField] private float skillBound;

    [Header("��ų ����,�ִ�ġ,���� �Ҹ� �� ĳ���� �ð�")]
    public byte skillLevelInfo;
    [HideInInspector][SerializeField] public float spCost;

    [HideInInspector][SerializeField] private float defaultCastingTime;

    private Animator[] effectOBJs = new Animator[0];//�� ���� ����Ʈ ������Ʈ
    [Header("��ų ����Ʈ ������Ʈ ������, ������")]
    [SerializeField] private GameObject effectOBJPrefab;
    [SerializeField] private Sprite skillIcon;
    [Header("��ų ��Ÿ�")]
    [HideInInspector][SerializeField] public byte skillRange;
    [Header("��ų ���ӽð�")]
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

#region ��ų Ŀ���� �ν�����

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
