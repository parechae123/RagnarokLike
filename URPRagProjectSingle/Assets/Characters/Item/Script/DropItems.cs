using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropItems : MonoBehaviour , ICameraTracker
{
    private SpriteRenderer sr;
    private SpriteRenderer SR
    {
        get 
        { 
            if(sr == null)
            {
                sr = gameObject.GetComponent<SpriteRenderer>();
                if(sr == null)
                {
                    sr = gameObject.AddComponent<SpriteRenderer>();
                    gameObject.AddComponent<SphereCollider>();
                    gameObject.layer = 7;
                }
            }
            return sr;
        }
    }
    InventoryItemBase itemInfo;
    InventoryItemBase ItemInfo
    {
        get 
        { 
            if(itemInfo == null)
            {
                //DropItemQueue를 만들어 Item이 없으면 넣어주자
            }
            return itemInfo; 
        }
        set 
        {
            if (value == null) ItemOff();//DropItemQueue를 만들어 다음 값이 null일 경우 enqueue해주어야할듯

            itemInfo = value; 
        }
    }
    public void GetItem()
    {
        switch (ItemInfo.slotType)
        {
            case SlotType.Equipments:
                UIManager.GetInstance().equipInven.GetItems((Equips)ItemInfo);
                break;
            case SlotType.ConsumableItem:
                UIManager.GetInstance().consumeInven.GetItems((Consumables)ItemInfo);
                break;
            case SlotType.MISC:
                //UIManager.GetInstance().MISCInven
                Debug.Log("기타 아이템은 아직 미구현 되었습니다");
                break;
            default:
                break;
        }
    }
    public void InitialIzeItem(InventoryItemBase itemInfo, Vector3 worldPos)
    {
        
        UnRegistCameraAction();
        ItemInfo = itemInfo;
        SR.sprite = itemInfo.IconIMG;
        transform.position = worldPos;
        RegistCameraAction();
    }
    public void ItemOff()
    {
        UnRegistCameraAction();
        gameObject.SetActive(false);
    }
    public void RegistCameraAction()
    {
        UnRegistCameraAction();
        PlayerCam.Instance.rotations += FollowCamera;
    }
    public void UnRegistCameraAction()
    {
        PlayerCam.Instance.rotations -= FollowCamera;
    }
    public void FollowCamera()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
