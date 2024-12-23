using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static NameDatas;
using static UnityEditor.Progress;
[CreateAssetMenu(fileName ="NameData", menuName = "custom/Items/names", order = 0)]
public class NameDatas : ScriptableObject
{
    public NameData[] equipNames;
    public NameData[] apixNames;
    public NameData[] levelNames;
    public NameData[] uiNames;
    public GradeData[] gradeData;
    [System.Serializable]
    public class NameData
    {
        public string eng_Name;
        public string lo_ko;
    }
    [System.Serializable]
    public class GradeData
    {
        public string eng_Name;
        public string lo_ko;
        public Color32 gradeColor;
    }
    public string GetEquipNameValue(string equipName)
    {
        foreach (NameData item in equipNames)
        {
            if(item.eng_Name == equipName&&UIManager.GetInstance().languege == Localization.lo_ko)
            {
                return item.lo_ko;
            }
            else if(item.eng_Name == equipName&&UIManager.GetInstance().languege == Localization.lo_en)
            {
                return item.eng_Name;
            }
        }
        Debug.LogError("이름이 없졍" + equipName);
        return string.Empty;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="apixName">apix를 Tostring 하여 사용</param>
    /// <returns></returns>
    public string GetApixNameValue(string apixName)
    {
        foreach (NameData item in apixNames)
        {
            if (item.eng_Name == apixName && UIManager.GetInstance().languege == Localization.lo_ko)
            {
                return item.lo_ko;
            }
            else if (item.eng_Name == apixName && UIManager.GetInstance().languege == Localization.lo_en)
            {
                return item.eng_Name;
            }
        }
        Debug.LogError("이름이 없졍"+apixName);
        return string.Empty;
    }
    public string GetLevelNameValue(int level)
    {
        switch (UIManager.GetInstance().languege)
        {
            case Localization.lo_en:
                return levelNames[level].eng_Name;
            case Localization.lo_ko:
                return levelNames[level].lo_ko;
            default:
                return levelNames[level].eng_Name;
        }
        
    }
    public string GetUINameValue(string apixName)
    {
        foreach (NameData item in uiNames)
        {
            if (apixName != item.eng_Name) continue;

            if (item.eng_Name == apixName && UIManager.GetInstance().languege == Localization.lo_ko)
            {
                return item.lo_ko;
            }
            else if (item.eng_Name == apixName && UIManager.GetInstance().languege == Localization.lo_en)
            {
                return item.eng_Name;
            }
        }
        Debug.LogError("이름이 없졍" + apixName);
        return string.Empty;
    }
    public (string,Color32) GetGradeNameValue(int gradeLevel)
    {
        string returnText = string.Empty;
        if (gradeLevel >= gradeData.Length) return ("알 수 없음", new Color32(255, 255, 255, 255));
        switch (UIManager.GetInstance().languege)
        {
            case Localization.lo_en:
                returnText = gradeData[gradeLevel].eng_Name;
                break;
            case Localization.lo_ko:
                returnText = gradeData[gradeLevel].lo_ko;
                break;
            default:
                break;
        }
        return (returnText, gradeData[gradeLevel].gradeColor);
    }
    public void GetSheetValues(string json,nameDataType type)
    {
        switch (type)
        {
            case nameDataType.equipName:
                equipNames = JsonConvert.DeserializeObject<NameData[]>(json);
                break;
            case nameDataType.apixName:
                apixNames = JsonConvert.DeserializeObject<NameData[]>(json);
                break;
            case nameDataType.levelName:
                levelNames = JsonConvert.DeserializeObject<NameData[]>(json);
                break;
            case nameDataType.uiNames:
                uiNames = JsonConvert.DeserializeObject<NameData[]>(json);
                break;
            case nameDataType.gradeData:
                TempParseData[] temp = JsonConvert.DeserializeObject<TempParseData[]>(json);
                gradeData = new GradeData[temp.GetLength(0)];
                for (int i = 0;i < temp.GetLength(0); i++) 
                {
                    gradeData[i] = new GradeData();
                    gradeData[i].eng_Name = temp[i].eng_Name;
                    gradeData[i].lo_ko = temp[i].lo_ko;
                    byte r = byte.Parse(temp[i].gradeColor.Substring(0, temp[i].gradeColor.IndexOf(',')));
                    temp[i].gradeColor = temp[i].gradeColor.Remove(0, temp[i].gradeColor.IndexOf(',') + 1);
                    byte g = byte.Parse(temp[i].gradeColor.Substring(0, temp[i].gradeColor.IndexOf(',')));
                    temp[i].gradeColor = temp[i].gradeColor.Remove(0, temp[i].gradeColor.IndexOf(',') + 1);
                    byte b = byte.Parse(temp[i].gradeColor.Substring(0, temp[i].gradeColor.IndexOf(',')));
                    temp[i].gradeColor = temp[i].gradeColor.Remove(0, temp[i].gradeColor.IndexOf(',') + 1);
                    byte a = byte.Parse(temp[i].gradeColor);

                    gradeData[i].gradeColor = new Color32(r, g,b,a);
                }
                break;
        }
        EditorUtility.SetDirty(this);
        // 프로젝트에 저장
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
public enum nameDataType
{
    equipName,apixName,levelName,uiNames,gradeData
}
public class TempParseData
{
    public string eng_Name;
    public string lo_ko;
    public string gradeColor;
}