using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questID;
    public string questName;
    public string description;
    public bool isQuestDone;
    public int level;
    public Quest(ScriptableQuest questInfo)
    {
        this.questID = questInfo.questID;
        this.questName = questInfo.questName;
        this.description = questInfo.description;
        this.level = questInfo.level;
        this.conditions = questInfo.condition.ToList();
    }

    [SerializeField] private List<IQuestConditions> conditions;
}
#region QuestEnums

public enum RewardType //��������Ʈ ������ �ָ������� �߰�
{
    //���    ,   �Ҹ�ǰ,    ���,     ����
    armor,weapon, cosumable, gold, classChange,exp,misc
}
public enum QuestType   //����Ʈ�� ����
{   //���        ����      ��ȣ�ۿ�    ��ȭ
    Hunting, Collection, Interaction, Conversation
}
public enum QuestStatus
{
    NotStarted,                     //����Ʈ�� ���� ���۵��� ���� ����
    InProgress,                     //����Ʈ�� ���� ���� ���� ����
    Completed,                      //����Ʈ�� �Ϸ�� ����
    Failed
}
#endregion


#region QuestInterfaces
public interface IQuestConditions
{
    bool IsMet();                   //������ �����Ǿ����� ���� ��ȯ
    void Intialize();               //������ �ʱ�ȭ�ϴ� �޼���

    float GetProgress();            //���� ������

    string GetDescription();        //���� ���� ��ȯ �޼���
}
[System.Serializable]
public class HuntingCondition : IQuestConditions
{
    public string targetCode;
    public int curr = 0;
    public int require;
    public HuntingCondition(string targetCode,int require)
    {
        this.targetCode = targetCode;
        this.require = require;
    }

    public bool IsMet()
    {
        return true;
    }
    public void Intialize()
    {

    }
    public float GetProgress() 
    {
        return 0f;
    }
    public string GetDescription()
    {
        return string.Empty;
    }
}
[System.Serializable]
public class CollectionCondition : IQuestConditions
{
    public string itemCode;
    public int require;
    public int curr = 0;
    public CollectionCondition(string itemCode,int require) 
    {
        this.itemCode = itemCode;
        this.require = require;
    }
    public bool IsMet()
    {
        return true;
    }
    public void Intialize()
    {

    }
    public float GetProgress()
    {
        return 0f;
    }
    public string GetDescription()
    {
        return string.Empty;
    }
}
[System.Serializable]
public class InteractionCondition : IQuestConditions
{
    public string interactCode;
    public int require;
    public int curr = 0;
    public InteractionCondition(string itemCode,int require) 
    { 
        interactCode = itemCode;
        this.require = require;
    }
    public bool IsMet()
    {
        return true;
    }
    public void Intialize()
    {

    }
    public float GetProgress()
    {
        return 0f;
    }
    public string GetDescription()
    {
        return string.Empty;
    }
}
[System.Serializable]
public class ConversationCondition : IQuestConditions
{
    public string npcCode;
    public int dialogueIndex;
    private bool isDone;
    public ConversationCondition(string npcCode,int dialogueIndex)
    {
        this.npcCode = npcCode;
        this.dialogueIndex = dialogueIndex;
    }
    public bool IsMet()
    {
        return isDone;
    }
    public void Intialize()
    {

    }
    public float GetProgress()
    {
        return 0f;
    }
    public string GetDescription()
    {
        return string.Empty;
    }
}

public interface IRewards
{
    
    void GetReward();
}

public class WeaponReward : IRewards
{
     
    public void GetReward()
    {

    }
}
public class ArmorReward : IRewards
{
     
    public void GetReward()
    {

    }
}
public class ConsumReward : IRewards
{
     
    public void GetReward()
    {

    }
}
public class MiscReward : IRewards
{
     
    public void GetReward()
    {

    }
}
public class EXPReward : IRewards
{
     
    public void GetReward()
    {

    }
}
public class RewardParsingData
{
    
    sbyte Level;
    RewardType rewardType;

}
#endregion

