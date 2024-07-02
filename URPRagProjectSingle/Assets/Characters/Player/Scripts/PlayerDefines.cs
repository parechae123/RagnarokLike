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
            public PlayerStates(float coolTime,float durationTime, string targetStateName,string nextStateName,bool isCancelableState)
            {
                skillCoolTime = coolTime;
                skillTimer = coolTime;
                this.stateName = targetStateName;
                this.nextStateName = nextStateName; 
                this.isCancelableState = isCancelableState;
                this.durationTime = durationTime;
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
            public MoveState( float coolTime,float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base( coolTime,durationTime , targetStateName,nextStateName, isCancelableState)
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
            public AttackState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
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
    [System.Serializable]
    public class Stats
    {
        private bool isCharacterDie = false;
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
                if (hp>=0)
                {
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
        public float attackSpeed;

        public bool IsEnoughSP(float spCost)
        {
            if (sp>= spCost&&!isCharacterDie)
            {
                sp -= spCost;
                return true;
            }
            return false;
        }
    }
    public class PlayerStat : Stats
    {
        public PlayerStat(float moveSpeed, float attackSpeed)
        {
            this.moveSpeed = moveSpeed;
            this.attackSpeed = attackSpeed;
        }
    }
}

