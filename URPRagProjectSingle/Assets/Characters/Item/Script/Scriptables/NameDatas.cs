using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static NameDatas;
using static UnityEditor.Progress;
[CreateAssetMenu(fileName ="NameData", menuName = "Items/names", order = 0)]
public class NameDatas : ScriptableObject
{
    public EquipNames[] equipNames;
    public ApixName[] apixNames;
    public LevelName[] levelNames;
    [System.Serializable]
    public class EquipNames
    {
        public string eng_Name;
        public string lo_Name;
    }
    [System.Serializable]
    public class ApixName
    {
        public string eng_Name;
        public string lo_Name;
    }
    [System.Serializable]
    public class LevelName
    {
        public string eng_Name;
        public string lo_Name;
    }
    public string GetEquipNameValue(string equipName)
    {
        foreach (var item in equipNames)
        {
            if(item.eng_Name == equipName)
            {
                return item.lo_Name;
            }
        }
        Debug.LogError("�̸��� ����" + equipName);
        return string.Empty;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="apixName">apix�� Tostring �Ͽ� ���</param>
    /// <returns></returns>
    public string GetApixNameValue(string apixName)
    {
        foreach (var item in apixNames)
        {
            if(item.eng_Name == apixName)
            {
                return item.lo_Name;
            }
        }
        Debug.LogError("�̸��� ����"+apixName);
        return string.Empty;
    }
    public string GetLevelNameValue(int level)
    {
        return levelNames[level].lo_Name;
    }
    public void GetEquipSheetValue(string json)
    {
        equipNames = JsonConvert.DeserializeObject<EquipNames[]>(json);
        EditorUtility.SetDirty(this);

        // ������Ʈ�� ����
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public void GetApixSheetValue(string json)
    {
        apixNames = JsonConvert.DeserializeObject<ApixName[]>(json);
        EditorUtility.SetDirty(this);

        // ������Ʈ�� ����
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public void GetLevelPrefixSheetValue(string json)
    {
        levelNames = JsonConvert.DeserializeObject<LevelName[]>(json);
        EditorUtility.SetDirty(this);

        // ������Ʈ�� ����
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
