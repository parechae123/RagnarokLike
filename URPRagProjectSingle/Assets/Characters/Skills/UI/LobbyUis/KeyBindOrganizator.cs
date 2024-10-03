using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class KeyBindOrganizator : MonoBehaviour
{
    // Start is called before the first frame update
    public KeyBinnder[] binders = new KeyBinnder[0];
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[CustomEditor(typeof(KeyBindOrganizator))]
public class BindOrganizator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        KeyBindOrganizator KBO = (KeyBindOrganizator)target;

        if (GUILayout.Button("Generate ScriptableObject"))
        {
            KBO.binders = new KeyBinnder[KBO.transform.childCount];
            for (int i = 0; i < KBO.transform.childCount; i++)
            {
                KBO.binders[i] = KBO.transform.GetChild(i).GetOrAddComponent<KeyBinnder>();
                KBO.binders[i].AutoSetting();
            }
        }
    }
}