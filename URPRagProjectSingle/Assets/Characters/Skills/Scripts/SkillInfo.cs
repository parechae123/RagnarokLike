using UnityEngine;
[CreateAssetMenu(fileName = "new SkillInfo", menuName = "Skill/SkillInfomations")]
public class SkillInfo : ScriptableObject
{
    public string skillName;
    public SkillBase[] skill;
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
    public SkillInfo(string skillName, SkillBase[] skill, byte maxSkillLevel)
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
    }
}