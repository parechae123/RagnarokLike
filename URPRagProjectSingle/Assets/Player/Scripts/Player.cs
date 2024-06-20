using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerDefines.States;
using NeutralDefines.State;

public class Player : MonoBehaviour
{
    public KeyCode tempKeyCode;
    public float arriveTime;
    private StateMachine stateMachine;
    public static Player instance;
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

    public void Awake()
    {
        Queue<PlayerStates> states = new Queue<PlayerStates>();
        states.Enqueue(new MoveState(KeyCode.Mouse0,0,"moveState"));
        //ÄðÅ¸ÀÓ ºÎºÐ 
    }
    public void Update()
    {
        stateMachine.CurrentState?.Execute();
    }
}
