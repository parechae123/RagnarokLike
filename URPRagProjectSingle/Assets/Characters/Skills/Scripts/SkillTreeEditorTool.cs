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
    Rect workSpaceArea = new Rect(10, 100, 800, 600);
    public SkillTreeBase targetSkillTree;
    public string newFileName;
    public string skillTreeSavePath;//������Ʈ ���� ��� ��������
    public (SkillGetConditionTable, int) selectedSkillNode;
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
        targetSkillTree = (SkillTreeBase)EditorGUILayout.ObjectField("�۾����", targetSkillTree, typeof(SkillTreeBase), false);
        skillTreeSavePath = EditorGUILayout.TextField("��ųƮ�� ���� ��ġ", skillTreeSavePath);

        if (targetSkillTree == null)
        {
            newFileName = EditorGUILayout.TextField("�� ���� �̸� : ", newFileName);
            CreateSkillTreeBTN(newFileName);
        }
        else
        {
            if (selectedSkillNode.Item1 != null)
            {
                ShowSelectedSkill();
            }
            if (makingConnection)conditionLevel = EditorGUILayout.IntField("���ེų ����",conditionLevel);
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
                        //�ļӽ�ų ��ġ
                        Vector2 additiveValue = workSpaceArea.position +(iconRect.size / 2f);
                        Vector2 startPos = targetSkillTree.skillIconsInSkilltree[i].positionOnSkillTree+ additiveValue;
                        //���ེų ��ġ
                        Vector2 endPos = targetSkillTree.skillIconsInSkilltree[targetSkillTree.skillIconsInSkilltree[i].skillGetConditions[J].targetIndex].positionOnSkillTree + additiveValue;
                        Vector2 middlePoint = (startPos + endPos) / 2f;
                        Vector2 direction = (endPos - startPos).normalized;
                        // ȭ������ ������ ���� ���
                        Vector2 rightHead = new Vector2(
                            direction.x * Mathf.Cos(20*Mathf.Rad2Deg) - direction.y * Mathf.Sin(20f * Mathf.Rad2Deg),
                            direction.x * Mathf.Sin(20f * Mathf.Rad2Deg) + direction.y * Mathf.Cos(20f * Mathf.Rad2Deg)
                        ) * 40;
                        // ȭ������ ���� ���� ���
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
                                selectedSkillNode.Item1 = targetSkillTree.skillIconsInSkilltree[i];
                                selectedSkillNode.Item2 = i;
                                rightClickMenu.ShowAsContext();
                                
                            }

                        }

                        if (makingConnection)
                        {
                            SkillNodeConnecting(selectedSkillNode,i,(byte)conditionLevel);
                        }
                        selectedSkillNode.Item1 = targetSkillTree.skillIconsInSkilltree[i];
                        selectedSkillNode.Item2 = i;
                    }
                    else if (Event.current.type == EventType.MouseDrag)
                    {
                        if (Event.current.button == 0)
                        {
                            selectedSkillNode.Item1.positionOnSkillTree += new Vector2Int((int)Event.current.delta.x,(int)Event.current.delta.y);
                            Repaint(); 
                        }
                    }
                    else if (Event.current.type == EventType.MouseUp)
                    {
                        selectedSkillNode.Item1.positionOnSkillTree = new Vector2Int((int)(selectedSkillNode.Item1.positionOnSkillTree.x / 100f)*100, (int)(selectedSkillNode.Item1.positionOnSkillTree.y / 100f) * 100);
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
        else if (userData.ToString() == "Remove")
        {
            RemoveSkillNode();
        }
    }
    private void HandleDragAndDrop(Rect dropArea)
    {
        // ���� �̺�Ʈ�� ������.
        Event evt = Event.current;

        // ���� �̺�Ʈ�� Ÿ�Կ� ���� ó���մϴ�.
        switch (evt.type)
        {
            // �巡�װ� ������Ʈ�� ���� �巡�װ� �Ϸ�� �� ó���մϴ�.
            case EventType.DragUpdated:
            case EventType.DragPerform:
                // �巡�� �� ��� ������ ���콺�� �ִ��� Ȯ���մϴ�.
                if (!dropArea.Contains(evt.mousePosition))
                    return; // ���콺�� ������ ������ ó���� �ߴ��մϴ�.

                // ���콺 Ŀ�� ��� ����
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                // �巡�װ� �Ϸ��
                if (evt.type == EventType.DragPerform)
                {
                    // �巡�׵� �׸��� �����մϴ�.
                    DragAndDrop.AcceptDrag();
                    // �巡�׵� ������Ʈ ����� �ݺ��մϴ�.
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        if (DragAndDrop.objectReferences[i] is SkillInfo)
                        {
                            if (!targetSkillTree)
                            {
                                Debug.LogError("��ųƮ���� ���� �Ҵ� �� �ֽʽÿ�.");
                                return;
                            }
                            SkillInfo tempSkillInfo = (SkillInfo)DragAndDrop.objectReferences[i];
                            if (IsSkillTreeEmptyValueThere(tempSkillInfo))
                            {
                                Debug.Log("�� ������ �߰�");
                            }
                            else
                            {
                                targetSkillTree.skillIconsInSkilltree.AddArraySkills(tempSkillInfo);

                            }
                        }
                        else if (DragAndDrop.objectReferences[i] is SkillTreeBase)
                        {
                            targetSkillTree = (SkillTreeBase)DragAndDrop.objectReferences[i];
                            targetSkillTree.SkilltreeResolution = new Vector2Int((int)workSpaceArea.size.x, (int)workSpaceArea.size.y);
                        }
                        else
                        {
                            Debug.LogError("���� Ÿ�� ����,���� �̸� : " + DragAndDrop.objectReferences[i].name);
                        }

                    }
                }
                break;
        }
    }
    private void CreateSkillTreeBTN(string fileName)
    {
        if (GUILayout.Button("���ο� ��ųƮ�� ����"))
        {
            if (skillTreeSavePath == string.Empty || fileName == string.Empty)
            {
                Debug.LogError("��� Ȥ�� ���ϸ��� �����Դϴ�.");
                return;
            }
            string tempPath = skillTreeSavePath;
            string[] checkSameName = AssetDatabase.FindAssets(fileName, new[] { tempPath });
            string[] assetPaths = checkSameName.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
            for (int i = 0; i < assetPaths.Length; i++)
            {
                if (assetPaths[i].Contains(fileName))
                {
                    Debug.LogError("NewSkillTreeFile �̸��� ���� SkillTree������ �̹� �����մϴ�.");
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
        selectedSkillNode.Item1.thisSkillInScriptableOBJ = (SkillInfo)EditorGUILayout.ObjectField("���õ� ������Ʈ", selectedSkillNode.Item1.thisSkillInScriptableOBJ, typeof(SkillInfo), false);
        selectedSkillNode.Item2 = EditorGUILayout.IntField(selectedSkillNode.Item2);

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
    private void SkillNodeConnecting((SkillGetConditionTable,int) startObject, int targetIndex,byte targetLevel)
    {
        if (targetIndex != startObject.Item2)
        {
            startObject.Item1.AddCondition(targetIndex, targetSkillTree.skillIconsInSkilltree[targetIndex].thisSkill.maxSkillLevel <= targetLevel?
                targetSkillTree.skillIconsInSkilltree[targetIndex].thisSkill.maxSkillLevel :targetLevel);
        }
        makingConnection = false;
    }
    private void RemoveSkillNode()
    {
        targetSkillTree.skillIconsInSkilltree[selectedSkillNode.Item2] = null;
        int filledArrayCount = targetSkillTree.skillIconsInSkilltree.Length -1;
        for (int i = 0; i< targetSkillTree.skillIconsInSkilltree.Length; i++)
        {
            //��ų �迭�� �մ��
            if (targetSkillTree.skillIconsInSkilltree[i] == null)
            {
                for (int j = i; j < targetSkillTree.skillIconsInSkilltree.Length; j++)
                {
                    if (j + 1 >= targetSkillTree.skillIconsInSkilltree.Length) break;
                    targetSkillTree.skillIconsInSkilltree[j] = targetSkillTree.skillIconsInSkilltree[j + 1];
                }
            }
        }
        //��ų �迭 ũ�� ����
        Array.Resize(ref targetSkillTree.skillIconsInSkilltree.skills, filledArrayCount);
        for (int i = 0; i < targetSkillTree.skillIconsInSkilltree.Length; i++)
        {
            for (int J = 0; J < targetSkillTree.skillIconsInSkilltree[i].skillGetConditions.Length; J++)
            {
                if (targetSkillTree.skillIconsInSkilltree[i].skillGetConditions[J].targetIndex == selectedSkillNode.Item2 ||
                    targetSkillTree.skillIconsInSkilltree[i].skillGetConditions[J].targetIndex > targetSkillTree.skillIconsInSkilltree.Length)
                {
                    targetSkillTree.skillIconsInSkilltree[i].skillGetConditions = new SkillGetCondition[0];
                }

            }
        }
    }
}