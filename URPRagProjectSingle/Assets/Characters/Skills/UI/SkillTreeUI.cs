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
public class SkillTreeUI : MonoBehaviour , IuiInterface
{

    public SkillTreeBase targetSkillTreeBase;
    public RectTransform rectTR;
    public SkillInfoInGame[] skillInfos = new SkillInfoInGame[0];
    public Material greyScaled, colored,blueScaled;
    public Text leftSkillPointText;
    [SerializeField]public SkillTreeSlot[,] skillBtns;
    public void RegistGameOBJ()
    {
        foreach (KeyCode item in KeyMapManager.GetInstance().keyMaps.Keys)
        {
            if (KeyMapManager.GetInstance().keyMaps[item].UIType == UITypes.SkillTreeWindow)
            {
                ShortCutOBJ temp = KeyMapManager.GetInstance().keyMaps[item];


                temp.subScribFuncs = null;
                temp.subScribFuncs += OnOff;
                KeyMapManager.GetInstance().keyMaps[item] = temp;
                gameObject.transform.parent.gameObject.SetActive(false);
                break;
            }
            else
            {
                continue;
            }
        }
    }
    //    public HashSet<(int, int)> isSkillSlotFilled = new HashSet<(int, int)>();
    // Start is called before the first frame update
    void Awake()
    {
        //skillInfos = new SkillInfoInGame[0];
        GetSkillBTN();
        ResetSkillTree(true);
        for (int i = 0; i < skillInfos.Length; i++)
        {
            SkillManager.GetInstance().AddSkillInfo(skillInfos[i]);
        }
        RegistGameOBJ();
    }
    private void Start()
    {
        CheckSkillStatus();
        Player.Instance.playerLevelInfo.jobLevelUP += CheckSkillStatus;
        Player.Instance.playerLevelInfo.jobLevelUP += UpdateSkillPointText;
    }
    public void OnOff() { gameObject.transform.parent.gameObject.SetActive(!gameObject.transform.parent.gameObject.activeSelf); }
    public void GetSkillBTN()
    {
        skillInfos = new SkillInfoInGame[0];
        skillBtns = new SkillTreeSlot[targetSkillTreeBase.SkilltreeResolution.x / 100, targetSkillTreeBase.SkilltreeResolution.y / 100];
        for (int i = 0; i < skillBtns.GetLength(1); i++)
        {
            for (int j = 0; j < skillBtns.GetLength(0); j++)
            {
                skillBtns[j, i].skillBTN = transform.GetChild((i * skillBtns.GetLength(0)) + j).GetChild(0).GetChild(0).GetComponent<Button>();
                skillBtns[j, i].skillBTN.interactable = false;
                skillBtns[j, i].skillBTN.image.sprite = null;
                skillBtns[j, i].skillBTN.gameObject.TryGetComponent<QuickSlot>(out QuickSlot tempQuickSlot);
                skillBtns[j, i].skillBTN.transform.parent.parent.GetChild(1).GetComponent<Text>().text = string.Empty;
                skillBtns[j, i].skillBTN.transform.parent.parent.GetChild(2).GetComponent<Text>().text = string.Empty;

#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
                if (tempQuickSlot != null) Destroy(tempQuickSlot);
            }
        }

    }
    
