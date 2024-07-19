using DG.Tweening;
using System;
using UnityEngine;
[CreateAssetMenu(fileName = "new SkillInfo", menuName = "Skill/SkillInfomations")]
[System.Serializable]
public class SkillInfo/* : ScriptableObject*/
{
    ///�ش� Ŭ������ �÷��̾ ��ų ������ �������־���ϴ� ��ü�� ���� ��ũ���ͺ� ������Ʈ �����ؾ���
    public string skillName;
    [Header("��ų ����")]
    [SerializeField] public ObjectiveType objectiveType;
    public SkillType skillType;
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
    /// ��ų �ҷ�����
    /// </summary>
    /// <param name="skillName"></param>
    /// <param name="skill"></param>
    /// <param name="maxSkillLevel"></param>
/*    public SkillInfo(string skillName, SkillBase[] skill, byte maxSkillLevel)
    {
        this.maxSkillLevel = maxSkillLevel;
        for (int i = 0; i < skill.Length; i++)
        {
            if (skill[i].skillName != skillName)
            {
                Debug.LogError("��ų�� �ҷ����� �� ������ �߻��Ͽ����ϴ�, ���̺�� �Է��Ϸ��� �����Ͱ� �����մϴ�.");
            }
            this.skill[i] = skill[i];
        }
    }*/


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