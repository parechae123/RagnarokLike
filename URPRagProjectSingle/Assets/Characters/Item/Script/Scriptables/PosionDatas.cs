using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using ExcelDataReader;
using System.Data;
using System.IO;
using System;
using System.Text;
#region Posion
    [CreateAssetMenu(fileName = "Cosumes", menuName = "Items/Cosumes", order = 0)]
    public class PosionDatas : ScriptableObject
    {
        [SerializeField] public PosionData[] items;
        public DefaultAsset sheet;
        public void GetSheetValue()
        {
            items = JsonConvert.DeserializeObject<PosionData[]>(new TableConvert().Json(sheet));
            EditorUtility.SetDirty(this);

            // 프로젝트에 저장
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    public class TableConvert
    {
        public string Json(DefaultAsset sheet)
        {
            if (sheet == null)
            {
                Debug.LogError("No Excel file assigned.");
                return string.Empty;
            }
            string jsonOutputPath = (Application.dataPath.Replace("/", "\\")) + "\\Characters\\Skills\\DataSheet\\Json";
            string filePath = AssetDatabase.GetAssetPath(sheet);
            var json = new StringBuilder();
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();

                        foreach (DataTable table in result.Tables)
                        {

                            if (table.Rows.Count <= 0) continue;
                            json.AppendLine("[");
                            for (int i = 1; i < table.Rows.Count; i++)  // Assuming first row is the headers
                            {
                                json.AppendLine("{");

                                for (int j = 0; j < table.Columns.Count; j++)
                                {
                                    json.Append($"\"{table.Rows[0][j]}\": \"{table.Rows[i][j]}\"");
                                    if (j < table.Columns.Count - 1)
                                        json.Append(",");
                                    json.AppendLine();
                                }

                                json.Append("}");
                                if (i < table.Rows.Count - 1) json.AppendLine(",");
                            }

                            json.AppendLine();
                            json.AppendLine("]");

                            string jsonFilePath = Path.Combine(jsonOutputPath, $"{sheet.name}.json");
                            File.WriteAllText(jsonFilePath, json.ToString());
                            //여기다가 다른 Asset자료형 추가해서 넣으면됨
                        }

                        Debug.Log("Excel file successfully converted to JSON.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while converting Excel to JSON: {ex.Message}");
            }
            return json.ToString();
        }
    }

    [CustomEditor(typeof(PosionDatas))]
    public class CosumeSCOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PosionDatas temp = (PosionDatas)target;
            if (GUILayout.Button("소모품 파싱"))
            {
                temp.GetSheetValue();
            }
        }
    }
    [System.Serializable]
    public class PosionData : ItemList
    {
        
        public PotionType potionType;
        public float valueOne;
    } 
#endregion
[System.Serializable]
public class ItemList 
{
    public int itemCode;
    public string itemName;
    public float goldValue;
    public string iconName;
}

