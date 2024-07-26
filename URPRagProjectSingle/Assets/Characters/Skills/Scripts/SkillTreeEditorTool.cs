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
    public string skillTreeSavePath;//프로젝트 파일 경로 기준으로
    public (SkillGetConditionTable, int) selectedSkillInfo;
    private GenericMenu rightClickMenu;
    private bool makingConnection = false;
    public int conditionLevel;
    [MenuItem(@"Tools/Skills/Skill Tree Maker")]
    private static void ShowWindow()
    {
        var window = GetWindow<SkillTreeEditorTool>("SkillTreeEditor");
        window.titleContent = new GUIContent("SkillTree Maker");
        window.Show();
    }

    private void OnGUI()
    {
        GUI.Box(workSpaceArea, "SkillTree WorkSpace");
        targetSkillTree = (SkillTreeBase)EditorGUILayout.ObjectField("작업대상", targetSkillTree, typeof(SkillTreeBase), false);
        skillTreeSavePath = EditorGUILayout.TextField("스킬트리 저장 위치", skillTreeSavePath);

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
            if (makingConnection)conditionLevel = EditorGUILayout.IntField("선행스킬 레벨",conditionLevel);
            for (int i = 0; i < targetSkillTree.skillIconsInSkilltree.Length; i++)
            {
                Rect iconRect = new Rect(workSpaceArea);
                iconRect.height = 100;
                iconRect.width = 100;
                iconRect.x += targetSkillTree.skillIconsInSkilltree[i].positionOnSkillTree.x;
                iconRect.y += targetSkillTree.skillIconsInSkilltree[i].positionOnSkillTree.y;

                for (int J = 0; J < targetSkillTree.skillIconsInSkilltree[i].skillGetConditions.Length; J++)
                {
                    if (targetSkillTree.skillIconsInSkilltree[i].skillGetConditions[J] != null)
                    {
                        Handles.color = Color.red;
                        //후속스킬 위치
                        Vector2 additiveValue = workSpaceArea.position +(iconRect.size / 2f);
                        Vector2 startPos = targetSkillTree.skillIconsInSkilltree[i].positionOnSkillTree+ additiveValue;
                        //선행스킬 위치
                        Vector2 endPos = targetSkillTree.skillIconsInSkilltree[targetSkillTree.skillIconsInSkilltree[i].skillGetConditions[J].targetIndex].positionOnSkillTree + additiveValue;
                        Vector2 middlePoint = (startPos + endPos) / 2f;
                        Vector2 direction = (endPos - startPos).normalized;
                        // 화살촉의 오른쪽 벡터 계산
                        Vector2 rightHead = new Vector2(
                            direction.x * Mathf.Cos(20*Mathf.Rad2Deg) - direction.y * Mathf.Sin(20f * Mathf.Rad2Deg),
                            direction.x * Mathf.Sin(20f * Mathf.Rad2Deg) + direction.y * Mathf.Cos(20f * Mathf.Rad2Deg)
                        ) * 40;
                        // 화살촉의 왼쪽 벡터 계산
                        Vector2 leftHead = new Vector2(
                            direction.x * Mathf.Cos(-(20*Mathf.Rad2Deg)) - direction.y * Mathf.Sin(-(20f * Mathf.Rad2Deg)),
                            direction.x * Mathf.Sin(-(20f * Mathf.Rad2Deg)) + direction.y * Mathf.Cos(-(20f * Mathf.Rad2Deg))
                        ) * 40;
                        rightHead = rightHead + middlePoint;
                        leftHead = leftHead+ middlePoint;

                        Handles.DrawAAPolyLine(15f, startPos, middlePoint, rightHead, middlePoint, leftHead, middlePoint, endPos);
                    }
                }

                if (iconRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (Event.current.button == 1)
                        {
                            if (makingConnection)
                            {
                                makingConnection = false;
                            }
                            else
                            {
                                selectedSkillInfo.Item1 = targetSkillTree.skillIconsInSkilltree[i];
                                selectedSkillInfo.Item2 = i;
                                rightClickMenu.ShowAsContext();
                                
                            }

                        }

                        if (makingConnection)
                        {
                            SkillConnecting(selectedSkillInfo,i,conditionLevel);
                        }
                        selectedSkillInfo.Item1 = targetSkillTree.skillIconsInSkilltree[i];
                        selectedSkillInfo.Item2 = i;
                    }
                    else if (Event.current.type == EventType.MouseDrag)
                    {
                        if (Event.current.button == 0)
                        {
                            selectedSkillInfo.Item1.positionOnSkillTree += Event.current.delta;
                            Repaint(); 
                        }
                    }
                    else if (Event.current.type == EventType.MouseUp)
                    {
                        Repaint();
                    }
                }

                GUI.Box(iconRect, targetSkillTree.skillIconsInSkilltree[i].thisSkill.skillIcon.texture);
            }
        }
        HandleDragAndDrop(workSpaceArea);
    }
    private void OnEnable()
    {
        EditorApplication.update += Update;
        rightClickMenu = new GenericMenu();
        rightClickMenu.AddItem(new GUIContent("Make Condition"), false,MenuItemCallBack, "Make Condition");
        rightClickMenu.AddItem(new GUIContent("Remove"), false, MenuItemCallBack, "Remove");
        makingConnection = false;
    }
    private void OnDisable()
    {
        EditorApplication.update -= Update;
        rightClickMenu = null;
        makingConnection = false;
    }
    private void Update()
    {
        if (this == null)
        {
            EditorApplication.update -= Update;
            return;
        }
    }
    private void MenuItemCallBack(object userData)
    {
        if (userData.ToString() == "Make Condition")
        {
            makingConnection = true;
        }
        else if(userData.ToString() == "Remove")
        {
            
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
        selectedSkillInfo.Item1.thisSkillInScriptableOBJ = (SkillInfo)EditorGUILayout.ObjectField("선택된 오브젝트", selectedSkillInfo.Item1.thisSkillInScriptableOBJ, typeof(SkillInfo), false);
        selectedSkillInfo.Item2 = EditorGUILayout.IntField(selectedSkillInfo.Item2);

        EditorGUI.EndDisabledGroup();
    }
    private bool IsSkillTreeEmptyValueThere(SkillInfo skillInfo)
    {
        for (int i = 0; i < targetSkillTree.skillIconsInSkilltree.Length; i++)
        {
            if (targetSkillTree.skillIconsInSkilltree[i].thisSkill == null || targetSkillTree.skillIconsInSkilltree[i].thisSkillInScriptableOBJ == null)
            {
                targetSkillTree.skillIconsInSkilltree[i] = new SkillGetConditionTable(skillInfo);
                return true;
            }
        }
        return false;
    }
    private void SkillConnecting((SkillGetConditionTable,int) startObject, int targetIndex,int targetLevel)
    {
        if (targetIndex != startObject.Item2)
        {
            startObject.Item1.AddCondition(targetIndex, targetSkillTree.skillIconsInSkilltree[targetIndex].thisSkill.maxSkillLevel <= targetLevel?
                targetSkillTree.skillIconsInSkilltree[targetIndex].thisSkill.maxSkillLevel :targetLevel);
        }
        makingConnection = false;
    }
}
