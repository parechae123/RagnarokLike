using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerDefines.States;
using NeutralDefines.State;
using PlayerDefines;
using System;
using UnityEditor;
using System.Linq;
using DG.Tweening;

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
    [SerializeField]public Node targetNode;
    [SerializeField] public Node currentNode;
    public LinkedList<Node> nodePreview = new LinkedList<Node>();
    [SerializeField]
    public StateMachine stateMachine;
    public StateMachine StateMachine
    {
        get { return stateMachine; }
    }
    private SpriteRenderer playerSR;
    public void Awake()
    {
        instance = this;
        stat = new PlayerStat(3,1);
        playerSR = GetComponent<SpriteRenderer>();
        InstallizeStates();
        //쿨타임 부분 수정필요
    }
    private void Start()
    {
        SetCurrentNodeAndPosition();
    }
    public void Update()
    {
        for (sbyte i = 0; i < StateMachine.AllStates.Length; i++)
        {
            StateMachine.AllStates[i].PlayerAction();
        }
        StateMachine.CurrentState.Execute();

    }
    public void SetTargetNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] targetHit = Physics.RaycastAll(ray, 1000f,8);
        transform.DOKill();
        nodePreview.Clear();
        transform.DOKill();
        if (targetHit.Length> 0)
        {
            Debug.Log(targetHit[0].point);
            Node tempNode = GridManager.GetInstance().PositionToNode(targetHit[0].point);
            targetNode = tempNode == null? currentNode : tempNode;
            if (targetNode != null)
            {
                SetCurrentNodeAndPosition();
                currentNode.SetGH(currentNode.nodeCenterPosition, targetNode.nodeCenterPosition);
                nodePreview = GridManager.GetInstance().PathFinding(currentNode.nodeCenterPosition, targetNode.nodeCenterPosition);
                
            }
        }
    }
    
    #region 플레이어 노드 관련 함수
    private void SetCurrentNodeAndPosition()
    {
        currentNode = GetPlayerNode();
        SetPlayerPositionToCenterPos();
    }
    /// <summary>
    /// 플레이어의 위치를 노드의 중앙으로 바꿔주는 함수
    /// </summary>
    private void SetPlayerPositionToCenterPos()
    {
        transform.position = new Vector3(currentNode.nodeCenterPosition.x, currentNode.nodeFloor+(playerSR.bounds.size.y)+0.5f, currentNode.nodeCenterPosition.y);
    }
    /// <summary>
    /// 플레이어 오브젝트의 위치를 기반으로 하여 Node를 리턴해주는 함수
    /// </summary>
    /// <returns></returns>
    private Node GetPlayerNode()
    {
        return GridManager.GetInstance().PositionToNode(transform.position);
    }
    #endregion
    public void PlayerMove()
    {
        float moveSpeedPerSec = 1/stat.moveSpeed;
        if (nodePreview.Count <= 0) return;
        Vector3 targetVector = new Vector3(nodePreview.First().nodeCenterPosition.x, currentNode.nodeFloor + (playerSR.bounds.size.y) + 0.5f, nodePreview.First().nodeCenterPosition.y);
        transform.DOMove(targetVector, moveSpeedPerSec).OnComplete(() =>
        {
            if (nodePreview.Count > 0)
            {
                currentNode = nodePreview.First();
                nodePreview.RemoveFirst();
                PlayerMove();
            }
            else
            {
                SetPlayerPositionToCenterPos();
            }


        }).SetEase(Ease.Linear);
        
    }
    private void InstallizeStates()
    {
        Queue<PlayerStates> states = new Queue<PlayerStates>();
        states.Enqueue(new MoveState(KeyCode.Mouse0, 1, 1, "moveState", "idleState", false));
        states.Enqueue(new IdleState(KeyCode.None, 1, 1, "idleState", "idleState", true));
        states.Enqueue(new AttackState(KeyCode.Mouse1, 1, stat.attackSpeed, "attackState", "idleState", false));
        stateMachine = new StateMachine(states.ToArray());
        StateMachine.ChangeState("idleState");
    }
    private void OnDrawGizmos()
    {
        if ( nodePreview == null) return;
        if (nodePreview.Count < 0) return;
        Node[] tempNodeArray = nodePreview.ToArray();
        for (int i = 0; i < nodePreview.Count; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(tempNodeArray[i].nodeCenterPosition.x,transform.position.y, tempNodeArray[i].nodeCenterPosition.y), Vector3.one);
        }
    }
}
