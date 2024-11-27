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

    public NameDatas nameData;
    public DefaultAsset equipNameSheet;
    public DefaultAsset apixNameSheet;
    public DefaultAsset prefixNameSheet;
    public DefaultAsset uiNameSheet;
    public DefaultAsset gradeSheet;
    
    public void Parse()
    {
        for (int i = 0; i < datas.Length; i++)
        {
            ((IDataFunc)datas[i]).GetSheetValue(new TableConvert().Json(assets[i]));
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
        if (GUILayout.Button("µ¥ÀÌÅÍ ÆÄ½Ì"))
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
