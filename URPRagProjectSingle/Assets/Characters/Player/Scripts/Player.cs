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
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Plugins;


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
    [SerializeField] public Node targetNode;
    [SerializeField] public Node currentNode;
    public LinkedList<Node> nodePreview = new LinkedList<Node>();

    [SerializeField]
    public StateMachine stateMachine;
    public StateMachine StateMachine
    {
        get { return stateMachine; }
    }
    private SpriteRenderer playerSR;
    [SerializeField] CursorStates playerCursorState = new CursorStates();
    public void Awake()
    {
        instance = this;
        stat = new PlayerStat(3, 1);
        playerSR = GetComponent<SpriteRenderer>();
        InstallizeStates();
        //��Ÿ�� �κ� �����ʿ�
    }
    private void Start()
    {
        SetCurrentNodeAndPosition();
        playerCursorState.SetDefaultCursor();
    }
    public void Update()
    {
        MouseBinding();
        StateMachine.CurrentState.Execute();

    }
    public bool SetTargetNode(Vector3 point)
    {
        Debug.Log(point);
        Node tempNode = GridManager.GetInstance().PositionToNode(point);

        if (tempNode != null)
        {
            if (targetNode == tempNode) return false;        //������ ���ߴ� ������ ���� ���� ������ ����Ͻ� �۵� ����
            targetNode = tempNode;
            nodePreview.Clear();
            SetCurrentNodeAndPosition();
            currentNode.SetGH(currentNode.nodeCenterPosition, targetNode.nodeCenterPosition);

            nodePreview = GridManager.GetInstance().PathFinding(currentNode.nodeCenterPosition, targetNode.nodeCenterPosition);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Ÿ�ٸ��� ����
    /// </summary>
    /// <param name="monsterTR"></param>
    /// <returns></returns>
    public bool SetTargetMonster(Transform monsterTR)
    {
        if (monsterTR != null) return true;
        return false;
    }

    #region �÷��̾� ��� ���� �Լ�
    public void SetCurrentNodeAndPosition()
    {
        currentNode = GridManager.GetInstance().PositionToNode(transform.position);
    }
    /// <summary>
    /// �÷��̾��� ��ġ�� ����� �߾����� �ٲ��ִ� �Լ�
    /// </summary>
    private void SetPlayerPositionToCenterPos()
    {
        transform.position = new Vector3(currentNode.nodeCenterPosition.x, currentNode.nodeFloor + (playerSR.bounds.size.y) + 0.5f, currentNode.nodeCenterPosition.y);
    }
    #endregion
    #region ������,���� ����

    public void PlayerMove()
    {
        float moveSpeedPerSec = 1 / stat.moveSpeed;
        if (nodePreview.Count <= 0) return;
        if (currentNode == nodePreview.First())
        {
            nodePreview.RemoveFirst();
        }
        Vector3[] tempNodePosArray = new Vector3[nodePreview.Count];

        for (short i = 0; i < tempNodePosArray.Length; i++)
        {
            if (currentNode != nodePreview.First())
            {
                tempNodePosArray[i].x = nodePreview.First().nodeCenterPosition.x;
                tempNodePosArray[i].y = nodePreview.First().nodeFloor + (playerSR.bounds.size.y) + 0.5f;
                tempNodePosArray[i].z = nodePreview.First().nodeCenterPosition.y;
            }
            else
            {
                --i;
            }
            nodePreview.RemoveFirst();
        }
        Path tempPath = new Path(PathType.Linear, tempNodePosArray, 1);
        transform.DOKill();
        DOPath(transform, tempPath, tempNodePosArray.Length * moveSpeedPerSec).SetEase(Ease.Linear);
        arriveTime = tempNodePosArray.Length * moveSpeedPerSec;
        return;


        /*        Vector3 targetVector = new Vector3(nodePreview.First().nodeCenterPosition.x, currentNode.nodeFloor + (playerSR.bounds.size.y) + 0.5f, nodePreview.First().nodeCenterPosition.y);
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
                        //SetPlayerPositionToCenterPos();
                    }


                }).SetEase(Ease.Linear);*/

    }
    public TweenerCore<Vector3, Path, PathOptions> DOPath(Transform target, Path path, float duration, PathMode pathMode = PathMode.Full3D)
    {
        TweenerCore<Vector3, Path, PathOptions> tweenerCore = DOTween.To(PathPlugin.Get(), () => target.position, delegate (Vector3 x)
        {
            target.position = x;
            currentNode = GridManager.GetInstance().PositionToNode(target.position);
        }, path, duration).SetTarget(target);
        tweenerCore.plugOptions.mode = pathMode;
        return tweenerCore;
    }
    #endregion
    #region Ű ���ε�
    /// <summary>
    /// ���콺�� �������� ���
    /// </summary>
    public void MouseBinding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] groundHit = Physics.RaycastAll(ray, 1000f, 8);
        RaycastHit[] monsterHit = Physics.RaycastAll(ray, 1000f, 64);
        if (monsterHit.Length > 0)
        {
            playerCursorState.changeState(cursorState.attackAble);
        }
        else
        {
            if(groundHit.Length > 0)
            {
                playerCursorState.changeState(cursorState.defaultCurser);
            }
            else
            {
                playerCursorState.changeState(cursorState.noneClickAbleState);
            }
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (monsterHit.Length > 0)
            {
                if (SetTargetMonster(monsterHit[0].transform))
                {
                    StateMachine.ChangeState("attackState");
                }
            }
            else if (groundHit.Length > 0)
            {
                if (SetTargetNode(groundHit[0].point))
                {
                    PlayerMove();
                    StateMachine.ChangeState("moveState");
                }
            }

        }
    }
    #endregion
    private void InstallizeStates()
    {
        Queue<PlayerStates> states = new Queue<PlayerStates>();
        states.Enqueue(new MoveState( 1, 1, "moveState", "idleState", false));
        states.Enqueue(new IdleState(1, 1, "idleState", "idleState", true));
        states.Enqueue(new AttackState(1, stat.attackSpeed, "attackState", "idleState", false));
        stateMachine = new StateMachine(states.ToArray());
        StateMachine.ChangeState("idleState");
    }
    private void OnDrawGizmos()
    {
        if (nodePreview == null) return;
        if (nodePreview.Count < 0) return;
        Node[] tempNodeArray = nodePreview.ToArray();
        for (int i = 0; i < nodePreview.Count; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(tempNodeArray[i].nodeCenterPosition.x, transform.position.y, tempNodeArray[i].nodeCenterPosition.y), Vector3.one);
        }
    }
}
