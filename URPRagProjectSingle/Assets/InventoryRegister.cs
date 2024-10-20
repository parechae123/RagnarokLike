using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryRegister : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RegistGameOBJ();
    }
    public void RegistGameOBJ()
    {
        foreach (KeyCode item in KeyMapManager.GetInstance().keyMaps.Keys)
        {
            if (KeyMapManager.GetInstance().keyMaps[item].UIType == UITypes.InventoryWindow)
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
    private void OnOff()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
