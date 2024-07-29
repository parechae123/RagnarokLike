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
            //state ���� ���� �� ������ ���ɽ� false �Ұ��ɽ� true
            public bool isCancelableState;
            //TODO : ���� �����ʿ�

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
            /// ������Ʈ ������
            /// </summary>
            /// <param name="keyCode">�Է� Ʈ���� Ű</param>
            /// <param name="coolTime">�ش� �ൿ�� ��Ÿ��</param>
            /// <param name="targetStateName">������Ʈ �̸�</param>
            /// <param name="isCancelableState">���� �� �ٸ� ���¸� ���� ������</param>
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
            //�ش� ������Ʈ�� �������ִ� ĳ������ ����
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
            public Action dieFunctions;//TODO : ��� ���� ����ʿ�
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
            public float moveSpeed; //�ʴ� �̵��ϴ� Ÿ�� ��
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
        //������ ���·� �ҷ���
        public GameObject UILoad(string name)
        {
            return Resources.Load<GameObject>(name);
        }
    }

}

