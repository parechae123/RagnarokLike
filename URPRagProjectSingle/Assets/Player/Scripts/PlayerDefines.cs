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
                Debug.Log("들어감");
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
                    Debug.Log("도는중");
                    skillTimer += Time.deltaTime;
                }
            }
            public virtual void Exit()
            {
                Debug.Log("상태에서 나감");
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
        float moveSpeed = 3; //초당 이동하는 타일 수
    }
}