    public void CheckSkillStatus()
    {
        //해당함수 JobLevelUp함수에 이벤트로 등록해야함
        for (int i = 0; i < targetSkillTreeBase.skillIconsInSkilltree.Length;i++)
        {
            (int,int) btnArray = (targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.x/100, targetSkillTreeBase.skillIconsInSkilltree[i].positionOnSkillTree.y /100);
            if (skillInfos[i].isSkillLearned)
            {
                if (skillInfos[i].maxSkillLevel <= skillInfos[i].nowSkillLevel || Player.Instance.playerLevelInfo.LeftSkillPoint <= 0)
                {
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.image.raycastTarget = false;
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.targetGraphic.material = colored;
                }
                else if (skillInfos[i].maxSkillLevel > skillInfos[i].nowSkillLevel && Player.Instance.playerLevelInfo.LeftSkillPoint > 0)
                {
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.image.raycastTarget = true;
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.targetGraphic.material = blueScaled;
                }
                continue;
            }
            bool[] tempBool = Player.Instance.playerLevelInfo.isLearnAble(i, targetSkillTreeBase.GetJobPhase,targetSkillTreeBase.skillIconsInSkilltree);

            bool isEscape = false;
            for (int j = 0; j < tempBool.Length; j++ )
            {
                if (!tempBool[j]) 
                { 
                    isEscape = true;
                    skillInfos[i].skillStatus = SkillStatus.noneLearnAble;
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.image.raycastTarget = false;
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.targetGraphic.material = greyScaled;
                    break; 
                }
            }

            if (isEscape) continue;
            else
            {
                if (Player.Instance.playerLevelInfo.LeftSkillPoint > 0)
                {
                    skillInfos[i].skillStatus = SkillStatus.learnAble;
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.image.raycastTarget = true;
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.targetGraphic.material = blueScaled;
                }
                else
                {
                    skillInfos[i].skillStatus = SkillStatus.noneLearnAble;
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.image.raycastTarget = false;
                    skillBtns[btnArray.Item1, btnArray.Item2].skillBTN.targetGraphic.material = greyScaled;

                }
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
            skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.interactable = true;

            //targetInspector.isSkillSlotFilled.Add(tempArray);
            skillBtns[tempArray.Item1, tempArray.Item2].levelText = skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.transform.parent.parent.GetChild(1).GetComponent<Text>();  //스킬 레벨 세팅
            skillBtns[tempArray.Item1, tempArray.Item2].levelText.color = Color.black;  //스킬 레벨 세팅
            skillBtns[tempArray.Item1, tempArray.Item2].levelText.text = string.Empty;
            skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.transform.parent.parent.GetChild(2).GetComponent<Text>().color = Color.black;  //스킬 이름 세팅
            skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.transform.parent.parent.GetChild(2).GetComponent<Text>().text =
                targetSkillTreeBase.skillIconsInSkilltree[i].thisSkill.skillName;

            skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.transform.parent.parent.GetChild(2).GetComponent<Text>().resizeTextForBestFit = true;
            skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.image.sprite =
                targetSkillTreeBase.skillIconsInSkilltree[i].thisSkill.skillIcon;
            if (callOnAwake)
            {
                SkillInfoInGame tempInGameSkill;
                if (targetSkillTreeBase.skillIconsInSkilltree[i].thisSkillInScriptableOBJ.skillType == SkillType.buff)
                {
                    //TODO : skillInfoInGame 말고 buffSkillInfoIngame객체를 새로 만들어야할듯함
                    tempInGameSkill = new BuffSkillInfoInGame(targetSkillTreeBase.skillIconsInSkilltree[i].thisSkillInScriptableOBJ);
                }
                else
                {
                    tempInGameSkill = new SkillInfoInGame(targetSkillTreeBase.skillIconsInSkilltree[i].thisSkillInScriptableOBJ);
                }
                Array.Resize<SkillInfoInGame>(ref skillInfos, skillInfos.Length + 1);
                skillInfos[skillInfos.Length - 1] = tempInGameSkill;
                skillBtns[tempArray.Item1, tempArray.Item2].levelText.text = string.Empty;
                skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.transform.parent.parent.gameObject.AddComponent<QuickSlot>().Install(tempInGameSkill, true);
                int currIndex = i;
                skillBtns[tempArray.Item1, tempArray.Item2].castingLevelDownBTN = skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.transform.parent.parent.GetChild(3).GetComponent<Button>();
                skillBtns[tempArray.Item1, tempArray.Item2].castingLevelUpBTN = skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.transform.parent.parent.GetChild(4).GetComponent<Button>();

                skillBtns[tempArray.Item1, tempArray.Item2].castingLevelDownBTN.onClick.AddListener(() =>
                {
                    if(tempInGameSkill.nowSkillLevel != 0)
                    {
                        tempInGameSkill.CastingSkillLevel -= 0;
                        skillBtns[tempArray.Item1, tempArray.Item2].LevelTextUpdate(((tempInGameSkill.CastingSkillLevel + 1) + "/" + tempInGameSkill.nowSkillLevel).ToString());
                    }

                });
                skillBtns[tempArray.Item1, tempArray.Item2].castingLevelUpBTN.onClick.AddListener(() =>
                {
                    if (tempInGameSkill.nowSkillLevel != 0)
                    {
                        tempInGameSkill.CastingSkillLevel+= 2;
                        skillBtns[tempArray.Item1, tempArray.Item2].LevelTextUpdate(((tempInGameSkill.CastingSkillLevel + 1) + "/" + tempInGameSkill.nowSkillLevel).ToString());
                    }
                });

                //skillBtns[tempArray.Item1, tempArray.Item2].castingLevelDownBTN.gameObject.SetActive(false);
                //skillBtns[tempArray.Item1, tempArray.Item2].castingLevelUpBTN.gameObject.SetActive(false);

                skillBtns[tempArray.Item1, tempArray.Item2].skillBTN.onClick.AddListener(() =>
                {
                    if(Player.Instance.playerLevelInfo.LearnSkill(targetSkillTreeBase.skillIconsInSkilltree, currIndex, tempInGameSkill, targetSkillTreeBase.GetPhase)) 
                    {
                        tempInGameSkill.CastingSkillLevel = 1;
                        skillBtns[tempArray.Item1, tempArray.Item2].LevelTextUpdate(((tempInGameSkill.CastingSkillLevel + 1) + "/" + tempInGameSkill.nowSkillLevel).ToString());

                        CheckSkillStatus();
                        UpdateSkillPointText();
                    }
                });

            }
            else
            {
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }
    }
    public void UpdateSkillPointText()
    {
        leftSkillPointText.text = "남은 스킬 포인트 : "+ Player.Instance.playerLevelInfo.LeftSkillPoint.ToString();
    }
}
#if UNITY_EDITOR
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
}
#endif
public struct SkillTreeSlot
{
    public Button skillBTN;
    public Button castingLevelUpBTN;
    public Button castingLevelDownBTN;
    public Text levelText;
    public void LevelTextUpdate(string text)
    {
        levelText.text = text;
        levelText.gameObject.SetActive(true);
        castingLevelUpBTN.gameObject.SetActive(true);
        castingLevelDownBTN.gameObject.SetActive(true);
    }
}