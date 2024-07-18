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
    /// 스킬 불러오기
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
                Debug.LogError("스킬을 불러오는 중 문제가 발생하였습니다, 테이블과 입력하려는 데이터가 상이합니다.");
            }
            this.skill[i] = skill[i];
        }
    }
}