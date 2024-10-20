using DG.Tweening;
using PlayerDefines.Stat;
using PlayerDefines.States;
using System;
using UnityEditor;
using UnityEngine;

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
                    if (!CurrentState.isCancelableState&&CurrentState.durationTime>= CurrentState.GetTimer) return;
                }
                anim.speed = 1;
                currentState?.Exit();                   //���� ���°��� ����������
                currentState = SearchState(newStateName);               //�μ��� �޾ƿ� ���°��� �Է�
                currentState?.Enter();                  //���� ���°�
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
                skillPos.y += 0.9f;
                if(targetStat != null&&skillInfo.objectiveType == ObjectiveType.OnlyTarget)
                {
                    skillPos = (targetStat.standingNode.worldPos + Player.Instance.playerLevelInfo.stat.standingNode.worldPos);
                    skillPos.x = skillPos.x / 2f;
                    skillPos.z = skillPos.z / 2f;
                }

                if (Player.Instance.playerLevelInfo.stat.IsEnoughSP(skillInfo.skill[skillInfo.CastingSkillLevel].spCost))
                {
                    Debug.Log("���� ���, ĳ���� ����");
                }
                else
                {
                    Debug.Log("���� �����, ĳ���� ���");
                    return;
                }

                //Ÿ�� �� ���� �� ��ų��ġ �ʱ�ȭ
                temp.casting = null;
                temp.targetStat = null;
                temp.castPos = Vector3.down*100;

                temp.casting += skillInfo.SkillCastTargetPlace;
                temp.castPos = skillPos;
                temp.targetStat = targetStat;
                if(skillInfo.skillPosition != SkillPosition.self) Player.Instance.SetCharactorDirrection(Player.Instance.transform.position, skillPos);
                currentState?.Exit();                   //���� ���°��� ����������
                currentState = temp;               
                currentState.durationTime = castingTime*Player.Instance.playerLevelInfo.stat.CastTimePercent;
                currentState?.Enter();                  //���� ���°�
                AnimationChange();
            }
            public void AnimationChange()
            {
                currentState?.SetAnimationSpeed(anim);
                anim.Play(currentState.stateName + animationDirrection);
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
            [SerializeField] private Texture2D grabCursorIMG;
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
                    case cursorState.grabCursor:
                        Cursor.SetCursor(grabCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
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
            defaultCurser, noneClickAbleState, grabCursor, attackAble, skillTargeting
        }

    }
    
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
    //0������ ���� ������ ���� �� SlotNumber�� �ƴ�
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
    /// pureStat�� ������Ű�� ������ �Լ�, (���� UI�� �����ؾ���)
    /// </summary>
    /// <param name="statType">���� ������ ����</param>
    /// <param name="statusPoint">PlayerStats�� LeftStatusPoint�� �־���</param>
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
                Debug.LogError("�������� ���� ���ݿ���Դϴ�");
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
