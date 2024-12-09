using DG.Tweening;
using PlayerDefines.Stat;
using PlayerDefines.States;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NeutralDefines
{
    namespace State
    {

        using PlayerDefines.States;
        using System.Collections;
        using Unity.VisualScripting;
        using UnityEngine;

        [System.Serializable]
        public class PlayerStateMachine
        {

            private PlayerStates[] allStates = new PlayerStates[0];
            public PlayerStates[] AllStates
            {
                get { return allStates; }
            }
            private PlayerStates currentState;
            public PlayerStates CurrentState
            {
                get { return currentState; }
            }
            public PlayerStateMachine(PlayerStates[] defaultStates, Animator anim)
            {
                allStates = defaultStates;
                this.anim = anim;
            }
            [SerializeField] public Animator anim;
            private Dirrections animationDirrection
            {
                get
                {
                    Vector2Int tempVecInt = Player.Instance.PlayerLookDir - PlayerCam.Instance.CameraDirrection;
                    //Debug.Log(tempVecInt);
                    sbyte maxValue = (sbyte)Mathf.Max(tempVecInt.x, tempVecInt.y);
                    sbyte minValue = (sbyte)Mathf.Min(tempVecInt.x, tempVecInt.y);
                    if (maxValue == default(sbyte) && minValue == default(sbyte)) return Dirrections.N;
                    else if (maxValue == (sbyte)2 || minValue == (sbyte)-2) return Dirrections.S;
                    else
                    {
                        if (PlayerCam.Instance.CameraDirrection.x == 0)
                        {
                            if ((maxValue < default(sbyte) && minValue < default(sbyte)) || (maxValue > default(sbyte) && minValue > default(sbyte))) return Dirrections.W;
                            else if ((maxValue > default(sbyte) && minValue < default(sbyte)) || (maxValue < default(sbyte) && minValue < default(sbyte))) return Dirrections.E;
                        }
                        else
                        {
                            if ((maxValue < default(sbyte) && minValue < default(sbyte)) || (maxValue > default(sbyte) && minValue > default(sbyte))) return Dirrections.E;
                            else if ((maxValue > default(sbyte) && minValue < default(sbyte)) || (maxValue < default(sbyte) && minValue < default(sbyte))) return Dirrections.W;
                        }

                    }
                    return Dirrections.E;
                }
            }
            public void SetDirrection(ref Vector2Int targetInstance,Vector3 startPos, Vector3 endPos)
            {
                Vector3 tempPos = startPos - endPos;
                if (tempPos.x != 0)
                {
                    if (tempPos.x > 0)
                    {
                        targetInstance = Vector2Int.right;
                    }
                    else if (tempPos.x < 0)
                    {
                        targetInstance = Vector2Int.left;
                    }
                }
                else
                {
                    if (tempPos.z > 0)
                    {
                        targetInstance = Vector2Int.up;
                    }
                    else if (tempPos.z < 0)
                    {
                        targetInstance = Vector2Int.down;
                    }
                }
            }
            public void SetDirrection(ref Vector2Int targetInstance,Vector2Int startPos, Vector2Int endPos)
            {
                Vector2Int tempPos = startPos - endPos;
                if (tempPos.x != 0)
                {
                    if (tempPos.x > 0)
                    {
                        targetInstance = Vector2Int.right;
                    }
                    else if (tempPos.x < 0)
                    {
                        targetInstance = Vector2Int.left;
                    }
                }
                else
                {
                    if (tempPos.y > 0)
                    {
                        targetInstance = Vector2Int.up;
                    }
                    else if (tempPos.y < 0)
                    {
                        targetInstance = Vector2Int.down;
                    }
                }
            }
            public void ChangeState(string newStateName)
            {
                if (currentState == null)
                {
                    currentState = SearchState("idleState");
                    currentState.Enter();
                    return;
                }
                else
                {
                    if (!CurrentState.isCancelableState&&CurrentState.durationTime>= CurrentState.GetTimer&&!Player.Instance.isMotionBookCancel) return;
                }
                anim.speed = 1;
                currentState?.Exit();                   //이전 상태값을 빠져나간다
                currentState = SearchState(newStateName);               //인수로 받아온 상태값을 입력
                currentState?.Enter();                  //다음 상태값
                if (currentState.durationTime == 0) return;
                AnimationChange();
            }
            public void ChangeState(float castingTime,SkillInfoInGame skillInfo,Stats targetStat,Vector3 skillPos)
            {
                if (currentState == null)
                {
                    currentState = SearchState("idleState");
                    currentState.Enter();
                    return;
                }
                else
                {
                    if (!CurrentState.isCancelableState) return;
                }
                anim.speed = 1;
                CastingState temp = (CastingState)SearchState("castingState");
                if (currentState == temp) return;
                if(targetStat != null&&skillInfo.objectiveType == ObjectiveType.OnlyTarget)
                {
                    skillPos = (targetStat.standingNode.worldPos + Player.Instance.playerLevelInfo.stat.standingNode.worldPos);
                    skillPos.x = skillPos.x / 2f;
                    skillPos.y = skillPos.y / 2f;
                    skillPos.z = skillPos.z / 2f;
                }

                if (Player.Instance.playerLevelInfo.stat.IsEnoughSP(skillInfo.skill[skillInfo.CastingSkillLevel].spCost))
                {
                    Debug.Log("마나 충분, 캐스팅 시작");
                }
                else
                {
                    Debug.Log("마나 불충분, 캐스팅 취소");
                    return;
                }

                //타겟 및 스텟 및 스킬위치 초기화
                temp.casting = null;
                temp.targetStat = null;
                temp.castPos = Vector3.down*100;
                temp.casting += skillInfo.SkillCastTargetPlace;

/*                if (skillInfo.skillType == SkillType.Active)
                {
                    temp.casting += skillInfo.SkillCastTargetPlace;
                }
                else if(skillInfo.skillType == SkillType.buff)
                {
                    temp.casting += skillInfo.SkillCastTargetPlace;
                }*/
                
                temp.castPos = skillPos;
                temp.targetStat = targetStat;
                if(skillInfo.skillPosition != SkillPosition.self) Player.Instance.SetCharactorDirrection(Player.Instance.transform.position, skillPos);
                currentState?.Exit();                   //이전 상태값을 빠져나간다
                currentState = temp;               
                currentState.durationTime = castingTime*Player.Instance.playerLevelInfo.stat.CastTimePercent;
                currentState?.Enter();                  //다음 상태값
                AnimationChange();
            }
            public void ChangeState(PlayerStates state)
            {
                currentState?.Exit();
                currentState = state;
                currentState?.Enter();
                AnimationChange();
            }
            public void AnimationChange()
            {
                currentState?.SetAnimationSpeed(anim);
                anim.Play(currentState.stateName + animationDirrection);
            }
            public void CamRotAnimChange()
            {
                currentState?.SetAnimationSpeed(anim);
                anim.Play(currentState.stateName + animationDirrection, 0, anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }

            public PlayerStates SearchState(string stateName)
            {
                sbyte i = 0;
                for (; i < allStates.Length; i++)
                {
                    if (allStates[i].stateName == stateName)
                    {
                        break;
                    }
                }
                return allStates[i];
            }
        }
        [System.Serializable]
        public class CursorStates
        {
            [SerializeField] private Texture2D defaultCursorIMG;
            [SerializeField] private Texture2D noneClickAbleIMG;
            [SerializeField] private Texture2D itemCursorIMG;
            [SerializeField] private Texture2D attackAbleCursorIMG;
            [SerializeField] private Texture2D skillTargetingCursorIMG;
            public cursorState CurrentCursorState
            {
                get;
                private set;
            }
            public void changeState(cursorState nextCursorState)
            {
                if (nextCursorState == CurrentCursorState) return;
                CurrentCursorState = nextCursorState;
                switch (CurrentCursorState)
                {
                    case cursorState.defaultCurser:
                        Cursor.SetCursor(defaultCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.noneClickAbleState:
                        Cursor.SetCursor(noneClickAbleIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.itemCursor:
                        Cursor.SetCursor(itemCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.attackAble:
                        Cursor.SetCursor(attackAbleCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.skillTargeting:
                        Cursor.SetCursor(skillTargetingCursorIMG, Vector2.down, CursorMode.Auto);
                        break;

                }
            }
        }
        public enum cursorState
        {
            defaultCurser, noneClickAbleState, itemCursor, attackAble, skillTargeting
        }

    }
    
}
public interface ICameraTracker
{
    void RegistCameraAction();
    void UnRegistCameraAction();
    void FollowCamera();
}

public interface ItemInfo
{
    protected virtual Sprite ItemIcon
    {
        get { return default(Sprite); }
    }
    protected virtual int ItemAmount
    {
        get { return default(int); }
    }
    public virtual void UseItem()
    {

    }
}
public enum SkillStatus
{
    learnAble,noneLearnAble,useable
}
[Serializable]
public struct ShortCutOBJ
{
    public UITypes UIType;
    //0이하의 갑을 가지고 있을 시 SlotNumber가 아님
    public int SlotNumber
    {
        get { return ((int)UIType - (int)UITypes.QuickSlotOne ); }
    }
    public bool needCombKey;
    public Action subScribFuncs;
}
public class RespawnBox
{
    public MonsterBase monster;
    public float leftRespawnTime;
    public RespawnBox(MonsterBase monster, float leftRespawnTime)
    {
        this.monster = monster;
        this.leftRespawnTime = leftRespawnTime;
    }

    public void UntillRespawn(float deltaTime)
    {
        leftRespawnTime -= deltaTime;
    }
}
public class BasicStatus
{
    public event Action updateStat;
    int changeAbleStrength = 0;
    int pureStrength = 1;
    public int GetPureStr { get { return pureStrength; } }
    public int Strength
    {
        get { return changeAbleStrength + pureStrength; }
        set { changeAbleStrength = value; }
    }
    
    int changeAbleAgility = 0;
    int pureAgility = 1;
    public int GetPureAgi { get { return pureAgility; } }
    public int Agility
    {
        get { return changeAbleAgility + pureAgility;}
        set { changeAbleAgility = value; }
    }
    
    int changeAbleVitality = 0;
    int pureVitality = 1;
    public int GetPureVit{get { return pureVitality; }}
    public int Vitality
    {
        get { return changeAbleVitality + pureVitality; }
        set { changeAbleVitality = value;}
    }
    
    int changeAbleDexterity = 0;
    int pureDexterity = 1;
    public int GetPureDex{get { return pureDexterity; }}
    public int Dexterity
    {
        get { return changeAbleDexterity + pureDexterity; }
        set { changeAbleDexterity = value; }
    }
    
    int changeAbleInteligence = 0;
    int pureInteligence = 1;
    public int GetPureInt{get { return pureInteligence; }}
    public int Inteligence
    {
        get { return changeAbleInteligence + pureInteligence; }
        set { changeAbleInteligence = value; }
    }
    
    int changeAbleLuck = 0;
    int pureLuck = 1;
    public int GetPureLuk { get{ return pureLuck; } }
    public int Luck
    {
        get { return changeAbleLuck + pureLuck; }
        set { changeAbleLuck = value;}
    }
    /// <summary>
    /// pureStat을 증가시키는 유일한 함수, (추후 UI에 연결해야함)
    /// </summary>
    /// <param name="statType">스텟 종류를 넣자</param>
    /// <param name="statusPoint">PlayerStats의 LeftStatusPoint을 넣어줌</param>
    public void PureStatUP(BasicStatTypes statType)
    {
        switch (statType)
        {
            case BasicStatTypes.Str:
                if(GetRequrePoint(pureStrength)<= Player.Instance.playerLevelInfo.LeftStatusPoint)
                {
                    Player.Instance.playerLevelInfo.usedStatusPoint += GetRequrePoint(pureStrength);
                    pureStrength++;
                }
                break;
            case BasicStatTypes.AGI:
                if (GetRequrePoint(pureAgility) <= Player.Instance.playerLevelInfo.LeftStatusPoint)
                {
                    Player.Instance.playerLevelInfo.usedStatusPoint += GetRequrePoint(pureAgility);
                    pureAgility++;
                }
                break;
            case BasicStatTypes.Vit:
                if (GetRequrePoint(pureVitality) <= Player.Instance.playerLevelInfo.LeftStatusPoint)
                {
                    Player.Instance.playerLevelInfo.usedStatusPoint += GetRequrePoint(pureVitality);
                    pureVitality++;
                }
                break;
            case BasicStatTypes.Int:
                if (GetRequrePoint(pureInteligence) <= Player.Instance.playerLevelInfo.LeftStatusPoint)
                {
                    Player.Instance.playerLevelInfo.usedStatusPoint += GetRequrePoint(pureInteligence);
                    pureInteligence++;
                }
                break;
            case BasicStatTypes.Dex:
                if (GetRequrePoint(pureDexterity) <= Player.Instance.playerLevelInfo.LeftStatusPoint)
                {
                    Player.Instance.playerLevelInfo.usedStatusPoint += GetRequrePoint(pureDexterity);
                    pureDexterity++;
                }
                break;
            case BasicStatTypes.Luk:
                if (GetRequrePoint(pureLuck) <= Player.Instance.playerLevelInfo.LeftStatusPoint)
                {
                    Player.Instance.playerLevelInfo.usedStatusPoint += GetRequrePoint(pureLuck);
                    pureLuck++;
                }
                break;
            default:
                Debug.LogError("지정되지 않은 스텟요소입니다");
                break;
        }
    }
    public short GetRequrePoint(int pureStat)
    {
        return (short)(2 + ((pureStat - 1) / 10));
    }
    public void SetChangeAbleStatus(BasicStatTypes type, int value)
    {
        switch (type)
        {
            case BasicStatTypes.Str:
                changeAbleStrength += value;
                break;
            case BasicStatTypes.AGI:
                changeAbleAgility += value;
                break;
            case BasicStatTypes.Vit:
                changeAbleVitality += value;
                break;
            case BasicStatTypes.Int:
                changeAbleInteligence += value; 
                break;
            case BasicStatTypes.Dex:
                changeAbleDexterity += value;
                break;
            case BasicStatTypes.Luk:
                changeAbleLuck += value;
                break;
            default:
                break;
        }
        updateStat?.Invoke();
    }
}
public class SlotInfo
{

    private RectTransform tr;
    public RectTransform TR 
    { 
        get 
        { 
            if (tr == null)
            {
                tr = (RectTransform)UIManager.GetInstance().MainCanvas.Find("SlotInfo").transform;
            }
            return tr;
        }
    }
    
    private Image outLine;
    public Image OutLine
    { 
        get 
        { 
            if (outLine == null)
            {
                outLine = TR.Find("OutLine").GetComponent<Image>();
            }
            return outLine;
        }
    }

    private TextMeshProUGUI nameText;
    public TextMeshProUGUI NameText
    {
        get 
        {
            if (nameText == null)
            {
                nameText = TR.Find("NameText").GetComponent<TextMeshProUGUI>();
            }
            return nameText;
        }
    }

    private TextMeshProUGUI rankText;
    public TextMeshProUGUI RankText
    {
        get
        {
            if (rankText == null)
            {
                rankText = TR.Find("RankText").GetComponent<TextMeshProUGUI>();
            }
            return rankText;
        }
    }

    private TextMeshProUGUI typeText;
    public TextMeshProUGUI TypeText
    {
        get
        {
            if (typeText == null)
            {
                typeText = TR.Find("TypeText").GetComponent<TextMeshProUGUI>();
            }
            return typeText;
        }
    }
   
    private TextMeshProUGUI uniqueText;
    public TextMeshProUGUI UniqueText
    {
        get
        {
            if (uniqueText == null)
            {
                uniqueText = TR.Find("UniqueText").GetComponent<TextMeshProUGUI>();
            }
            return uniqueText;
        }
    }
   
    private TextMeshProUGUI uniqueValueText;
    public TextMeshProUGUI UniqueValueText
    {
        get
        {
            if (uniqueValueText == null)
            {
                uniqueValueText = TR.Find("UniqueValueText").GetComponent<TextMeshProUGUI>();
            }
            return uniqueValueText;
        }
    }
   
    private TextMeshProUGUI apixText;
    public TextMeshProUGUI ApixText
    {
        get
        {
            if (apixText == null)
            {
                apixText = TR.Find("ApixText").GetComponent<TextMeshProUGUI>();
            }
            return apixText;
        }
    }
    
    private TextMeshProUGUI apixValueText;
    public TextMeshProUGUI ApixValueText
    {
        get
        {
            if (apixValueText == null)
            {
                apixValueText = TR.Find("ApixValueText").GetComponent<TextMeshProUGUI>();
            }
            return apixValueText;
        }
    }
    
    private TextMeshProUGUI priceText;
    public TextMeshProUGUI PriceText
    {
        get
        {
            if (priceText == null)
            {
                priceText = TR.Find("PriceText").GetComponent<TextMeshProUGUI>();
            }
            return priceText;
        }
    }
    
    private TextMeshProUGUI flavorText;
    public TextMeshProUGUI FlavorText
    {
        get
        {
            if (flavorText == null)
            {
                flavorText = TR.Find("FlavorText").GetComponent<TextMeshProUGUI>();
            }
            return flavorText;
        }
    }

    public void SetText(Consumables consumables,Vector3 screenPoint)
    {
        TR.position = screenPoint;
        RsetTexts();

        //TODO : 소모품 등급이 상정되지 않음
        (string, Color32) gradeInfo = ResourceManager.GetInstance().NameSheet.GetGradeNameValue(0);
        OutLine.color = gradeInfo.Item2;
        RankText.color = gradeInfo.Item2;
        RankText.text = "일반";
        NameText.text = consumables.itemName;
        switch (consumables.consumType)
        {
            case ConsumType.none:
                break;
            case ConsumType.posion:
                UniqueText.text = consumables.effectInHP ?"즉시 체력 회복":"즉시 마나 회복";
                UniqueValueText.text = consumables.effectValue.ToString();
                break;
            case ConsumType.buffItem:
                break;
            case ConsumType.food:
                break;
            default:
                break;
        }
        PriceText.text = $"{consumables.SellValue}G";
        //FlavorText.text
        TR.gameObject.SetActive(true);
    }
    public void SetText(Equips equips,Vector3 screenPoint)
    {
        TR.position = screenPoint;
        RsetTexts();
        (string, Color32) gradeInfo = ResourceManager.GetInstance().NameSheet.GetGradeNameValue(equips.gradeLevel);

        OutLine.color = gradeInfo.Item2;
        RankText.text = gradeInfo.Item1;
        RankText.color = gradeInfo.Item2;
        NameText.text = equips.itemName;
        if(equips.GetPart == EquipPart.LeftHand|| equips.GetPart == EquipPart.RightHand|| equips.GetPart == EquipPart.TwoHanded)
        {
            Weapons temp = (Weapons)equips;
            UniqueText.text = temp.IsMATKWeapon ? "주문력" : "공격력";
            UniqueValueText.text = temp.ValueOne.ToString("N0");
            UniqueText.text += $"\n{ResourceManager.GetInstance().NameSheet.GetUINameValue(temp.apixList.statLine.Item1.ToString())}";
            UniqueValueText.text += $"\n{temp.apixList.statLine.Item2}";
            TypeText.text = $"무기({(equips.GetPart == EquipPart.TwoHanded ? "양손" : (equips.GetPart == EquipPart.RightHand ? "오른손" : "왼손")) + ResourceManager.GetInstance().NameSheet.GetEquipNameValue(temp.itemCode)})";
            ApixText.text = string.Empty;
            ApixValueText.text = string.Empty;
            for (int i = 0; i < temp.apixList.abilityApixes.Length; i++)
            {
                ApixText.text += ("+" + ResourceManager.GetInstance().NameSheet.GetUINameValue(temp.apixList.abilityApixes[i].Item1.ToString()) + "\n");
                ApixValueText.text += temp.apixList.abilityApixes[i].Item2.ToString("N4") + "\n";
            }
        }
        else
        {
            Armors temp = (Armors)equips;
            UniqueText.text = temp.magicDeff? "마법 저항력" : "방어력";
            UniqueValueText.text = temp.ValueOne.ToString("N0");
            UniqueText.text += $"\n{ResourceManager.GetInstance().NameSheet.GetUINameValue(temp.apixList.statLine.Item1.ToString())}";
            UniqueValueText.text += $"\n{temp.apixList.statLine.Item2}";
            TypeText.text = $"방어구({ResourceManager.GetInstance().NameSheet.GetEquipNameValue(equips.itemCode)})";
            ApixText.text = string.Empty;
            ApixValueText.text = string.Empty;
            for (int i = 0; i < temp.apixList.abilityApixes.Length; i++)
            {
                ApixText.text += ("+" + ResourceManager.GetInstance().NameSheet.GetUINameValue(temp.apixList.abilityApixes[i].Item1.ToString()) + "\n");
                ApixValueText.text += temp.apixList.abilityApixes[i].Item2.ToString("N4") + "\n";
            }
        }

        PriceText.text = equips.SellValue.ToString()+ 'G';
        FlavorText.text = string.Empty;
        TR.gameObject.SetActive(true);
    }

    public void SetText(Miscs miscs,Vector3 pos)
    {
        TR.position = pos;
        RsetTexts();
        NameText.text = miscs.itemName;
        PriceText.text = miscs.SellValue.ToString();

        TR.gameObject.SetActive(true);
    }
    public void SetText(SkillInfoInGame skill,Vector3 pos)
    {
        if (skill.nowSkillLevel <= 0) return;
        RsetTexts();
        TR.position = pos;
        NameText.text = skill.skillName+ $"{skill.nowSkillLevel}/{skill.maxSkillLevel}";
        RankText.text = $"기본 시전 시간 : {skill.skill[skill.CastingSkillLevel].defaultCastingTime.ToString("N2")}";
        UniqueText.text = "스킬 종류 : ";
        UniqueValueText.text = ResourceManager.GetInstance().NameSheet.GetUINameValue(skill.skillType.ToString());

        UniqueText.text += $"\n기본 수치 : ";
        UniqueValueText.text += $"\n{skill.skill[skill.CastingSkillLevel].defaultValue.ToString("N0")}";

        ApixText.text += $"\n사거리 : ";
        ApixValueText.text += $"\n{skill.skill[skill.CastingSkillLevel].skillRange}";

        ApixText.text += $"\n시전 범위 : ";
        ApixValueText.text += $"\n{ResourceManager.GetInstance().NameSheet.GetUINameValue(skill.objectiveType.ToString())}" +
            (skill.objectiveType == ObjectiveType.Bounded ? skill.skill[skill.CastingSkillLevel].SkillBound.ToString() : string.Empty);
        string coefficType = string.Empty;
        switch (skill.skill[skill.CastingSkillLevel].coefficientType)
        {
            case ValueType.Physical:
                coefficType = "근접 공격력";
                break;
            case ValueType.Magic:
                coefficType = "주문력";
                break;
            case ValueType.PhysicalRange:
                coefficType = "원거리 공격력";
                break;
        }

        PriceText.text = $"{ResourceManager.GetInstance().NameSheet.GetUINameValue(skill.skill[skill.CastingSkillLevel].damageType.ToString())}," +
            coefficType +
            $"({skill.skill[skill.CastingSkillLevel].coefficient.ToString("N1")})";



        
        FlavorText.text = $"{skill.flavorText}";

        TR.gameObject.SetActive(true);
    }
    public void RsetTexts()
    {
        NameText.text = string.Empty;
        RankText.text = string.Empty;
        TypeText.text = string.Empty;
        OutLine.color = Color.white;
        RankText.color = Color.white;
        UniqueText.text = string.Empty;
        UniqueValueText.text = string.Empty;
        ApixText.text= string.Empty;
        ApixValueText.text= string.Empty;
        PriceText.text= string.Empty;
        FlavorText.text= string.Empty;
    }
}
public enum ConsumType
{
    none,posion,buffItem,food
}
[System.Serializable]
public class DialogStateMachine
{
    public DialogState curr;
    public DialogState[] dialogStates;
    public void ChangeDialog(string title)
    {

        foreach (DialogState item in dialogStates)
        {
            if (item.GetTitle== title)
            {
                if(curr != null) curr.Exit();
                curr = item;
                curr.Enter();
                return;
            }
        }
    }
    public void ChangeDialog(DialogType stateType)
    {

        foreach (DialogState item in dialogStates)
        {
            if (item.StateType == stateType)
            {
                if(curr != null) curr.Exit();
                curr = item;
                curr.Enter();
                return;
            }
        }
    }
    public void ExitDialog()
    {
        curr.Exit();
        curr = null;
    }
}
[System.Serializable]
public class DialogState
{
    public string GetTitle
    {
        get 
        {
            if (data == null) { Debug.LogError("대화 정보가 제대로 입력되지 않았습니다."); return string.Empty; }
            else if (data.title == string.Empty) Debug.LogError("타이틀이 없어용..");

            return data.title;
        }
    }
    public DialogType StateType
    {
        get { return data.type; }
    }
    [SerializeField]private Dialog data;
    private Dialog Data
    {
        get { return data; }
    }
    public DialogState(Dialog dialog)
    {
        this.data = dialog;
    }


    public void Enter()
    {
        Player.Instance.isMoveAble = false;
    }
    public void Execute(int index)
    {
        if (index > data.textData.Length) return;
        else
        {

            //TODO : dialog Text를 할당 후 해당 기능 넣어줘야함
            TextMeshProUGUI temp;
            
/*          if(temp) 
            {
                
                temp.DOText(temp, data.textData[index], 10);
            }*/
        }
    }
    public void Exit() 
    {
        Player.Instance.isMoveAble = true;
    }
}