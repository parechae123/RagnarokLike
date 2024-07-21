using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillTreeEditorTool : EditorWindow
{
    [MenuItem(@"Tools/Skill Tree Maker")]
    
    private static void ShowWindow()
    {
        var window = GetWindow<SkillTreeEditorTool>();
        window.titleContent = new GUIContent("Skill Tree Maker");
        window.Show();
    }
    private void OnEnable()
    {
        EditorApplication.update += Update;
    }
    private void OnDisable()
    {
        EditorApplication.update -= Update;
    }
    private void Update()
    {
        if (this == null)
        {
            EditorApplication.update -= Update;
            return;
        }
    }
}
