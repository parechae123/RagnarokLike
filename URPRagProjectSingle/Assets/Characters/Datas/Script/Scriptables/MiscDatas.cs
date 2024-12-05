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

        // 프로젝트에 저장
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

        Debug.Log($"기타 아이템 중 {itemCode}코드가 존재하지 않습니다");
        return null;
    }
    
}

#endregion