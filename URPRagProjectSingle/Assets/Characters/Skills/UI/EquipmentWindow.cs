using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentWindow : MonoBehaviour
{
    
    void Start()
    {
        RegistEquipmentSlots();
        RegistGameOBJ();
    }
    public void RegistGameOBJ()
    {
        foreach (KeyCode item in KeyMapManager.GetInstance().keyMaps.Keys)
        {
            if (KeyMapManager.GetInstance().keyMaps[item].UIType == UITypes.EquipmentWindow)
            {
                ShortCutOBJ temp = KeyMapManager.GetInstance().keyMaps[item];

                temp.subScribFuncs = null;
                temp.subScribFuncs += OnOff;
                KeyMapManager.GetInstance().keyMaps[item] = temp;
                gameObject.SetActive(false);
                break;
            }
            else
            {
                continue;
            }
        }
    }
    private void RegistEquipmentSlots()
    {
        Transform playerSiluet = transform.GetChild(0);
        UIManager.GetInstance().equipWindowWeapons = new InventorySlots[2];
        UIManager.GetInstance().equipWindowArmors = new InventorySlots[5];

        for (byte i = 0; i < playerSiluet.childCount; i++)
        {
            for (byte j = 0; j < playerSiluet.GetChild(i).childCount; j++)
            {
                if (i == 0)
                {
                    UIManager.GetInstance().equipWindowWeapons[j] = playerSiluet.GetChild(i).GetChild(j).GetComponent<InventorySlots>();
                }
                else if(i== 1)
                {
                    UIManager.GetInstance().equipWindowArmors[j] = playerSiluet.GetChild(i).GetChild(j).GetComponent<InventorySlots>();
                }
            }
        }
    }
    private void OnOff()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
