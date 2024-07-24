using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class SkillTreeEditorTool : EditorWindow
{
    Rect workSpaceArea = new Rect(10, 100, 600, 600);
    public SkillTreeBase targetSkillTree;
    public string newFileName;
    public string skillTreeSavePath                         ;//프로젝트 파일 경로 기준으로
    public (SkillGetConditionTable,int) selectedSkillInfo;
    [MenuItem(@"Tools/Skills/Skill Tree Maker")]
    private static void ShowWindow()
    {
        var window = GetWindow<SkillTreeEditorTool>("SkillTreeEditor");
        window.titleContent = new GUIContent("SkillTree Maker");
        window.Show();
    }

    private void OnGUI()
    {
        GUI.Box(workSpaceArea,"SkillTree WorkSpace");
        targetSkillTree = (SkillTreeBase)EditorGUILayout.ObjectField("작업대상", targetSkillTree, typeof(SkillTreeBase),false);
        skillTreeSavePath = EditorGUILayout.TextField("스킬트리 저장 위치",skillTreeSavePath);
        
        if (targetSkillTree == null)
        {
            newFileName = EditorGUILayout.TextField("새 파일 이름 : ", newFileName);
            CreateSkillTreeBTN(newFileName);
        }
        else
        {
            if (selectedSkillInfo.Item1 != null)
            {
                ShowSelectedSkill();
            }
            for (int i = 0; i < targetSkillTree.skillIconsInSkilltree.Length; i++)
            {
                Rect iconRect = new Rect(workSpaceArea);
                iconRect.height = 100;
                iconRect.width = 100;
                iconRect.x += targetSkillTree.skillIconsInSkilltree[i].positionOnSkillTree.x;
                iconRect.y += targetSkillTree.skillIconsInSkilltree[i].positionOnSkillTree.y;

                if (iconRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        selectedSkillInfo.Item1.positionOnSkillTree += Event.current.delta;
                        Repaint();
                    }
                    else if(Event.current.type == EventType.MouseUp)
                    {
                        Repaint();
                    }
                    else if(Event.current.type == EventType.MouseDown)
                    {
                        selectedSkillInfo.Item1 = targetSkillTree.skillIconsInSkilltree[i];
                        selectedSkillInfo.Item2 = i;

                    }
                }
                GUI.Box(iconRect, targetSkillTree.skillIconsInSkilltree[i].thisSkill.skillIcon.texture);
            }
        }
        HandleDragAndDrop(workSpaceArea);
    }
    public void CreateSkillTree()
    {
        
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

    private void HandleDragAndDrop(Rect dropArea)
    {
        // 현재 이벤트를 가져옴.
        Event evt = Event.current;

        // 현재 이벤트의 타입에 따라 처리합니다.
        switch (evt.type)
        {
            // 드래그가 업데이트될 때와 드래그가 완료될 때 처리합니다.
            case EventType.DragUpdated:
            case EventType.DragPerform:
                // 드래그 앤 드롭 영역에 마우스가 있는지 확인합니다.
                if (!dropArea.Contains(evt.mousePosition))
                    return; // 마우스가 영역에 없으면 처리를 중단합니다.

                // 마우스 커서 모드 변경
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                // 드래그가 완료시
                if (evt.type == EventType.DragPerform)
                {
                    // 드래그된 항목을 수락합니다.
                    DragAndDrop.AcceptDrag();
                    // 드래그된 오브젝트 목록을 반복합니다.
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        if (DragAndDrop.objectReferences[i] is SkillInfo)
                        {
                            if (!targetSkillTree)
                            {
                                Debug.LogError("스킬트리를 먼저 할당 해 주십시오.");
                                return;
                            }
                            SkillInfo tempSkillInfo = (SkillInfo)DragAndDrop.objectReferences[i];
                            if (IsSkillTreeEmptyValueThere(tempSkillInfo))
                            {
                                Debug.Log("빈 공간에 추가");
                            }
                            else
                            {
                                targetSkillTree.skillIconsInSkilltree.AddArraySkills(tempSkillInfo);

                            }
                        }
                        else if (DragAndDrop.objectReferences[i] is SkillTreeBase)
                        {
                            targetSkillTree = (SkillTreeBase)DragAndDrop.objectReferences[i];
                        }
                        else
                        {
                            Debug.LogError("파일 타입 오류,파일 이름 : " + DragAndDrop.objectReferences[i].name);
                        }

                    }
                }
                break;
        }
    }
    private void CreateSkillTreeBTN(string fileName)
    {
        if (GUILayout.Button("새로운 스킬트리 생성"))
        {
            if (skillTreeSavePath == string.Empty || fileName == string.Empty) 
            {
                Debug.LogError("경로 혹은 파일명이 공란입니다.");
                return;
            } 
            string tempPath = skillTreeSavePath;
            string[] checkSameName = AssetDatabase.FindAssets(fileName, new[] { tempPath });
            string[] assetPaths = checkSameName.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
            for (int i = 0; i < assetPaths.Length; i++)
            {
                if (assetPaths[i].Contains(fileName))
                {
                    Debug.LogError("NewSkillTreeFile 이름을 가진 SkillTree파일이 이미 존재합니다.");
                    return;
                }

            }
            targetSkillTree = ScriptableObject.CreateInstance<SkillTreeBase>();
            tempPath = Path.Combine(skillTreeSavePath, $"{fileName + ".asset"}");
            AssetDatabase.CreateAsset(targetSkillTree, tempPath);
            AssetDatabase.SaveAssets();
        }

    }
    private void ShowSelectedSkill()
    {
        EditorGUI.BeginDisabledGroup(true);
        selectedSkillInfo.Item1.thisSkillInScriptableOBJ = (SkillInfo)EditorGUILayout.ObjectField("선택된 오브젝트",selectedSkillInfo.Item1.thisSkillInScriptableOBJ,typeof(SkillInfo),false);
        selectedSkillInfo.Item2 = EditorGUILayout.IntField(selectedSkillInfo.Item2);

        EditorGUI.EndDisabledGroup();
    }
    private bool IsSkillTreeEmptyValueThere(SkillInfo skillInfo)
    {
        for (int i = 0; i < targetSkillTree.skillIconsInSkilltree.Length; i++)
        {
            if (targetSkillTree.skillIconsInSkilltree[i].thisSkill == null|| targetSkillTree.skillIconsInSkilltree[i].thisSkillInScriptableOBJ == null)
            {
                targetSkillTree.skillIconsInSkilltree[i] = new SkillGetConditionTable(skillInfo);
                return true;
            }
        }
        return false;
    }
}
