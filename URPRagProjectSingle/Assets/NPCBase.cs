using PlayerDefines.Stat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : MonoBehaviour
{
    public NPCStat stat;
    public string npcName;
    [SerializeField]public Dialog[] dialogDatas;
    public (int, int) currDiaAddy;
    // Start is called before the first frame update
    void Start()
    {
        SettingDialogData(ResourceManager.GetInstance().DialogData.dialogs);
    }

    void SettingDialogData(DialogParseData[] datas)
    {
        DialogParseData[] tempData = Array.FindAll(datas, item => item.npcName == npcName);
        if (dialogDatas == null) dialogDatas = new Dialog[0];
        string lastTitle = string.Empty;
        for (int i = 0; i < tempData.Length; i++)
        {
            if (lastTitle != tempData[i].title)
            {
                lastTitle = tempData[i].title;
                Array.Resize(ref dialogDatas, dialogDatas.Length+1);
                dialogDatas[dialogDatas.Length - 1] = new Dialog(Array.FindAll(tempData, item => item.title == lastTitle));
            }
            else
            {
                continue;
            }
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
