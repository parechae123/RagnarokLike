using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionBTN : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform optionPannel;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            optionPannel.gameObject.SetActive(!optionPannel.gameObject.activeSelf);
        });
    }
}
