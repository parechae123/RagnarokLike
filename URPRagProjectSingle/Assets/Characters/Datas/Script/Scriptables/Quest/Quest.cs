using System.Collections;
using System.Collections.Generic;
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
    public QuestType questType;
    public int level;

    [SerializeField] private List<IQuestConditions> conditions;
}
#region QuestEnums

public enum RewardType //��������Ʈ ������ �ָ������� �߰�
{
    //���    ,   �Ҹ�ǰ,    ���,     ����
    Equipment, Cosumable, Gold, ClassChange
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

#endregion