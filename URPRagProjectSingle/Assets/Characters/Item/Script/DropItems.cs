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
                //DropItemQueue�� ����� Item�� ������ �־�����
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
                Debug.Log("��Ÿ �������� ���� �̱��� �Ǿ����ϴ�");
                break;
            default:
                break;
        }
        Release();
    }
    public void InitialIzeItem(InventoryItemBase itemInfo, Vector3 worldPos)
    {
        gameObject.SetActive(true);
        UnRegistCameraAction();
        ItemInfo = itemInfo;
        SR.sprite = itemInfo.IconIMG;
        transform.position = worldPos;
        RegistCameraAction();
    }
    public void Release()
    {
        
        UnRegistCameraAction();
        ItemInfo = null;
        SR.sprite = null;
        gameObject.SetActive(false);
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
