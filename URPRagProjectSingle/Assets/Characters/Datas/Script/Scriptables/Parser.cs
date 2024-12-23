using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
[CreateAssetMenu(fileName = "Parser", menuName = "custom/TableParser", order = 0)]
public class Parser : ScriptableObject
{
    public ScriptableObject[] datas;
    public DefaultAsset[] assets;
    public QuestDatas questData;

    public NameDatas nameData;
    public DefaultAsset equipNameSheet;
    public DefaultAsset apixNameSheet;
    public DefaultAsset prefixNameSheet;
    public DefaultAsset uiNameSheet;
    public DefaultAsset gradeSheet;

    public DefaultAsset questInfoSheet;
    public DefaultAsset questInfoDetailCondition;
    public DefaultAsset questRewardSheet;

    public void Parse()
    {
        for (int i = 0; i < datas.Length; i++)
        {
            ((IDataFunc)datas[i]).GetSheetValue(new TableConvert().Json(assets[i]));
        }
        if(!questInfoSheet|| !questInfoDetailCondition||!questData)
        {
            Debug.LogError("퀘스트 데이터가 없습니다 시트 혹은 스크립터블 오브젝트를 확인하여 주십시오.");
            return;
        }
        else
        {
            (questData).GetSheetConditionValue(new TableConvert().Json(questInfoSheet), new TableConvert().Json(questInfoDetailCondition));
        }
        if(!questRewardSheet||!questData)
        {
            Debug.LogError("퀘스트 데이터가 없습니다 시트 혹은 스크립터블 오브젝트를 확인하여 주십시오.");
            return;
        }
        else
        {
            ((QuestDatas)questData).GetSheetRewardValue( new TableConvert().Json(questRewardSheet));
        }
    }
}
[CustomEditor(typeof(Parser))]
public class DataParser : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Parser temp = (Parser)target;
        if (GUILayout.Button("데이터 파싱"))
        {
            temp.Parse();
            temp.nameData.GetSheetValues(new TableConvert().Json(temp.equipNameSheet), nameDataType.equipName);
            temp.nameData.GetSheetValues(new TableConvert().Json(temp.apixNameSheet), nameDataType.apixName);
            temp.nameData.GetSheetValues(new TableConvert().Json(temp.prefixNameSheet),nameDataType.levelName);
            temp.nameData.GetSheetValues(new TableConvert().Json(temp.uiNameSheet),nameDataType.uiNames);
            temp.nameData.GetSheetValues(new TableConvert().Json(temp.gradeSheet),nameDataType.gradeData);
        }
    }
}
