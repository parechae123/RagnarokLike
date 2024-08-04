using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour,IPointerClickHandler
{
    private SlotItem slotItem;
    private SlotItem SlotItem
    {
        get
        {
            return slotItem;
        }
        set
        {
            iconImage.sprite = value.IconIMG;
            SlotText.text = value.slotNumberInfo;
            slotItem = value;
        }
    }
    private Button btn
    {
        get;
        set;
    }
    private Image iconImage
    {
        get 
        { 
            btn ??= transform.GetComponentInChildren<Button>();
            return btn.image;
        }
    }
    private Text slotText
    {
        get;
        set;
    }
    private Text SlotText
    {
        get 
        { 
            slotText ??= transform.GetComponentInChildren<Text>();
            return slotText;
        }
    }
    private void Awake()
    {
        btn = transform.GetComponentInChildren<Button>();
        iconImage.sprite = slotItem.IconIMG;
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
public interface SlotItem
{
    public virtual Sprite IconIMG
    {
        get { return null; }
    }
    public virtual string slotNumberInfo
    {
        get { return null; }
    }
    public virtual void SlotFunction(Vector3 effectPosition)
    {
        
    }
}