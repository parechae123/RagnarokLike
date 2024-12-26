using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionTab : MonoBehaviour,IPointerDownHandler
{
    public OptionMain OptionMainRemote
    {
        get { return transform.parent.GetComponent<OptionMain>(); }
    }
    public OptionType tabType;
    public RectTransform targetOBJ;
    public void OnPointerDown(PointerEventData pp)
    {
        OptionMainRemote.SetOption(this);
        
        Debug.Log("´©¸§");
    }
}
public enum OptionType
{
    SoundOption,KeyMappingOption,QuestProgress,QuestDone
}
