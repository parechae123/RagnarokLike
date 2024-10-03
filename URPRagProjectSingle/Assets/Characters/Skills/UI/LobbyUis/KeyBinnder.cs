using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyBinnder : MonoBehaviour,IPointerDownHandler
{
    private KeyBindOrganizator _main
    {
        get { return transform.parent.GetComponent<KeyBindOrganizator>(); }
    }
    public KeyCode inputKey;
    public UITypes types;
    private bool watingInput = false;
    void Start()
    {
        AutoSetting();
    }
    public void AutoSetting()
    {
        switch (transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
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
        watingInput = true;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (watingInput)
        {
            
        }
    }
}
