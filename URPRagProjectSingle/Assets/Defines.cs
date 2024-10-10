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
                temp.castPos = skillPos;
                temp.targetStat = targetStat;

                currentState?.Exit();                   //이전 상태값을 빠져나간다
                currentState = temp;               
                currentState.durationTime = castingTime;
                currentState?.Enter();                  //다음 상태값
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
    //0이하의 갑을 가지고 있을 시 SlotNumber가 아님
    public int SlotNumber
    {
        get { return ((int)UIType - (int)UITypes.QuickSlotOne ); }
    }
    public bool needCombKey;
    public Action subScribFuncs;
}