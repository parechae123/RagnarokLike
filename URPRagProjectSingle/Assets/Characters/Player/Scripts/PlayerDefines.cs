using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Player;
using UnityEngine;
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
            public float SkillTimer
            {
                get { return skillCoolTime; }
            }
            protected float durationTime;
            public float DurationTime
            {
                get
                {
                    return durationTime;
                }
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
            Stats stats;
            public AttackState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState, Stats characterStat) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {
                stats = characterStat;
            }
            public override void SetAnimationSpeed(Animator anim)
            {
                anim.speed = stats.attackSpeed;
            }
            public override void Enter()
            {

            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;
                if (stats.attackSpeed < skillTimer)
                {
                    skillTimer = 0;
                    if (stats.target.isCharacterDie)
                    {
                        Player.Instance.StateMachine.ChangeState(nextStateName);
                    }
                    else
                    {
                        stats.AttackTarget();
                    }
                }

            }
            public override void Exit()
            {
                skillTimer = stats.attackSpeed;
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



        public class CastingState : PlayerStates
        {

            public CastingState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
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
                base.Exit();
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
            public Stats(Node initializeNode, float hp, float moveSpeed, float attackSpeed, float attackDamage)
            {
                isCharacterDie = false;
                standingNode = initializeNode;
                standingNode.CharacterOnNode = this;
                HP = hp;
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
            public float moveSpeed; //초당 이동하는 타일 수
            public float abilityPower;
            public float attackDamage;
            public float attackSpeed;
            public Stats target;
            public void AttackTarget(float damage = float.MinValue)
            {

                target.HP -= damage == float.MinValue ? attackDamage : damage;
                Debug.Log(target.HP);
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
            public PlayerStat(Node initializeNode, float hp, float moveSpeed, float attackSpeed, float attackDamage) : base(initializeNode, hp, moveSpeed, attackSpeed, attackDamage)
            {

            }
        }
    }
    public class PlayerUi
    {
        //프리팹 형태로 불러옴
        public GameObject UILoad(string name)
        {
            return Resources.Load<GameObject>(name);
        }
    }

}

