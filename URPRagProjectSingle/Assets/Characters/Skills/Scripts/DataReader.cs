using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

public class DataReader : MonoBehaviour
{

    [Header("��ų ������ �� ����Ʈ ����  �ּ�")]
    [SerializeField] string IconFolerPath;
    [SerializeField] string FXFolerPath;
    [Header("��ų ������ ��Ʈ �� ���̽�")]
    public DefaultAsset skillDetailSheet; // ��ų ������ ��Ʈ
    public FileStream skillDetailJson; // ��ų ������ ���̽�����
    public SkillBaseObjectOnly[] skillbaseArray = new SkillBaseObjectOnly[0];
    [Header("��ų ���� ��Ʈ �� ���̽�")]
    public DefaultAsset skillInfoSheet; // ��ų�������̼� ��Ʈ
    public FileStream skillInfoJson; // ��ų�������̼� ��Ʈ
    public SkillInfoObjectOnly[] skillInfoArray = new SkillInfoObjectOnly[0];
    [Header("���� ���(ȯ�濡 ���� ���� �ʿ�)")]
    
    public string jsonOutputPath; // JSON ������ ����� ���
    public string skillInfoScriptableObjectPath;
    public string skillDetailScriptableObjectPath;
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
        jsonOutputPath = (Application.dataPath.Replace("/", "\\")) + "\\Characters\\Skills\\DataSheet\\Json";
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
                        //����ٰ� �ٸ� Asset�ڷ��� �߰��ؼ� �������
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
        for (uint i = 0; i < skillInfoArray.Length; i++)
        {
            Debug.Log(i);
            SkillInfo skillinfo = ScriptableObject.CreateInstance<SkillInfo>();
            skillinfo.ObjectToScriptableOBJ(skillInfoArray[i]);
            string fxPath = Path.Combine(FXFolerPath, skillinfo.jobName);
            string iconPath = Path.Combine(IconFolerPath,skillinfo.jobName);
            if (!AssetDatabase.IsValidFolder(fxPath))
            {
                Directory.CreateDirectory(fxPath);
                Debug.Log("������ �����Ǿ����ϴ� ���� ��� : " + fxPath);
                Debug.Log("������ �����Ǿ����ϴ� ���� �̸� : " + skillinfo.jobName);
            }
            if (!AssetDatabase.IsValidFolder(iconPath))
            {
                Directory.CreateDirectory(iconPath);
                Debug.Log("������ �����Ǿ����ϴ� ���� ��� : " + iconPath);
                Debug.Log("������ �����Ǿ����ϴ� ���� �̸� : " + skillinfo.jobName);
            }

            iconPath = Path.Combine(iconPath, $"{skillinfo.skillName}.png");
            fxPath = Path.Combine(fxPath, $"{skillinfo.skillName}.prefab");
            skillinfo.SetSkillAsset(AssetDatabase.LoadAssetAtPath<Sprite>(iconPath),  AssetDatabase.LoadAssetAtPath<GameObject>(fxPath));
            for (uint J = 0; J < skillbaseArray.Length; J++)
            {
                if (skillInfoArray[i].skillName == skillbaseArray[J].skillName)
                {
                    SkillBase skillbase = ScriptableObject.CreateInstance<SkillBase>();
                    skillbase.ObjectToScriptableObject(skillbaseArray[J]);
                    string skillDetailPath = Path.Combine(skillDetailScriptableObjectPath, skillInfoArray[i].jobName);
                    if (!AssetDatabase.IsValidFolder(skillDetailPath))
                    {
                        Directory.CreateDirectory(skillDetailPath);
                    }
                    skillDetailPath = Path.Combine(skillDetailPath, skillInfoArray[i].skillName);
                    if (!AssetDatabase.IsValidFolder(skillDetailPath))
                    {
                        Directory.CreateDirectory(skillDetailPath);
                    }
                    skillDetailPath = Path.Combine(skillDetailPath, skillbaseArray[J].skillName + $"{skillbase.skillLevel}.asset");
                    AssetDatabase.CreateAsset(skillbase, skillDetailPath);
                    skillinfo.AddSkillDetailData(skillbase);

                    skillbase.SaveAsset();
                    
                    Debug.Log(skillInfoArray[i].skillName + skillbaseArray[J].skillLevel);
                }
            }
            skillinfo.SaveAsset();
            string skillInfoPath = Path.Combine(skillInfoScriptableObjectPath, skillInfoArray[i].jobName);
            if (!AssetDatabase.IsValidFolder(skillInfoPath))
            {
                Directory.CreateDirectory(skillInfoPath);
            }
            skillInfoPath = Path.Combine(skillInfoPath, $"{skillInfoArray[i].skillName}.asset");
            SkillInfo aleardyExistSkillInfo = AssetDatabase.LoadAssetAtPath<SkillInfo>(skillInfoPath);
            if (aleardyExistSkillInfo != null) 
            {
                aleardyExistSkillInfo.UpdateInfomation(skillinfo);
            }
            else
            {
                AssetDatabase.CreateAsset(skillinfo, skillInfoPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
            if (i < table.Rows.Count - 1) json.AppendLine(",");
        }

        json.AppendLine();
        json.AppendLine("]");

        return json.ToString();
    }
}

[CustomEditor(typeof(DataReader))]
public class SkillReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DataReader skillReader = (DataReader)target;
        if (GUILayout.Button("Generate ScriptableObject"))
        {
            skillReader.ConvertExcelToJson();
        }
    }
}


[System.Serializable]
public class SkillBaseObjectOnly
{
    public string skillName;
    public string koreanSkillName;
    public byte skillLevel;
    public float defaultValue;
    public ValueType damageType;
    public float coefficient;
    public int coolTimeTick;
    public ValueType coefficientType;
    public byte skillBound;
    public float spCost;
    public float defaultCastingTime;
    public byte skillRange;
    public int skillDuration;
    public string buffTypeOne;
    public float buffValueOne;
    public string buffTypeTwo;
    public float buffValueTwo;
    public string buffTypeThree;
    public float buffValueThree;
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
    public string flavorText;
}

#endif