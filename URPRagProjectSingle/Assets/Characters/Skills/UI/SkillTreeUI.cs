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
//    public HashSet<(int, int)> isSkillSlotFilled = new HashSet<(int, int)>();
    // Start is called before the first frame update
    private void UpdateSkillUI()
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
                    targetInspector.skillBtns[j, i] = targetInspector.transform.GetChild((i * targetInspector.skillBtns.GetLength(0)) +j).GetChild(0).GetComponent<Button>();
                    targetInspector.skillBtns[j, i].interactable = false;
                    targetInspector.skillBtns[j, i].image.sprite = null;
                    targetInspector.skillBtns[j, i].transform.parent.GetChild(1).GetComponent<Text>().text = string.Empty;
                    targetInspector.skillBtns[j, i].transform.parent.GetChild(2).GetComponent<Text>().text = string.Empty;
                }
            }
            for (int i = 0; i < targetInspector.targetSkillTreeBase.skillIconsInSkilltree.Length; i++)
            {
                (int, int) tempArray;
                tempArray.Item1 = targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.x / 100;
                tempArray.Item2 = targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.y / 100;
                targetInspector.skillBtns[tempArray.Item1,tempArray.Item2].interactable = true;

                //targetInspector.isSkillSlotFilled.Add(tempArray);
                targetInspector.skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(1).GetComponent<Text>().color = Color.black;  //스킬 레벨 세팅
                targetInspector.skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(1).GetComponent<Text>().text =
                    targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].thisSkill.nowSkillLevel.ToString();
                targetInspector.skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(2).GetComponent<Text>().color = Color.black;  //스킬 이름 세팅
                targetInspector.skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(2).GetComponent<Text>().text =
                    targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].thisSkill.skillName;
                targetInspector.skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(2).GetComponent<Text>().resizeTextForBestFit = true;
                targetInspector.skillBtns[tempArray.Item1, tempArray.Item2].image.sprite = 
                    targetInspector.targetSkillTreeBase.skillIconsInSkilltree[i].thisSkill.skillIcon;
            }
        }
    }

    private void GetSkills()
    {

    }
}
