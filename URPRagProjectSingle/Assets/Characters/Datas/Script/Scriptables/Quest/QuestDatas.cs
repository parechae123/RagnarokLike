using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using static ScriptableQuest;
using static UnityEditor.Progress;
[CreateAssetMenu(fileName = "QuestData", menuName = "custom/QuestData", order = 0)]
public class QuestDatas : ScriptableObject
{
    [SerializeField] public ScriptableQuest[] questData;
    public Quest GetQuest(string questID)
    {
        foreach (ScriptableQuest item in questData)
        {
            if (questID == item.questID) return new Quest(item);
        }
        return null;
    }
    public int GetQuestArr(string questID)
    {
        for (int i = 0; i < questData.Length; i++)
        {
            if (questData[i].questID == questID) return i;
        }
        return -1;
    }
    public void GetSheetConditionValue(string sheetJson,string conditionJson)
    {
        questData = JsonConvert.DeserializeObject<ScriptableQuest[]>(sheetJson);
        Queue<ScriptableQuestCondition> conditions = new Queue<ScriptableQuestCondition>(JsonConvert.DeserializeObject<ScriptableQuestCondition[]>(conditionJson));
        while (conditions.Count > 0)
        {
            ScriptableQuestCondition currCondition = conditions.Dequeue();
            for (int i = 0; i< questData.Length; i++)
            {
                if (questData[i].condition == null) questData[i].condition = new IQuestConditions[0];

                if (questData[i].questID == currCondition.parentQuestName)
                {
                    if(currCondition.arr >= questData[i].condition.Length)
                    {
                        Array.Resize(ref questData[i].condition, currCondition.arr + 1);
                    }
                    questData[i].condition[currCondition.arr] = currCondition.Convert();
                    currCondition = null;
                    break;
                }
            }
            if(currCondition != null) Debug.LogError($"누락된 컨디션이 있습니다 : {currCondition.parentQuestName}퀘스트 ||{currCondition.arr}번째 컨디션");
        }
        EditorUtility.SetDirty(this);

        // 프로젝트에 저장
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public void GetSheetRewardValue(string rewardJson)
    {
        Queue<RewardParsingData> rewardData = new Queue<RewardParsingData>(JsonConvert.DeserializeObject<RewardParsingData[]>(rewardJson));
        while (rewardData.Count > 0)
        {
            RewardParsingData currRWdata = rewardData.Dequeue();
            for (int i = 0; i< questData.Length; i++)
            {
                if (questData[i].rewards == null) questData[i].rewards = new IRewards[0];

                if (questData[i].questID == currRWdata.questCode)
                {
                    Array.Resize(ref questData[i].rewards, questData[i].rewards.Length + 1);
                    questData[i].rewards[questData[i].rewards.Length-1] = currRWdata.Converts(currRWdata);
                    currRWdata = null;
                    break;
                }
            }
            if(currRWdata != null) Debug.LogError($"누락된 컨디션이 있습니다 : {currRWdata.questCode}퀘스트 ||{currRWdata.rewardType}타입");
        }
        EditorUtility.SetDirty(this);

        // 프로젝트에 저장
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
[CustomEditor(typeof(QuestDatas))]
public class CheckQuestCondition : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        QuestDatas data = (QuestDatas)target;
        if(GUILayout.Button("컨디션값 확인"))
        {
            for (int i = 0; i < data.questData.Length; i++)
            {
                for (int j = 0; j < data.questData[i].condition.Length; j++)
                {
                    Debug.Log(data.questData[i].condition[j]);
                }
            }
        }
    }
}
[System.Serializable]
public class ScriptableQuest
{
    public string questID;
    public string questName;
    public string description;
    public int level;
    [SerializeReference]public IQuestConditions[] condition;
    [SerializeReference]public IRewards[] rewards;
    
}

public class ScriptableQuestCondition
{
    public QuestType questType;
    public string parentQuestName;
    public string targetCode;
    public int arr;
    public int valueOne;        //해당 위치에 maxValue

    public IQuestConditions Convert()
    {
        switch (questType)
        {
            case QuestType.Hunting:
                return new HuntingCondition(targetCode, valueOne);
            case QuestType.Collection:
                return new CollectionCondition(targetCode, valueOne);
            case QuestType.Interaction:
                return new InteractionCondition(targetCode, valueOne);

            case QuestType.Conversation:
                return new ConversationCondition(targetCode, valueOne,parentQuestName);
        }
        Debug.LogError("파일을 못 불러왔습니다.");
        return null;
    }
}