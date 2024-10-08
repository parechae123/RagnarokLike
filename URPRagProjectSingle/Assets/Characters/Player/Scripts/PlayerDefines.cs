using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
namespace PlayerDefines
{
    namespace States
    {
        using PlayerDefines.Stat;
        public interface IState
        {
            void Enter();
            void Execute();
            void Exit();
        }
        [System.Serializable]
        public class PlayerStates : IState
        {
            public string stateName;
            public string nextStateName;
            //state 도중 끊을 수 없는지 가능시 false 불가능시 true
            public bool isCancelableState;
            //TODO : 방향 지정필요

            protected float skillCoolTime;
            protected float skillTimer;
            protected float durationTime;
            public float DurationTime
            {
                get;
                set; 
            }
            /// <summary>
            /// 스테이트 생성자
            /// </summary>
            /// <param name="keyCode">입력 트리거 키</param>
            /// <param name="coolTime">해당 행동의 쿨타임</param>
            /// <param name="targetStateName">스테이트 이름</param>
            /// <param name="isCancelableState">상태 중 다른 상태를 받을 것인지</param>
            public PlayerStates(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState)
            {
                skillCoolTime = coolTime;
                skillTimer = coolTime;
                this.stateName = targetStateName;
                this.nextStateName = nextStateName;
                this.isCancelableState = isCancelableState;
                this.durationTime = durationTime;
            }

            public virtual void SetAnimationSpeed(Animator anim)
            {
                anim.speed = 1;
            }
            public virtual void Enter()
            {
                skillTimer = 0;
            }
            public virtual void Execute()
            {
                skillTimer += Time.deltaTime;
                if (skillCoolTime < skillTimer)
                {
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                }
            }

            public virtual void Exit()
            {
                skillTimer = skillCoolTime;
            }


        }




        public class MoveState : PlayerStates
        {
            public MoveState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {

            }
            public override void Enter()
            {
                base.Enter();
                durationTime = Player.Instance.arriveTime;

            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;
                if (durationTime < skillTimer)
                {
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                }

            }
            public override void Exit()
            {
                base.Exit();
            }
        }




