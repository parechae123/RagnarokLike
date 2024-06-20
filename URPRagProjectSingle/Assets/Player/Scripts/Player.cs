using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerDefines.States;
using NeutralDefines.State;
using PlayerDefines;
using System;

public class Player : MonoBehaviour
{
    private PlayerStat stat;
    private static Player instance;
    public static Player Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new GameObject("Player").AddComponent<Player>();
            }

            return instance;
        }
    }
    public KeyCode tempKeyCode;
    //이동 도착 예정시간
    public float arriveTime;
    [SerializeField]
    public StateMachine stateMachine;
    public StateMachine StateMachine
    {
        get { return stateMachine; }
    }

    public void Awake()
    {
        instance = this;
        stat = new PlayerStat(3,1);
        installizeStates();
        //쿨타임 부분 수정필요
    }
    public void Update()
    {
        for (sbyte i = 0; i < StateMachine.AllStates.Length; i++)
        {
            StateMachine.AllStates[i].PlayerAction();
        }
        StateMachine.CurrentState.Execute();
    }

    private void installizeStates()
    {
        Queue<PlayerStates> states = new Queue<PlayerStates>();
        states.Enqueue(new MoveState(KeyCode.Mouse0, 1, 1, "moveState", "idleState", false));
        states.Enqueue(new IdleState(KeyCode.None, 1, 1, "idleState", "idleState", true));
        states.Enqueue(new AttackState(KeyCode.Mouse1, 1, stat.attackSpeed, "attackState", "idleState", false));
        stateMachine = new StateMachine(states.ToArray());
        StateMachine.ChangeState("idleState");
    }
}
