using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SkillTreeUI : MonoBehaviour
{
    public SkillTreeBase targetSkillTreeBase;
    public RectTransform rectTR;
    public Button[,] skillBtns = new Button[0,0];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[CustomEditor(typeof(SkillTreeUI))]
public class SkillTreeUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SkillTreeUI targetInspector = (SkillTreeUI)target;
        if (GUILayout.Button("ReadSkillTree"))
        {
            targetInspector.skillBtns = new Button[targetInspector.targetSkillTreeBase.SkilltreeResolution.x/100, targetInspector.targetSkillTreeBase.SkilltreeResolution.y / 100];
            for (int i = 0; i < targetInspector.skillBtns.GetLength(1); i++)
            {
                for (int j = 0; j < targetInspector.skillBtns.GetLength(0); j++)
                {
                    targetInspector.skillBtns[j, i] = targetInspector.transform.GetChild((i * targetInspector.skillBtns.GetLength(0)) +j).GetComponent<Button>();
                    targetInspector.skillBtns[j, i].interactable = false;
                }
            }
            for (int i = 0; i < targetInspector.targetSkillTreeBase.skillIconsInSkilltree.Length; i++)
            {
                targetInspector.skillBtns[targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.x/100,
                    targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.y/100].interactable = true;

                targetInspector.skillBtns[targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.x / 100,
                    targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.y / 100].image.sprite = 
                    targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].thisSkill.skillIcon;
            }
        }
    }

    private void GetSkills()
    {

    }
}
