using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyBinnder : MonoBehaviour, IPointerDownHandler
{
    private KeyBindOrganizator _main
    {
        get { return transform.parent.GetComponent<KeyBindOrganizator>(); }
    }
    public KeyCode inputKey;
    public UITypes types;
    private bool waitingInput = false;
    public bool isWaitingInput
    {
        get { return waitingInput; }
    }
    public Image bindedKeyBG
    {
        get { return transform.Find("BindedKeyBG").GetComponent<Image>(); }
    }
    public Text boundKeyText
    {
        get { return transform.Find("BoundedKey").GetComponent<Text>(); }
    }
    public void SetDefaultKey()
    {
        AutoSetting();
        boundKeyText.text = inputKey.ToString();
        ShortCutOBJ ShortCutTemp = new ShortCutOBJ { UIType = types, needCombKey = types.ToString().Contains("QuickSlot") ? false : true};
        if (types != UITypes.CombKey) KeyMapManager.GetInstance().keyMaps.Add(inputKey, ShortCutTemp);
        else KeyMapManager.GetInstance().combKey = inputKey;
    }
    public void AutoSetting()
    {
        //ÀÏÈ¸¼ºÀ¸·Î ÇÁ·ÎÆÛÆ¼³ª º¯¼ö¼±¾ð ¾ÈÇÔ
        switch (transform.Find("description").GetComponent<TextMeshProUGUI>().text)
        {
            case "UIÁ¶ÇÕÅ°":
                types = UITypes.CombKey;
                break;
            case "ÀÎº¥Åä¸®":
                types = UITypes.InventoryWindow;
                break;
            case "ÀåºñÃ¢":
                types = UITypes.EquipmentWindow;
                break;
            case "½ºÅ³Æ®¸®":
                types = UITypes.SkillTreeWindow;
                break;
            case "½ºÅÝÃ¢":
                types = UITypes.StatusWindow;
                break;
            case "Äù½ºÆ® ¸ñ·Ï":
                types = UITypes.QuestWindow;
                break;
            case "Äü½½·Ô1":
                types = UITypes.QuickSlotOne;
                break;
            case "Äü½½·Ô2":
                types = UITypes.QuickSlotTwo;
                break;
            case "Äü½½·Ô3":
                types = UITypes.QuickSlotThree;
                break;
            case "Äü½½·Ô4":
                types = UITypes.QuickSlotFour;
                break;
            case "Äü½½·Ô5":
                types = UITypes.QuickSlotFive;
                break;
            case "Äü½½·Ô6":
                types = UITypes.QuickSlotSix;
                break;
            case "Äü½½·Ô7":
                types = UITypes.QuickSlotSeven;
                break;
            case "Äü½½·Ô8":
                types = UITypes.QuickSlotEight;
                break;
            case "Äü½½·Ô9":
                types = UITypes.QuickSlotNine;
                break;

        }
    }
    public void OnPointerDown(PointerEventData pp)
    {
        if (!_main.IsOtherKeyBindingSequence)
        {
            waitingInput = true;
            _main.SetBinderColor(this);
            StartCoroutine(ReadyInputFunc());
        }
    }
    public void CheckDuplecatedKey(KeyCode newInputKey)
    {
        if (!_main.IskeyDuplicated(newInputKey))
        {

            boundKeyText.text = newInputKey.ToString();
            if (types == UITypes.CombKey)
            {
                KeyMapManager.GetInstance().combKey = newInputKey;
                inputKey = newInputKey;
                return;
            }
            ShortCutOBJ shortCutTemp;
            if (KeyMapManager.GetInstance().keyMaps.ContainsKey(inputKey))
            {
                shortCutTemp = KeyMapManager.GetInstance().keyMaps[inputKey];
            }
            else
            {
                shortCutTemp = new ShortCutOBJ { UIType = types, needCombKey = types.ToString().Contains("QuickSlot") ? false : true };
            }
            KeyMapManager.GetInstance().keyMaps.Remove(inputKey);
            KeyMapManager.GetInstance().keyMaps.Add(newInputKey, shortCutTemp);
            inputKey = newInputKey;
        }
    }
    IEnumerator ReadyInputFunc()
    {
        yield return new WaitForEndOfFrame();
        while (waitingInput)
        {

            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (code.ToString().Contains("Mouse")) continue;

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    waitingInput = false;
                    _main.SetBinderColor(null);
                    break;
                }
                if (Input.GetKeyDown(code))
                {
                    Debug.Log(code.ToString());
                    CheckDuplecatedKey(code);
                    waitingInput = false;
                    break;
                }

            }
            yield return null;
        }
        Debug.Log(KeyMapManager.GetInstance().ExportKeyMapJson());
        StopCoroutine(ReadyInputFunc());
    }
}
