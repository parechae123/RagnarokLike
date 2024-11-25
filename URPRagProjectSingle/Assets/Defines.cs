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
public class Positions : ItemInfo
{
    private Sprite itemIcon;
    protected Sprite ItemIcon
    {
        get { return itemIcon; }
    }
    private int amount;
    protected int ItemAmount
    {
        get { return amount; }
    }
    public void UseItem()
    {

    }
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
    private TextMeshProUGUI valueText;
    public TextMeshProUGUI ValueText
    {
        get
        {
            if (valueText == null)
            {
                valueText = TR.Find("ValueText").GetComponent<TextMeshProUGUI>();
            }
            return valueText;
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

    public void SetText(Consumables consumables)
    {
        
    }
    public void SetText(Equips equips,int apixCount)
    {
        Color itemColor = Color.white;
        switch (apixCount)
        {
            case 0:
                itemColor = Color.white;
                RankText.text = "일반";
                RankText.color = itemColor;
                break;
            case 1:
                itemColor = Color.blue;
                RankText.text = "고급";
                RankText.color = itemColor;
                break;
            case 2:
                itemColor = Color.yellow;
                RankText.text = "희귀";
                RankText.color = itemColor;
                break;
            case 3:
                itemColor = new Color32(243, 115, 33, 255);
                RankText.text = "전설";
                RankText.color = itemColor;
                break;
            default:
                RankText.text = "알 수 없음";
                RankText.color = itemColor;
                break;
        }
        NameText.text = equips.itemName;
        if(equips.GetPart == EquipPart.LeftHand|| equips.GetPart == EquipPart.RightHand|| equips.GetPart == EquipPart.TwoHanded)
        {
            TypeText.text = $"무기({ResourceManager.GetInstance().NameSheet.GetEquipNameValue(((Weapons)equips).WeaponType.ToString())})";
        }
        else
        {
            TypeText.text = $"방어구({ResourceManager.GetInstance().NameSheet.GetEquipNameValue(((Weapons)equips).WeaponType.ToString())})";
        }
        FlavorText.text = string.Empty;
        //TODO : 어픽스와 골드만 하면됨
        
    }
    public void SetText(Miscs miscs)
    {

    }
    public void SetText(SkillInfoInGame skill)
    {

    }
}