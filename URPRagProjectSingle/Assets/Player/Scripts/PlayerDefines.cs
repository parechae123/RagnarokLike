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
        public class PlayerStates : IState
        {
            public string stateName;
            protected KeyCode functionKey;
            protected float skillCoolTime;
            protected float skillTimer;
            public PlayerStates(KeyCode keyCode,float coolTime, string targetStateName)
            {
                functionKey = keyCode;
                skillCoolTime = coolTime;
                this.stateName = targetStateName;
            }
            public virtual void Enter()
            {
                Debug.Log("��");
                skillTimer = 0;
            }
            public virtual void Execute()
            {
                if (skillTimer >= skillCoolTime)
                {
                    Exit();
                }
                else
                {
                    Debug.Log("������");
                    skillTimer += Time.deltaTime;
                }
            }
            public virtual void Exit()
            {
                Debug.Log("���¿��� ����");
                skillTimer = skillCoolTime;
            }
            public virtual void PlayAction()
            {
                if (skillTimer >= skillCoolTime)
                {
                    if (Input.GetKeyDown(functionKey))
                    {
                        Enter();
                    }
                }
                Execute();
            }

        }
        public class MoveState : PlayerStates
        {
            public MoveState(KeyCode keyCode ,float coolTime,string stateName) : base(keyCode, coolTime , stateName)
            {
                
            }
            public override void PlayAction()
            {

                base.PlayAction();
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
        public class AttackState : PlayerStates
        {
            public AttackState(KeyCode keyCode ,float coolTime,string stateName) : base(keyCode, coolTime , stateName)
            {
                
            }
            public override void PlayAction()
            {

                base.PlayAction();
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
            public IdleState(KeyCode keyCode ,float coolTime,string stateName) : base(keyCode, coolTime , stateName)
            {
                
            }
            public override void PlayAction()
            {

                base.PlayAction();
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
        public class CastingState : PlayerStates
        {
            public CastingState(KeyCode keyCode ,float coolTime,string stateName) : base(keyCode, coolTime , stateName)
            {
                
            }
            public override void PlayAction()
            {

                base.PlayAction();
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
    public class PlayerStat
    {
        float moveSpeed = 3; //�ʴ� �̵��ϴ� Ÿ�� ��
    }
}

