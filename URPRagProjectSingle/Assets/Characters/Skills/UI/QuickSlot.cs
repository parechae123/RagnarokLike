using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour,IPointerClickHandler
{
    private ItemBase slotItem;
    private ItemBase SlotItem
    {
        get
        {
            return slotItem;
        }
        set
        {
            btn.interactable = false;
            iconImage.sprite = value.IconIMG;
            SlotText.text = value.slotNumberInfo;
            btn.interactable = true;
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
    /// <summary>
    /// 그냥 추가시
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isSwap"></param>
    public void ChangeSlot(ItemBase item)
    {
        SlotItem = item;
    }
    /// <summary>
    /// 슬롯아이템 스왑시
    /// </summary>
    /// <param name="item"></param>
    public void ChangeSlot(ref ItemBase item)
    {
        ItemBase tempItemBase = item;
        item = slotItem;
        slotItem = tempItemBase;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
    }
}
public interface ItemBase
{
    public Sprite IconIMG
    {
        get { return null; }
    }
    public string slotNumberInfo
    {
        get { return null; }
    }
    public void SlotFunction(Vector3 effectPosition)
    {
        
    }
}