#region Misc
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MiscData : ItemList
{

}
[CreateAssetMenu(fileName = "Miscs", menuName = "custom/Items/Miscs", order = 0)]
public class MiscDatas : ScriptableObject, IDataFunc
{
    [SerializeField] public MiscData[] items;
    public void GetSheetValue(string json)
    {
        items = JsonConvert.DeserializeObject<MiscData[]>(json);
        EditorUtility.SetDirty(this);

        // ������Ʈ�� ����
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public Miscs GetMiscs(int itemCode)
    {
        foreach (MiscData item in items)
        {
            if (item.itemCode == itemCode)
            {
                return new Miscs(item.itemCode.ToString(), item.itemName, ResourceManager.GetInstance().ItemIconAtlas.GetSprite(item.itemName), item.goldValue);
            }
        }

        Debug.Log($"��Ÿ ������ �� {itemCode}�ڵ尡 �������� �ʽ��ϴ�");
        return null;
    }
    
}

#endregion