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
    //�̵� ���� �����ð�
    public float arriveTime;
    [SerializeField]public Node targetNode;
    [SerializeField] public Node currentNode;
    public List<Node> nodePreview = new List<Node>();
    [SerializeField]
    public StateMachine stateMachine;
    public StateMachine StateMachine
    {
        get { return stateMachine; }
    }
    private MeshFilter playerMesh;
    public void Awake()
    {
        instance = this;
        stat = new PlayerStat(3,1);
        playerMesh = GetComponent<MeshFilter>();
        InstallizeStates();
        //��Ÿ�� �κ� �����ʿ�
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
        if (targetHit.Length> 0)
        {
            Debug.Log(targetHit[0].point);
            Node tempNode = GridManager.GetInstance().PositionToNode(targetHit[0].point);
            targetNode = tempNode == null? currentNode : tempNode;
            if (targetNode != null)
            {
                SetCurrentNodeAndPosition();
                currentNode.SetGH(currentNode.nodeCenterPosition, targetNode.nodeCenterPosition);
                nodePreview.Clear();

            }
        }
    }
    
    #region �÷��̾� ��� ���� �Լ�
    private void SetCurrentNodeAndPosition()
    {
        currentNode = GetPlayerNode();
        SetPlayerPositionToCenterPos();
    }
    /// <summary>
    /// �÷��̾��� ��ġ�� ����� �߾����� �ٲ��ִ� �Լ�
    /// </summary>
    private void SetPlayerPositionToCenterPos()
    {
        transform.position = new Vector3(currentNode.nodeCenterPosition.x, currentNode.nodeFloor+(playerMesh.mesh.bounds.size.y * 1.5f), currentNode.nodeCenterPosition.y);
    }
    /// <summary>
    /// �÷��̾� ������Ʈ�� ��ġ�� ������� �Ͽ� Node�� �������ִ� �Լ�
    /// </summary>
    /// <returns></returns>
    private Node GetPlayerNode()
    {
        return GridManager.GetInstance().PositionToNode(transform.position);
    }
    #endregion
    private void PlayerMove()
    {

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
}
