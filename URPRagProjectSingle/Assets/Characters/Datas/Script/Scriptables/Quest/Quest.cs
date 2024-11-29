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

public enum RewardType //전직퀘스트 구분을 주목적으로 추가
{
    //장비    ,   소모품,    골드,     전직
    Equipment, Cosumable, Gold, ClassChange
}
public enum QuestType   //퀘스트의 유형
{   //사냥        수집      상호작용    대화
    Hunting, Collection, Interaction, Conversation
}
public enum QuestStatus
{
    NotStarted,                     //퀘스트가 아직 시작되지 않은 상태
    InProgress,                     //퀘스트가 현재 진행 중인 상태
    Completed,                      //퀘스트가 완료된 상태
    Failed
}
#endregion
#region QuestInterfaces
public interface IQuestConditions
{
    bool IsMet();                   //조건이 충족되었는지 여부 반환
    void Intialize();               //조건을 초기화하는 메서드

    float GetProgress();            //조건 충족도

    string GetDescription();        //조건 설명 반환 메서드
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