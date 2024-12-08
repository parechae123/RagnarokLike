using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CreateAssetMenu(fileName ="dialogData", menuName = "custom/DialogData", order = 0)]
public class DialogData : ScriptableObject, IDataFunc
{
    public DialogParseData[] dialogs;
    public void GetSheetValue(string Json)
    {
        dialogs = JsonConvert.DeserializeObject<DialogParseData[]>(Json); 
        EditorUtility.SetDirty(this);

        // ������Ʈ�� ����
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
public enum DialogType
{
    dialog,greeting,leaveTalk,quest
}
[System.Serializable]
public class DialogParseData
{
    public DialogType type;
    public string npcName;
    public int curArr;
    public string title;
    public string text;
    public string questCode;

}
[System.Serializable]
public class Dialog
{
    public DialogType type;
    [SerializeField]public TextData[] textData;
    public int questArr;        //�Ҵ�� ����Ʈ�� ������ -1
    public string title;
    public bool isInteractedLog = false;
    public Dialog(DialogParseData[] parseData)
    {
        textData = new TextData[parseData.Length];
        int arr = -1;
        if (parseData[0].questCode != string.Empty)
        {
            arr = ResourceManager.GetInstance().QuestData.GetQuestArr(parseData[0].questCode);
        }
        questArr = arr;
        title = parseData[0].title;
        for (int i = 0; i < parseData.Length; i++)
        {
            if (parseData[i].title != title) break;
            type = parseData[i].type;
            textData[i] = new TextData();
            textData[i].curArr = parseData[i].curArr;
            textData[i].text = parseData[i].text;
        }
    }
       
}
[System.Serializable]
public struct TextData
{

    public int curArr;
    public string text;
}
//title�� ��������