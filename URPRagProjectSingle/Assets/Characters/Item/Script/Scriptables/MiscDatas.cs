#region Misc
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MiscData : ItemList
{

}
[CreateAssetMenu(fileName = "Miscs", menuName = "Items/Miscs", order = 0)]
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
}

#endregion