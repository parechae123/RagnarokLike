using PlayerDefines.Stat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : MonoBehaviour
{
    [SerializeField]public NPCStat stat;
    public string npcName;

    // Start is called before the first frame update
    void Start()
    {
        stat = new NPCStat(GridManager.GetInstance().PositionToNode(transform.position), float.MaxValue, float.MaxValue, 0, 0, 0, 0, 0);
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
                Dialog tempDial = new Dialog(Array.FindAll(tempData, item => item.title == lastTitle));
                if (stat.dialogStateMachine.dialogStates == null) stat.dialogStateMachine.dialogStates = new DialogState[0];
                Array.Resize(ref stat.dialogStateMachine.dialogStates, stat.dialogStateMachine.dialogStates.Length+1);
                stat.dialogStateMachine.dialogStates[stat.dialogStateMachine.dialogStates.Length-1] = new DialogState(tempDial);
            }
            else
            {
                continue;
            }
        }
        
    }
}
