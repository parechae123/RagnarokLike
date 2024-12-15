using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
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
using UnityEngine.Android;
using UnityEngine.Rendering;

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
        rewards = questInfo.rewards;
    }
    public void QuestClear()
    {
        isQuestDone = true;
        for (int i = 0; i < rewards.Length; i++)
        {
            rewards[i].GetReward();
        }
    }
    [SerializeField] private List<IQuestConditions> conditions;
    [SerializeField] public List<IQuestConditions> Conditions
    {
        get { return conditions; }
    }

    private IRewards[] rewards;
    public int GetCurrCondition()
    {
        for (int i = 0; i < Conditions.Count; i++)
        {
            if (Conditions[i].IsMet()) return i;
        }
        return 0;
    }
    public void ConditionUpdate()
    {
        IQuestConditions[] tempConditions = Conditions.FindAll(c => !c.IsMet()).ToArray();
        if (tempConditions.Length > 0) return;
        if (tempConditions[0].GetType() != typeof(CollectionCondition))
        {
            tempConditions[0]?.Intialize();
        }
        else
        {
            for (int i = 0; i < tempConditions.Length; i++)
            {
                if(tempConditions[i].GetType() == typeof(CollectionCondition))
                {
                    tempConditions[i]?.Intialize();
                    continue;
                }
                else if(tempConditions[i].GetType() == typeof(ConversationCondition))
                {
                    tempConditions[i]?.Intialize();
                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }
    public IQuestConditions[] SubmitCollection(ConversationCondition conversationCondition)
    {
        int tempNum = Conditions.IndexOf(conversationCondition);
        if (tempNum < 0) return null;

        return Conditions.GetRange(0, tempNum + 1).FindAll((c)=> c.GetType() ==typeof(CollectionCondition)).ToArray();
    }
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

    void RegistAction();
    void RemoveAction();
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

    public bool IsMet()=>curr >= require;
    public void Intialize()
    {
        
        RegistAction();
        curr = 0;
    }
    public void RegistAction()
    {
        QuestManager.GetInstance().huntEvent += Hunt;
        //TODO : 여기에 UI 업데이트 함수로 huntEvent에 같이 등록해줘야 할듯?
    }
    public void RemoveAction()
    {
        if (!IsMet()) return;
        QuestManager.GetInstance().huntEvent -= Hunt;
    }
    public float GetProgress() => (float)curr / require;
    public string GetDescription() => $"Slay {targetCode} {curr}/{require}";
    public void Hunt(string mobCode)
    {
        if (mobCode != targetCode) return;
        curr++;
        if(IsMet()) RemoveAction();
    }
}
[System.Serializable]
public class CollectionCondition : IQuestConditions
{
    public string itemCode;
    public sbyte require;
    public sbyte curr = 0;
    public CollectionCondition(string itemCode,int require) 
    {
        
        this.itemCode = itemCode;
        this.require = (sbyte)require;
    }

    public bool IsMet() => curr >= require;
    public void Intialize()=> curr = 0;
    public float GetProgress() => (float)curr / require;
    public string GetDescription() => $"Collect {itemCode}s {curr}/{require}";

    public void RemoveAction()
    {
        if (!IsMet()) return;
        QuestManager.GetInstance().collectEvent -= Collect;
        UIManager.GetInstance().consumeInven.RemoveItem(itemCode, require);
    }
    public void RegistAction()
    {
        QuestManager.GetInstance().collectEvent += Collect;
    }
    public void Collect(string itemCode)
    {
        int itemAmount = UIManager.GetInstance().consumeInven.GetAmount(itemCode);
        curr = itemAmount > sbyte.MaxValue? sbyte.MaxValue : (sbyte)itemAmount;
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

    public bool IsMet() => curr >= require;
    public void Intialize()
    {
        
        curr = 0;
    }
    public void RegistAction()
    {
        QuestManager.GetInstance().interactiveEvent += interact;
    }
    public void RemoveAction()
    {
        QuestManager.GetInstance().interactiveEvent -= interact;
    }
    public float GetProgress() => (float)curr / require;
    public string GetDescription() => $"interact {interactCode} {require} times";
    public void interact(string objCode)
    {
        if (objCode != interactCode) return;
        curr++;
        if(IsMet()) RemoveAction();
    }
}
[System.Serializable]
public class ConversationCondition : IQuestConditions
{
    public string npcCode;
    public int dialogueIndex;
    private bool isDone;
    public string questName;
    public ConversationCondition(string npcCode,int dialogueIndex,string questName)
    {
        this.npcCode = npcCode;
        this.dialogueIndex = dialogueIndex;
        this.questName = questName;
    }

    public bool IsMet() => isDone;
    public void Intialize()
    {
        isDone = false;
        RegistAction();
    }
    public float GetProgress() => isDone ? 1:0;
    public string GetDescription() => $"convasation with {npcCode}";
    public void RegistAction()
    {
        QuestManager.GetInstance().conversationEvent += CheckCondition;
    }
    public void RemoveAction()
    {
        QuestManager.GetInstance().conversationEvent -= CheckCondition;
    }
    public void CheckCondition(string npcName)
    {
        if (npcName == npcCode) return;
        IQuestConditions[] tempCondition = QuestManager.GetInstance().AcceptedQuests.Find(q=> q.questName == questName).SubmitCollection(this);
        if (tempCondition.All(c => c.IsMet()))
        {
            for (int i = 0; i < tempCondition.Length; i++)
            {
                tempCondition[i].RemoveAction();
            }
        }
        else return;
        isDone = true;
        RemoveAction();
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

