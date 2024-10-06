using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class KeyBindOrganizator : MonoBehaviour
{
    // Start is called before the first frame update
    public KeyBinnder[] binders = new KeyBinnder[0];
    public Color notSelected, selected;
    private void Start()
    {
        if (KeyMapManager.GetInstance().ImportkeyMapJson())
        {
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

        if (GUILayout.Button("Generate ScriptableObject"))
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