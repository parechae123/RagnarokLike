using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
[System.Serializable]
public class SkillTreeUI : MonoBehaviour
{
    public SkillTreeBase targetSkillTreeBase;
    public RectTransform rectTR;
    public SkillInfoInGame[] skillInfos = new SkillInfoInGame[0];
    
    [SerializeField]public Button[,] skillBtns;
//    public HashSet<(int, int)> isSkillSlotFilled = new HashSet<(int, int)>();
    // Start is called before the first frame update
    void Awake()
    {
        skillInfos = new SkillInfoInGame[0];
        ResetSkillTree(true);
        for (int i = 0; i < skillInfos.Length; i++)
        {
            SkillManager.GetInstance().AddSkillInfo(skillInfos[i]);
        }
    }
    public void GetSkillBTN()
    {
        skillInfos = new SkillInfoInGame[0];
        skillBtns = new Button[targetSkillTreeBase.SkilltreeResolution.x / 100, targetSkillTreeBase.SkilltreeResolution.y / 100];
        for (int i = 0; i < skillBtns.GetLength(1); i++)
        {
            for (int j = 0; j < skillBtns.GetLength(0); j++)
            {
                skillBtns[j, i] = transform.GetChild((i * skillBtns.GetLength(0)) + j).GetChild(0).GetComponent<Button>();
                skillBtns[j, i].interactable = false;
                skillBtns[j, i].image.sprite = null;
                skillBtns[j, i].transform.parent.GetChild(1).GetComponent<Text>().text = string.Empty;
                skillBtns[j, i].transform.parent.GetChild(2).GetComponent<Text>().text = string.Empty;
                skillBtns[j, i].gameObject.TryGetComponent<QuickSlot>(out QuickSlot tempQuickSlot);
                if (tempQuickSlot != null) Destroy(tempQuickSlot);
            }
        }
    }
    public void ResetSkillTree(bool callOnAwake)
    {
        for (int i = 0; i < targetSkillTreeBase.skillIconsInSkilltree.Length; i++)
        {
            (int, int) tempArray;
            tempArray.Item1 = targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.x / 100;
            tempArray.Item2 = targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.y / 100;
            skillBtns[tempArray.Item1, tempArray.Item2].interactable = true;

            //targetInspector.isSkillSlotFilled.Add(tempArray);
            skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(1).GetComponent<Text>().color = Color.black;  //스킬 레벨 세팅
            skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(1).GetComponent<Text>().text =
                targetSkillTreeBase.skillIconsInSkilltree[i].thisSkill.nowSkillLevel.ToString();
            skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(2).GetComponent<Text>().color = Color.black;  //스킬 이름 세팅
            skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(2).GetComponent<Text>().text =
                targetSkillTreeBase.skillIconsInSkilltree[i].thisSkill.skillName;
            skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.GetChild(2).GetComponent<Text>().resizeTextForBestFit = true;
            skillBtns[tempArray.Item1, tempArray.Item2].image.sprite =
                targetSkillTreeBase.skillIconsInSkilltree[i].thisSkill.skillIcon;
            if (callOnAwake)
            {
                SkillInfoInGame tempInGameSkill = new SkillInfoInGame(targetSkillTreeBase.skillIconsInSkilltree[i].thisSkillInScriptableOBJ);
                Array.Resize<SkillInfoInGame>(ref skillInfos, skillInfos.Length + 1);
                skillInfos[skillInfos.Length - 1] = tempInGameSkill;
                skillBtns[tempArray.Item1, tempArray.Item2].transform.parent.gameObject.AddComponent<QuickSlot>().Install(tempInGameSkill, true);
            }
            else
            {
                EditorUtility.SetDirty(this);
            }
        }
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
            targetInspector.GetSkillBTN();
            targetInspector.ResetSkillTree(false);

        }
    }


    private void GetSkills()
    {

    }
}