        public class AttackState : PlayerStates
        {
            //해당 스테이트를 가지고있는 캐릭터의 스텟
            public AttackState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState, Stats characterStat) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {
                
            }
            public override void SetAnimationSpeed(Animator anim)
            {
                anim.speed = Player.Instance.playerLevelInfo.stat.attackSpeed;
            }
            public override void Enter()
            {

            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;
                if (Player.Instance.playerLevelInfo.stat.attackSpeed < skillTimer)
                {
                    skillTimer = 0;
                    if (Player.Instance.playerLevelInfo.stat.target.isCharacterDie)
                    {
                        Player.Instance.StateMachine.ChangeState(nextStateName);
                    }
                    else
                    {
                        Player.Instance.playerLevelInfo.stat.AttackTarget();
                    }
                }

            }
            public override void Exit()
            {
                skillTimer = Player.Instance.playerLevelInfo.stat.attackSpeed;
            }
        }
        public class CastingState : PlayerStates
        {
            public Action<Vector3,Stats,Stats> casting;
            public Vector3 castPos;
            public Stats targetStat;
            public CastingState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {
                
            }
            public override void SetAnimationSpeed(Animator anim)
            {
                anim.speed = Player.Instance.playerLevelInfo.stat.attackSpeed;
                //TODO : status int,dex 구현 시 castingTime으로 변환해야함
            }
            public override void Enter()
            {
                skillTimer = 0;
            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;
                UIManager.GetInstance().SetCastingBarValue(durationTime, skillTimer);
                if (DurationTime <= skillTimer)
                {
                    skillTimer = 0;
                    UIManager.GetInstance().CastingBarOnOff(false);
                    casting.Invoke(castPos,targetStat,Player.Instance.playerLevelInfo.stat);
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                }

            }
            public override void Exit()
            {
                UIManager.GetInstance().CastingBarOnOff(false);
                skillTimer = 0;
            }
        }
        public class IdleState : PlayerStates
        {
            public IdleState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {

            }
            public override void Enter()
            {
                base.Enter();
            }
            public override void Execute()
            {

                base.Execute();


            }
            public override void Exit()
            {
                skillTimer = 0;
            }
        }
    }
    namespace Stat
    {
        [System.Serializable]
        public class Stats
        {
            public bool isCharacterDie
            {
                get;
                private set;
            }
            public Action<Vector3, bool> moveFunction;
            public Action dieFunctions;//TODO : 사망 연출 등록필요
            public EquipStat equipStat = new EquipStat(0,0,0,0,0,0);
            
            public Stats(Node initializeNode, float hp,float sp, float moveSpeed, float attackSpeed, float attackDamage,byte attackRange)
            {
                isCharacterDie = false;
                standingNode = initializeNode;
                standingNode.CharacterOnNode = this;
                HP = hp;
                this.sp = sp;
                this.charactorAttackRange = attackRange;
                this.moveSpeed = moveSpeed;
                this.attackSpeed = attackSpeed;
                this.attackDamage = attackDamage;
            }
            public Node standingNode
            {
                get;
                set;
            }
            private float hp;
            public float HP
            {
                get
                {
                    return hp;
                }
                set
                {
                    Debug.Log((value- hp) +"몬스터 데미지");
                    hp = value;
                    if (hp <= 0)
                    {
                        dieFunctions?.Invoke();
                        isCharacterDie = true;
                    }
                }
            }
            private float sp
            {
                get;
                set;
            }
            private float moveSpeed;
            public float MoveSpeed 
            {
                get { return Player.Instance.playerLevelInfo.stat.moveSpeed + equipStat.EquipMoveSpeed; }
                set { }
            } //초당 이동하는 타일 수
            public float abilityPower;
            public float attackDamage;
            public float attackSpeed;
            public byte charactorAttackRange;
            public float CastTimePercent
            {
                get { return 0; /*DexInt같은 능력치 추가 후 바꿔야함*/}
            }
            

            public Stats target;
            public void AttackTarget(float damage = float.MinValue)
            {

                target.HP -= damage == float.MinValue ? attackDamage : damage;
            }

            public bool IsEnoughSP(float spCost)
            {
                if (sp >= spCost && !isCharacterDie)
                {
                    sp -= spCost;
                    return true;
                }
                return false;
            }
        }
        public class PlayerStat : Stats
        {
            public PlayerStat(Node initializeNode, float hp,float sp, float moveSpeed, float attackSpeed, float attackDamage,byte attackRange) : base(initializeNode, hp,sp, moveSpeed, attackSpeed, attackDamage,attackRange)
            {

            }
        }
    }
    public class PlayerUI
    {
        private Canvas mainCanvas;
        public RectTransform skillTreeUI;
        public RectTransform statusUI;
        public RectTransform equipUI;
        public RectTransform inventoryUI;
        public RectTransform escUI;
        public RectTransform quickSlotUI;
        public Stack<RectTransform> uiStack;
        public PlayerUI() 
        {
            ResetUI();
        }
        public void ResetUI()
        {
            ResourceManager.GetInstance().LoadAsync<GameObject>("InGameCanvas", (UIs) =>
            {
                uiStack = uiStack ?? new Stack<RectTransform>();
                uiStack.Clear();
                if (mainCanvas != null) GameObject.Destroy(mainCanvas);

                mainCanvas = GameObject.Instantiate(UIs).GetComponent<Canvas>();

                skillTreeUI = mainCanvas.transform.Find("skillTreeUI").transform as RectTransform;
                skillTreeUI.gameObject.SetActive(false);

                statusUI = mainCanvas.transform.Find("statusUI").transform as RectTransform;
                statusUI.gameObject.SetActive(false);

                equipUI = mainCanvas.transform.Find("equipUI").transform as RectTransform;
                equipUI.gameObject.SetActive(false);

                inventoryUI = mainCanvas.transform.Find("inventoryUI").transform as RectTransform;
                inventoryUI.gameObject.SetActive(false);

                escUI = mainCanvas.transform.Find("escUI").transform as RectTransform;
                escUI.gameObject.SetActive(false);

                quickSlotUI = mainCanvas.transform.Find("quickSlotUI").transform as RectTransform;
                quickSlotUI.gameObject.SetActive(false);
            });
        }
        /// <summary>
        /// 마지막으로 켠 UI를 ActiveFalse해주고 uiStack에서 제거
        /// </summary>
        /// <param name="targetUI">해당 매개변수 등록 시 대상 UI만 ActiveFalse해줌</param>
        public void PopUI(RectTransform targetUI = null)
        {
            if (uiStack.Count <= 0) return;

            if (targetUI != null)
            {
                if (uiStack.Contains(targetUI))
                {
                    Stack<RectTransform> tempStack = new Stack<RectTransform>();
                    RectTransform tempRect;
                    for (int i = 0; i < uiStack.Count; i++)
                    {
                        tempRect = tempStack.Pop();
                        if (tempRect != targetUI)
                        {
                            tempStack.Push(tempRect);
                        }
                        else
                        {
                            targetUI.gameObject.SetActive(false);
                        }
                    }
                }

            }
            else
            {
                uiStack.Pop().gameObject.SetActive(false);
            }


        }
        /// <summary>
        /// uiStack인스턴스에 타겟 UI의 RectTransform을 등록,gameobject의 active를 true로 바꿔줌
        /// </summary>
        /// <param name="targetUI">활성화 시킬 UI</param>
        public void PushUI(RectTransform targetUI)
        {
            uiStack.Push(targetUI);
            targetUI.gameObject.SetActive(true);
        }
    }
}

