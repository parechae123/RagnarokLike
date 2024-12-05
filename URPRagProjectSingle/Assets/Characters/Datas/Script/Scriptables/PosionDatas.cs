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
using static UnityEditor.Progress;
#region Posion
[CreateAssetMenu(fileName = "Cosumes", menuName = "custom/Items/Cosumes", order = 0)]
public class PosionDatas : ScriptableObject, IDataFunc
{
    [SerializeField] public PosionData[] items;
    public void GetSheetValue(string json)
    {
        items = JsonConvert.DeserializeObject<PosionData[]>(json);
        EditorUtility.SetDirty(this);

        // 프로젝트에 저장
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public Potions GetPosion(int itemCode)
    {
        foreach (PosionData item in items)
        {
            if(item.itemCode == itemCode) 
            {
                return new Potions(item.itemCode.ToString(), item.itemName, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(item.itemName), item.goldValue, item.potionType, item.valueOne);
            }
        }
        Debug.Log($"포션 아이템 중 {itemCode}코드가 존재하지 않습니다");
        return null;
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
public interface IDataFunc
{
    void GetSheetValue(string json);
}
