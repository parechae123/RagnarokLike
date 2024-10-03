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
            case "UI����Ű":
                types = UITypes.CombKey;
                break;
            case "�κ��丮":
                types = UITypes.InventoryWindow;
                break;
            case "���â":
                types = UITypes.EquipmentWindow;
                break;
            case "��ųƮ��":
                types = UITypes.SkillTreeWindow;
                break;
            case "����â":
                types = UITypes.StatusWindow;
                break;
            case "������1":
                types = UITypes.QuickSlotOne;
                break;
            case "������2":
                types = UITypes.QuickSlotTwo;
                break;
            case "������3":
                types = UITypes.QuickSlotThree;
                break;
            case "������4":
                types = UITypes.QuickSlotFour;
                break;
            case "������5":
                types = UITypes.QuickSlotFive;
                break;
            case "������6":
                types = UITypes.QuickSlotSix;
                break;
            case "������7":
                types = UITypes.QuickSlotSeven;
                break;
            case "������8":
                types = UITypes.QuickSlotEight;
                break;
            case "������9":
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
