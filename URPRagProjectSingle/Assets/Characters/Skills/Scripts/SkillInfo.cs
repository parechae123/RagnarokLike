using DG.Tweening;
using System;
using UnityEngine;
[CreateAssetMenu(fileName = "new SkillInfo", menuName = "Skill/SkillInfomations")]
[System.Serializable]
public class SkillInfo : ScriptableObject
{
    ///�ش� Ŭ������ �÷��̾ ��ų ������ �������־���ϴ� ��ü�� ���� ��ũ���ͺ� ������Ʈ �����ؾ���
    public string skillName;
    public string jobName;
    [Header("��ų ����")]
    public SkillType skillType;
    [SerializeField] public ObjectiveType objectiveType;
    [SerializeField] public SkillPosition skillPosition;
    public SkillBase[] skill;
    [Header("��ų ����Ʈ ������Ʈ ������, ������")]
    [SerializeField] private GameObject effectOBJPrefab;
    [SerializeField] private Sprite skillIcon;
    public byte maxSkillLevel;
    public byte nowSkillLeve;
    public byte castingSkillLevel;
    public bool isSkillLearned
    {
        get;
        private set;
    }

#if UNITY_EDITOR

    public void ObjectToScriptableOBJ(SkillInfoObjectOnly basedObject)
    {
        skillName = basedObject.skillName;
        maxSkillLevel = basedObject.maxSkillLevel;
        skill = new SkillBase[maxSkillLevel];
        jobName = basedObject.jobName;
        skillType = basedObject.skillType;
        objectiveType = basedObject.objectiveType;
        skillPosition = basedObject.skillPosition;
    }

#endif
    public void AddSkillDetailData(SkillBase skillData)
    {
        skill[skillData.skillLevel - (byte)1] = skillData; 
    }
}
[System.Serializable]
public class SkillInfoInGame
{
    ///�ش� Ŭ������ �÷��̾ ��ų ������ �������־���ϴ� ��ü�� ���� ��ũ���ͺ� ������Ʈ �����ؾ���
    public string skillName;
    public string jobName;
    [Header("��ų ����")]
    public SkillType skillType;
    [SerializeField] public ObjectiveType objectiveType;
    [SerializeField] public SkillPosition skillPosition;
    public SkillBase[] skill;
    [SerializeField] private Sprite skillIcon;
    [Header("��ų ����Ʈ ������Ʈ ������, ������")]
    private Animator[] effectOBJs = new Animator[0];//�� ���� ����Ʈ ������Ʈ
    [SerializeField] private GameObject effectOBJPrefab;
    public byte maxSkillLevel;
    public byte nowSkillLeve;
    public byte castingSkillLevel;
    public bool isSkillLearned
    {
        get;
        private set;
    }
    /// <summary>
    /// �� ������� ����Ʈ ������Ʈ�� �ִϸ��̼��� ��ȯ���ݴϴ�.
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// ��ų ����Ʈ�� ����մϴ�.
    /// </summary>
    /// <param name="EffectPosition"></param>
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