using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Experimental.GraphView;
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

public enum RewardType 
{
    //장비    ,   소모품,    골드,     전직
    armor,weapon, cosumable, gold, classChange,exp,misc
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

public interface IRewards
{
    
    void GetReward();
}

public class WeaponReward : IRewards
{
    public int amount;
    public bool twohandedWP;
    public WeaponType weaponType;
    public sbyte level;
    public WeaponReward(RewardParsingData data) 
    { 
        amount = data.amount;
        twohandedWP = data.isTwoHanded;
        weaponType = data.weaponType;
        level = data.itemLevel;
    }

    public void GetReward()
    {
        UIManager.GetInstance().equipInven.GetItems(MonsterManager.GetInstance().Drop.GetDefinedWeapon(level,twohandedWP,weaponType));
    }
}
public class ArmorReward : IRewards
{
    public int amount;
    public EquipPart part;
    public ArmorMat mat;
    public sbyte level;
    

    public ArmorReward(RewardParsingData data)
    {
        amount = data.amount;
        part = data.armorPart;
        mat = data.armorMat;
        level = data.itemLevel;
    }
     
    public void GetReward()
    {
        UIManager.GetInstance().equipInven.GetItems(MonsterManager.GetInstance().Drop.GetDefinedArmor(level, mat, part));
    }
}
public class ConsumReward : IRewards
{
    public int itemCode;
    public int amount;
    public ConsumReward(RewardParsingData data)
    {
        itemCode = data.itemCode;
        amount = data.amount;
    }
    public void GetReward()
    {
        Consumables temp = ResourceManager.GetInstance().PosionDatas.GetPosion(itemCode);
        if(temp == null) return;
        UIManager.GetInstance().consumeInven.GetItems(temp);
    }
}
public class MiscReward : IRewards
{
    public int itemCode;
    public int amount;
    public MiscReward(RewardParsingData data)
    {
        itemCode = data.itemCode;
        amount = data.amount;
    }
    public void GetReward()
    {
        Miscs temp = ResourceManager.GetInstance().MiscDatas.GetMiscs(itemCode);
        if (temp == null) return;
        UIManager.GetInstance().miscInven.GetItems(temp);
    }
}
public class EXPReward : IRewards
{
    public bool isJobEXP;
    public float expValue;
    public EXPReward(RewardParsingData data)
    {
        this.isJobEXP = data.isJobExp;
        this.expValue = data.expValue;
    }

    public void GetReward()
    {
        if (isJobEXP)
        {
            Player.Instance.playerLevelInfo.GetJobEXP(this.expValue);
        }
        else
        {
            Player.Instance.playerLevelInfo.GetBaseEXP(this.expValue);
        }
    }
}
public class GoldReward : IRewards
{
    public int goldValue;
    public GoldReward(RewardParsingData data)
    {
        this.goldValue = data.amount;
    }

    public void GetReward()
    {
        UIManager.GetInstance().PlayerGold += goldValue;
    }
}
public class ClassChangeReward : IRewards
{
    public string className;
    public ClassChangeReward(RewardParsingData data)
    {
        this.className = data.className;
    }

    public void GetReward()
    {
        //TODO : PlayerInstance에 Jodata바꿔줘야함
    }
}
[System.Serializable]
public class RewardParsingData
{
    public RewardType rewardType;
    public string questCode;
    public int itemCode;
    public int amount;
    public bool isTwoHanded;
    public WeaponType weaponType;

    public EquipPart armorPart;
    public ArmorMat armorMat;
    public sbyte itemLevel;
    public bool isJobExp;
    public float expValue;
    public string className;


    public IRewards Converts(RewardParsingData data)
    {
        IRewards tempReward = null;
        switch (data.rewardType)
        {
            case RewardType.armor:
                tempReward = new ArmorReward(data);
                break;
            case RewardType.weapon:

                tempReward = new WeaponReward(data);
                break;
            case RewardType.cosumable:
                tempReward = new ConsumReward(data);
                break;
            case RewardType.gold:
                tempReward = new GoldReward(data);
                break;
            case RewardType.classChange:
                tempReward = new ClassChangeReward(data);
                break;
            case RewardType.exp:
                tempReward = new EXPReward(data);
                break;
            case RewardType.misc:
                tempReward = new MiscReward(data);
                break;
        }

        return tempReward;
    }
}
#endregion

