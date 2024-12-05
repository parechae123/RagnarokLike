using DG.Tweening;
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
        Release();
    }
    public void InitialIzeItem(InventoryItemBase itemInfo, Vector3 worldPos)
    {
        transform.rotation = Camera.main.transform.rotation;
        UnRegistCameraAction();
        ItemInfo = itemInfo;
        SR.sprite = itemInfo.IconIMG;
        if (transform.parent == null) 
        {
            Transform temp = new GameObject("ItemParent").transform;
            temp.localScale = Vector3.one * 0.4f;
            temp.parent = null;
            transform.parent = temp;
            transform.localPosition = Vector3.up * 0.4f;
            transform.localScale = Vector3.one;
        }
        else
        {
            
            transform.parent.gameObject.SetActive(true);
        }
        transform.parent.DOKill();
        transform.parent.position = worldPos + Vector3.up;
        transform.parent.DOPath(new Vector3[3] { worldPos + Vector3.up, worldPos + (Vector3.up * 0.7f), worldPos }, 0.4f);

        RegistCameraAction();
    }
    public void Release()
    {
        
        UnRegistCameraAction();
        ItemInfo = null;
        SR.sprite = null;
        transform.parent.gameObject.SetActive(false);
        MonsterManager.GetInstance().Drop.items.Enqueue(this);
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
