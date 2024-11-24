using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using static UnityEditor.Progress;
[CreateAssetMenu(fileName = "QuestData", menuName = "custom/QuestData", order = 0)]
public class QuestDatas : ScriptableObject, IDataFunc
{
    [SerializeField] ScriptableQuest[] questData;
    public void GetSheetValue(string sheetJson)
    {
        questData = JsonConvert.DeserializeObject<ScriptableQuest[]>(sheetJson);
        EditorUtility.SetDirty(this);

        // 프로젝트에 저장
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
[System.Serializable]
public class ScriptableQuest
{
    public string questID;
    public string questName;
    public string description;
    public int level;

    [System.Serializable]
    class ScriptableQuestCondition
    {
        public QuestType questType;
        public string targetCode;
        public int valueOne;        //해당 위치에 maxValue

        public IQuestConditions Convert()
        {
            switch (questType)
            {
                case QuestType.Hunting:
                    return new HuntingCondition(targetCode,valueOne);
                case QuestType.Collection:
                    return new CollectionCondition(targetCode, valueOne);
                case QuestType.Interaction:
                    return new InteractionCondition(targetCode,valueOne);
                    
                case QuestType.Conversation:
                    return new ConversationCondition(targetCode, valueOne);
            }
            Debug.LogError("파일을 못 불러왔습니다.");
            return null;
        }
    }
}