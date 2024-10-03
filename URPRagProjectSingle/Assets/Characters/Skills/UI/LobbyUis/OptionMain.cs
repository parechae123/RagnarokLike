using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class OptionMain : MonoBehaviour
{
    public Color selected, notSelected;
    public OptionTab[] optionTabs;
    private void Awake()
    {
        //초기값 세팅
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<OptionTab>(out OptionTab result))
            {
                Array.Resize(ref optionTabs, optionTabs.Length+1);
                optionTabs[optionTabs.Length - 1] = result;
                optionTabs[i].targetOBJ.gameObject.SetActive(false);
            }
        }
        SetOption(optionTabs[0]);
    }
    public void SetOption(OptionTab tab)
    {
        foreach (OptionTab item in optionTabs)
        {
            if (item == tab)
            {
                item.GetComponent<Image>().color = selected;
                item.targetOBJ.gameObject.SetActive(true);
                continue;
            }
            else
            {
                item.GetComponent<Image>().color = notSelected;
                item.targetOBJ.gameObject.SetActive(false);
            }

        }
    }
}
