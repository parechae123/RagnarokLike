#region Misc
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MiscData : ItemList
{

}
[CreateAssetMenu(fileName = "Miscs", menuName = "Items/Miscs", order = 0)]
public class MiscDatas : ScriptableObject
{
    [SerializeField] public MiscData[] items;
    public DefaultAsset sheet;
    public void GetSheetValue()
    {
        items = JsonConvert.DeserializeObject<MiscData[]>(new TableConvert().Json(sheet));
        EditorUtility.SetDirty(this);

        // ������Ʈ�� ����
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

[CustomEditor(typeof(MiscDatas))]
public class MiscSCOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MiscDatas temp = (MiscDatas)target;
        if (GUILayout.Button("�Ҹ�ǰ �Ľ�"))
        {
            temp.GetSheetValue();
        }
    }
}
#endregion