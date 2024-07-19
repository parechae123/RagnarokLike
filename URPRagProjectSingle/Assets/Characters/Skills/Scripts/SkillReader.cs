using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class SkillReader : MonoBehaviour
{
    [Header("��ų ������ ��Ʈ �� ���̽�")]
    public DefaultAsset skillDetailSheet; // ��ų ������ ��Ʈ
    public FileStream skillDetailJson; // ��ų ������ ���̽�����
    public SkillBaseObjectOnly[] skillbaseArray = new SkillBaseObjectOnly[0];
    [Header("��ų ���� ��Ʈ �� ���̽�")]
    public DefaultAsset skillInfoSheet; // ��ų�������̼� ��Ʈ
    public FileStream skillInfoJson; // ��ų�������̼� ��Ʈ
    public SkillInfoObjectOnly[] skillInfoArray = new SkillInfoObjectOnly[0];
    [Header("���� ���")]
    public string jsonOutputPath; // JSON ������ ����� ���

    public void ConvertExcelToJson()
    {
        StartConvert(skillDetailSheet);
        StartConvert(skillInfoSheet);
//        Debug.Log(new StreamReader(skillInfoJson).ReadToEnd());
        skillInfoArray = JsonConvert.DeserializeObject<SkillInfoObjectOnly[]>(new StreamReader(skillInfoJson).ReadToEnd());
//        Debug.Log(new StreamReader(skillDetailJson).ReadToEnd());
        skillbaseArray = JsonConvert.DeserializeObject<SkillBaseObjectOnly[]>(new StreamReader(skillDetailJson).ReadToEnd());
        ConvertToScriptableOBJ();
        skillInfoJson.Dispose();
        skillDetailJson.Dispose();

    }
    public void StartConvert(DefaultAsset targetExcel)
    {
        if (targetExcel == null)
        {
            Debug.LogError("No Excel file assigned.");
            return;
        }

        string filePath = AssetDatabase.GetAssetPath(targetExcel);

        try
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();

                    foreach (DataTable table in result.Tables)
                    {
                        string json = DataTableToJson(table);


                        string jsonFilePath = Path.Combine(jsonOutputPath, $"{targetExcel.name}.json");
                        File.WriteAllText(jsonFilePath, json);
                        if (targetExcel == skillDetailSheet) skillDetailJson = File.Open(jsonFilePath, FileMode.Open);
                        else skillInfoJson = File.Open(jsonFilePath, FileMode.Open);
                    }

                    Debug.Log("Excel file successfully converted to JSON.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while converting Excel to JSON: {ex.Message}");
        }
    }
    public void ConvertToScriptableOBJ()
    {
        for (uint i = 0; i<skillInfoArray.Length; i++)
        {
            Debug.Log(i);
            for (uint J = 0; J < skillbaseArray.Length; J++)
            {
                if (skillInfoArray[i].skillName == skillbaseArray[J].skillName)
                {
                    Debug.Log(skillInfoArray[i].skillName + skillbaseArray[J].skillLevel);
                    //���⼭ ��ũ���ͺ� ������Ʈ �������ָ� �� �� ��
                }
            }
        }
    }


    private string DataTableToJson(DataTable table)
    {
        var json = new StringBuilder();
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
            if (i < table.Rows.Count - 1)json.AppendLine(",");
        }

        json.AppendLine();
        json.AppendLine("]");

        return json.ToString();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(SkillReader))]
public class SkillReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SkillReader skillReader = (SkillReader)target;
        if (GUILayout.Button("Generate ScriptableObject"))
        {
            skillReader.ConvertExcelToJson();
        }
    }
}
#endif

[System.Serializable]
public class SkillBaseObjectOnly
{
    public string skillName;
    public string koreanSkillName;
    public byte skillLevel;
    public float defaultValue;
    public ValueType damageType;
    public float coefficient;
    public ValueType coefficientType;
    public float skillBound;
    public float spCost;
    public float defaultCastingTime;
    public byte skillRange;
    public float skillDuration;
}
[System.Serializable]
public class SkillInfoObjectOnly
{
    public string skillName;
    public byte maxSkillLevel;
    public string jobName;
    public SkillType skillType;
    public ObjectiveType objectiveType;
    public SkillPosition skillPosition;
}
