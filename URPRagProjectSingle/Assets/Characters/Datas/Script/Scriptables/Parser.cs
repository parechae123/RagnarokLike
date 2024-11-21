using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
[CreateAssetMenu(fileName = "Parser", menuName = "Items/TableParser", order = 0)]
public class Parser : ScriptableObject
{
    public ScriptableObject[] datas;
    public DefaultAsset[] assets;

    public NameDatas nameData;
    public DefaultAsset equipNameSheet;
    public DefaultAsset apixNameSheet;
    public DefaultAsset prefixNameSheet;
    
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
            temp.nameData.GetEquipSheetValue(new TableConvert().Json(temp.equipNameSheet));
            temp.nameData.GetApixSheetValue(new TableConvert().Json(temp.apixNameSheet));
            temp.nameData.GetLevelPrefixSheetValue(new TableConvert().Json(temp.prefixNameSheet));
        }
    }
}
