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
    public int GetPhase
    {
        get { return ((int)jobPhase); }
    }
    [SerializeField] JobRoot jobRoot;
    [SerializeField]public SkillIconsInSkilltree skillIconsInSkilltree;
    public Vector2Int SkilltreeResolution;
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

}
[System.Serializable]
public class SkillGetConditionTable
{   
    public SkillInfo thisSkillInScriptableOBJ;
    [HideInInspector]public SkillInfoInGame thisSkill;
    public Vector2Int positionOnSkillTree;
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
    public void AddCondition(int targetIndex,byte targetLevel)
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
    public byte targetLevel;
    public SkillGetCondition(int targetIndex, byte targetLevel)
    {
        this.targetIndex = targetIndex;
        this.targetLevel = targetLevel;
    }
}