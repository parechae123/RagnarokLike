using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class KeyBindOrganizator : MonoBehaviour
{
    // Start is called before the first frame update
    public KeyBinnder[] binders = new KeyBinnder[0];
    public Color notSelected, selected;
    public void ReadOrCreateKeyBind()
    {
        if (KeyMapManager.GetInstance().ImportkeyMapJson())
        {
            Queue<(KeyCode,ShortCutOBJ)> dataQueue = new Queue<(KeyCode, ShortCutOBJ)>();
            KeyCode[] temp = KeyMapManager.GetInstance().ConvertKeyArray();
            for (int i = 0; i < KeyMapManager.GetInstance().keyMaps.Count; i++)
            {
                dataQueue.Enqueue((temp[i], KeyMapManager.GetInstance().keyMaps[temp[i]]));
            }
            Queue<KeyBinnder> binderQueue = new Queue<KeyBinnder>(binders);
            int tester = 0;
            while (binderQueue.Count > 0 &&dataQueue.Count > 0)
            {
                binderQueue.TryPeek(out KeyBinnder key);
                dataQueue.TryPeek(out (KeyCode,ShortCutOBJ) DQ);
                tester++;
                if (tester > 1000) break;
                
                if(key.types == UITypes.CombKey)
                {
                    KeyBinnder tempBinder = binderQueue.Dequeue();
                    tempBinder.inputKey = KeyMapManager.GetInstance().combKey;
                    tempBinder.boundKeyText.text = KeyMapManager.GetInstance().combKey.ToString();
                }

                if (key.types != DQ.Item2.UIType) 
                { 
                    dataQueue.Enqueue(dataQueue.Dequeue());
                }
                else
                {
                    KeyBinnder tempBinder = binderQueue.Dequeue();
                    (KeyCode, ShortCutOBJ) tempOBJ = dataQueue.Dequeue();
                    tempBinder.types = tempOBJ.Item2.UIType;
                    tempBinder.inputKey = tempOBJ.Item1;
                    tempBinder.boundKeyText.text = tempOBJ.Item1.ToString();
                }
            }

            //TODO : 세팅값과 UI 연동 필요
            return;
        }
        for (int i = 0; i < binders.Length; i++)
        {
            binders[i].SetDefaultKey();
        }
    }
    public bool IsOtherKeyBindingSequence
    {
        get { return binders.Max(item => item.isWaitingInput); }
    }
    public void SetBinderColor(KeyBinnder temp)
    {
        for (int i = 0; i < binders.Length; i++)
        {
            if (temp == binders[i]) temp.bindedKeyBG.color = selected;
            else binders[i].bindedKeyBG.color = notSelected;

        }
    }
    /// <summary>
    /// false를 리턴 할 시 중복된 키가 없다는것
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public bool IskeyDuplicated(KeyCode sender)
    {
        SetBinderColor(null);
        for (int i = 0; i < binders.Length; i++)
        {
            if (binders[i].isWaitingInput) continue;
            else if (binders[i].inputKey == sender) return true;
        }

        return false;
    }
}
[CustomEditor(typeof(KeyBindOrganizator))]
public class BindOrganizator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        KeyBindOrganizator KBO = (KeyBindOrganizator)target;

        if (GUILayout.Button("Find KeyBinders"))
        {
            KBO.binders = new KeyBinnder[KBO.transform.childCount];
            for (int i = 0; i < KBO.transform.childCount; i++)
            {
                KBO.binders[i] = KBO.transform.GetChild(i).GetOrAddComponent<KeyBinnder>();
                KBO.binders[i].AutoSetting();
            }
        }
    }
}