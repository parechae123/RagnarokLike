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
            Debug.LogError("����Ʈ �����Ͱ� �����ϴ� ��Ʈ Ȥ�� ��ũ���ͺ� ������Ʈ�� Ȯ���Ͽ� �ֽʽÿ�.");
            return;
        }
        else
        {
            (questData).GetSheetConditionValue(new TableConvert().Json(questInfoSheet), new TableConvert().Json(questInfoDetailCondition));
        }
        if(!questRewardSheet||!questData)
        {
            Debug.LogError("����Ʈ �����Ͱ� �����ϴ� ��Ʈ Ȥ�� ��ũ���ͺ� ������Ʈ�� Ȯ���Ͽ� �ֽʽÿ�.");
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
        if (GUILayout.Button("������ �Ľ�"))
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
