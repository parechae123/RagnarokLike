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
    /// ��ų�� ��� �� �ִ��� ���θ� ��ȯ
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
                if (this[this[targetLearnSkillIndex].skillGetConditions[i].targetIndex].thisSkill.isSkillLearned)       //��ų�� ��� �����ΰ�?
                {
                    if (this[this[targetLearnSkillIndex].skillGetConditions[i].targetIndex].thisSkill.nowSkillLevel < this[targetLearnSkillIndex].skillGetConditions[i].targetLevel)//��ų�� ������ �������� �ʾҴ°�
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
    public SkillGetCondition[] skillGetConditions = new SkillGetCondition[0];     //���ེų �ε���,�迭�� 0�� ��� ���Ǿ���
}
public class SkillGetCondition
{
    public int targetLevel;
    public int targetIndex;
}