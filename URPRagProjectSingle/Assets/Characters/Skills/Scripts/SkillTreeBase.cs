using System;
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
    [SerializeField]public SkillIconsInSkilltree skillIconsInSkilltree;

}
[System.Serializable]
public class SkillIconsInSkilltree
{

    [SerializeField] public SkillGetConditionTable[] skills = new SkillGetConditionTable[0];
    public SkillGetConditionTable this[int index]
    {
        get
        {
            return skills[index];
        }
        set
        {
            skills[index] = value;
        }
    }

    public int Length
    {
        get { return skills.Length; }
    }
    public void AddArraySkills(SkillInfo data)
    {
        int skillLength = skills.Length;
        Array.Resize(ref skills, skillLength + 1);
        skills[skillLength] = new SkillGetConditionTable(data);
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
[System.Serializable]
public class SkillGetConditionTable
{   
    public SkillInfo thisSkillInScriptableOBJ;
    [HideInInspector]public SkillInfoInGame thisSkill;
    public Vector2 positionOnSkillTree;
    public SkillGetCondition[] skillGetConditions = new SkillGetCondition[0];     //선행스킬 인덱스,배열이 0일 경우 조건없음
    public SkillGetConditionTable(SkillInfo skillInfo)
    {
        thisSkillInScriptableOBJ = skillInfo;
        thisSkill = new SkillInfoInGame(skillInfo);
        skillGetConditions = new SkillGetCondition[0];
    }
    public bool isEmpty
    {
        get
        {
            if (thisSkillInScriptableOBJ == null || thisSkill == null)
            {
                return true;
            }
            return false;
        }
    }
    public void AddCondition(int targetIndex,int targetLevel)
    {
        int tempIndex = skillGetConditions.Length;
        if (skillGetConditions == null) skillGetConditions = new SkillGetCondition[0];
        Array.Resize(ref skillGetConditions, skillGetConditions.Length +1);
        skillGetConditions[tempIndex] = new SkillGetCondition(targetIndex,targetLevel);
    }
}
[System.Serializable]
public class SkillGetCondition
{
    public int targetIndex;
    public int targetLevel;
    public SkillGetCondition(int targetIndex, int targetLevel)
    {
        this.targetIndex = targetIndex;
        this.targetLevel = targetLevel;
    }
}