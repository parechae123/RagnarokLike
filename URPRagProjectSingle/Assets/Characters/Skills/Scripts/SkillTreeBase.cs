using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "new SkillTree", menuName = "Skill/SkillTree")]
public class SkillTreeBase : ScriptableObject
{
    [SerializeField] BaseJobType jobType;
    [SerializeField] JobPhase jobPhase;
    [SerializeField] JobRoot jobRoot;
    [SerializeField]SkillIconsInSkilltree skillIconsInSkilltree;
    public void aa()
    {
        
    }
}
[System.Serializable]
public class SkillIconsInSkilltree
{

    [SerializeField] private SkillGetConditionTable[] skills;
    public SkillGetConditionTable this[int index]
    {
        get
        {
            return skills[index];
        }
        set
        {
            this[index] = value;
        }
    }
    /// <summary>
    /// 스킬을 배울 수 있는지 여부를 반환
    /// </summary>
    /// <param name="targetLearnSkillIndex"></param>
    /// <returns></returns>
    public bool isLearnAble(int targetLearnSkillIndex)
    {
        if (this[targetLearnSkillIndex].skillGetConditions.Length <= 0) return true;
        else
        {
            for (int i = 0; i < this[targetLearnSkillIndex].skillGetConditions.Length; i++)
            {
                if (this[this[targetLearnSkillIndex].skillGetConditions[i].targetIndex].thisSkill.isSkillLearned)       //스킬을 배운 상태인가?
                {
                    if (this[this[targetLearnSkillIndex].skillGetConditions[i].targetIndex].thisSkill.nowSkillLevel < this[targetLearnSkillIndex].skillGetConditions[i].targetLevel)//스킬의 레벨이 충족되지 않았는가
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }
}
public class SkillGetConditionTable
{
    public SkillInfoInGame thisSkill;
    public SkillGetCondition[] skillGetConditions = new SkillGetCondition[0];     //선행스킬 인덱스,배열이 0일 경우 조건없음
}
public class SkillGetCondition
{
    public int targetLevel;
    public int targetIndex;
}