using PlayerDefines.Stat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : MonoBehaviour
{
    public NPCStat stat;
    public string npcName;

    // Start is called before the first frame update
    void Start()
    {
        SettingDialogData(ResourceManager.GetInstance().DialogData.dialogs);
    }

    void SettingDialogData(DialogParseData[] datas)
    {
        DialogParseData[] tempData = Array.FindAll(datas, item => item.npcName == npcName);
        if (stat.dialogStateMachine == null) stat.dialogStateMachine = new DialogStateMachine();
        string lastTitle = string.Empty;
        for (int i = 0; i < tempData.Length; i++)
        {
            if (lastTitle != tempData[i].title)
            {
                lastTitle = tempData[i].title;
                Array.Resize(ref stat.dialogStateMachine.dialogStates, stat.dialogStateMachine.dialogStates.Length+1);
                Dialog tempDial = new Dialog(Array.FindAll(tempData, item => item.title == lastTitle));
                stat.dialogStateMachine.dialogStates[stat.dialogStateMachine.dialogStates.Length-1] = new DialogState(tempDial.title,tempDial);
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
