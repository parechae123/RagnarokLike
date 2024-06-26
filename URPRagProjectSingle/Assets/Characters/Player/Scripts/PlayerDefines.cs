using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace PlayerDefines
{
    namespace States
    {
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
            protected KeyCode boundedKey;
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
            public PlayerStates(KeyCode keyCode,float coolTime,float durationTime, string targetStateName,string nextStateName,bool isCancelableState)
            {
                boundedKey = keyCode;
                skillCoolTime = coolTime;
                skillTimer = coolTime;
                this.stateName = targetStateName;
                this.nextStateName = nextStateName; 
                this.isCancelableState = isCancelableState;
                this.durationTime = durationTime;
            }
            public virtual void Enter()
            {
                Debug.Log("��" + stateName);
                skillTimer = 0;
            }
            public virtual void Execute()
            {
                Debug.Log("������"+stateName);
                skillTimer += Time.deltaTime;
                if (skillCoolTime < skillTimer)
                {
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                }
            }

            public virtual void Exit()
            {
                Debug.Log("���¿��� ����" + stateName);
                skillTimer = skillCoolTime;
            }
            public virtual void PlayerAction()
            {
                //��Ÿ���� �����Ǿ��� ��
                if (skillTimer >= skillCoolTime)
                {
                    if (Input.GetKeyDown(boundedKey))
                    {
                        Player.Instance.StateMachine.ChangeState(stateName);
                    }
                }

            }

        }




        public class MoveState : PlayerStates
        {
            public MoveState(KeyCode keyCode, float coolTime,float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(keyCode, coolTime,durationTime , targetStateName,nextStateName, isCancelableState)
            {
                
            }
            public override void PlayerAction()
            {
                if (Input.GetKeyDown(boundedKey))
                {
                    Player.Instance.StateMachine.ChangeState(stateName);

                    Player.Instance.SetTargetNode();
                    Player.Instance.PlayerMove();
                }

            }
            public override void Enter()
            {
                base.Enter();
                durationTime = Player.Instance.arriveTime;

            }
            public override void Execute()
            {
                Debug.Log("������" + stateName);
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
            public AttackState(KeyCode keyCode, float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(keyCode, coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {

            }
            public override void PlayerAction()
            {

                base.PlayerAction();
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
        public class IdleState : PlayerStates
        {
            public IdleState(KeyCode keyCode , float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(keyCode, coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {

            }
            public override void PlayerAction()
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
                Debug.Log("���¿��� ����"+stateName);
                skillTimer = 0;
            }
        }



        public class CastingState : PlayerStates
        {
            public CastingState(KeyCode keyCode, float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(keyCode, coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {

            }
            public override void PlayerAction()
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
    [System.Serializable]
    public class PlayerStat
    {
        public float moveSpeed = 3; //�ʴ� �̵��ϴ� Ÿ�� ��
        public float attackSpeed;
        public PlayerStat(float moveSpeed, float attackSpeed)
        {
            this.moveSpeed = moveSpeed;
            this.attackSpeed = attackSpeed;
        }
    }
}

