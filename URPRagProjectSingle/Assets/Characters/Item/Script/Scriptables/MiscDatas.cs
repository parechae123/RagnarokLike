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

        // 프로젝트에 저장
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

#endregion