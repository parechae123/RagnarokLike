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
using PlayerDefines.Stat;


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
    [SerializeField] public Node targetNode;
    [SerializeField] private Node currentNode;
    [SerializeField] public Node CurrentNode
    {
        get { return currentNode; }
        set 
        {
            stat.standingNode.CharacterOnNode = stat.standingNode.CharacterOnNode == stat ? null : stat.standingNode.CharacterOnNode;
            stat.standingNode = value;
            stat.standingNode.CharacterOnNode = stat;
            currentNode = value;
        }
    }
    public LinkedList<Node> nodePreview = new LinkedList<Node>();

    [SerializeField]
    public PlayerStateMachine stateMachine;
    public PlayerStateMachine StateMachine
    {
        get { return stateMachine; }
    }

    private SpriteRenderer playerSR;
    [SerializeField] private Transform targetCell;
    [SerializeField] CursorStates playerCursorState = new CursorStates();
    [SerializeField]Vector2Int playerLookDir;
    public Vector2Int PlayerLookDir
    {
        get { return playerLookDir; }
    }
    public void Awake()
    {
        instance = this;

        playerSR = GetComponent<SpriteRenderer>();

        //쿨타임 부분 수정필요
    }
    private void Start()
    {
        stat = new PlayerStat(currentNode, 100, 3, 1,10);
        InstallizeStates();
        SetCurrentNodeAndPosition();
        stat.moveFunction += PlayerMoveOrder;
        playerCursorState.SetDefaultCursor();
    }
    public void Update()
    {
        MouseBinding();
        StateMachine.CurrentState.Execute();

    }
    public bool SetTargetNode(Vector3 point)
    {
        Node tempNode = GridManager.GetInstance().PositionToNode(point);

        if (tempNode != null)
        {
            if (targetNode == tempNode) return false;        //이전에 구했던 목적지 노드와 같은 목적지 노드일시 작동 제한
            targetNode = tempNode;
            nodePreview.Clear();
            SetCurrentNodeAndPosition();
            CurrentNode.SetGH(CurrentNode.nodeCenterPosition, targetNode.nodeCenterPosition);

            nodePreview = GridManager.GetInstance().PathFinding(CurrentNode.nodeCenterPosition, targetNode.nodeCenterPosition);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 타겟몬스터 세팅
    /// </summary>
    /// <param name="monsterTR"></param>
    /// <returns></returns>
    public bool SetTargetMonster(Transform monsterTR)
    {
        if (monsterTR != null) return true;
        return false;
    }

    #region 플레이어 노드 관련 함수
    public void SetCurrentNodeAndPosition()
    {
        CurrentNode = GridManager.GetInstance().PositionToNode(transform.position);
    }
    /// <summary>
    /// 플레이어의 위치를 노드의 중앙으로 바꿔주는 함수
    /// </summary>
    private void SetPlayerPositionToCenterPos()
    {
        transform.position = new Vector3(CurrentNode.nodeCenterPosition.x, CurrentNode.nodeFloor + (playerSR.bounds.size.y) + 0.5f, CurrentNode.nodeCenterPosition.y);
    }
    #endregion
    #region 움직임,공격 관련

    public void PlayerMove(bool isMoveToAttack = false)
    {
        float moveSpeedPerSec = 1 / stat.moveSpeed;
        if (nodePreview.Count <= 0) return;
        if (CurrentNode == nodePreview.First())
        {
            nodePreview.RemoveFirst();
        }

        Vector3[] tempNodePosArray = new Vector3[nodePreview.Count];

        for (short i = 0; i < tempNodePosArray.Length; i++)
        {
            if (CurrentNode != nodePreview.First())
            {
                if (nodePreview.First().CharacterOnNode != null && stat != nodePreview.First().CharacterOnNode)
                {
                    Array.Resize(ref tempNodePosArray, i);
                    nodePreview.Clear();
                    break;
                }
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
        if (isMoveToAttack)
        {
            DOPath(transform, tempPath, tempNodePosArray.Length * moveSpeedPerSec).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (GridManager.GetInstance().MeleeAttackOrder(stat,stat.target))
                {
                    //현재 상태가 attackState가 아닐 경우
                    if (StateMachine.CurrentState != StateMachine.SearchState("attackState"))
                    {
                        stateMachine.SetDirrection(ref playerLookDir, stat.standingNode.nodeCenterPosition, stat.target.standingNode.nodeCenterPosition);
                        //attackState로 바꿉니다
                        StateMachine.ChangeState("attackState");
                    }
                }
            });
        }
        else
        {
            DOPath(transform, tempPath, tempNodePosArray.Length * moveSpeedPerSec).SetEase(Ease.Linear);
        }
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
            stateMachine.SetDirrection(ref playerLookDir,target.position,x);

            StateMachine.AnimationChange();
            target.position = x;
            CurrentNode = GridManager.GetInstance().PositionToNode(target.position);
        }, path, duration).SetTarget(target);
        tweenerCore.plugOptions.mode = pathMode;
        return tweenerCore;
    }
    #endregion
    #region 키 바인딩
    /// <summary>
    /// 마우스는 고정값을 사용
    /// </summary>
    public void MouseBinding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] groundHit = Physics.RaycastAll(ray, 1000f, 8);
        RaycastHit[] monsterHit = Physics.RaycastAll(ray, 1000f, 64);
        if (monsterHit.Length > 0)
        {
            playerCursorState.changeState(cursorState.attackAble);
            if (targetCell.gameObject.activeSelf) targetCell.gameObject.SetActive(false);
        }
        else
        {
            if(groundHit.Length > 0)
            {
                playerCursorState.changeState(cursorState.defaultCurser);
                Node tempNode = GridManager.GetInstance().PositionToNode(groundHit[0].point);
                if (tempNode != null)
                {
                    if(!targetCell.gameObject.activeSelf) targetCell.gameObject.SetActive(true);
                    targetCell.position = new Vector3(GridManager.GetInstance().PositionToNode(groundHit[0].point).nodeCenterPosition.x,
                    GridManager.GetInstance().PositionToNode(groundHit[0].point).nodeFloor,
                    GridManager.GetInstance().PositionToNode(groundHit[0].point).nodeCenterPosition.y);
                }

            }
            else
            {
                if (targetCell.gameObject.activeSelf) targetCell.gameObject.SetActive(false);
                playerCursorState.changeState(cursorState.noneClickAbleState);
            }
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (monsterHit.Length > 0)
            {
                if (SetTargetMonster(monsterHit[0].transform))
                {
                    if (GridManager.GetInstance().MeleeAttackOrder(stat, GridManager.GetInstance().PositionToNode(monsterHit[0].point)?.CharacterOnNode))
                    {
                        //현재 상태가 attackState가 아닐 경우
                        if (StateMachine.CurrentState != StateMachine.SearchState("attackState"))
                        {
                            //attackState로 바꿉니다
                            StateMachine.SetDirrection(ref playerLookDir, stat.standingNode.nodeCenterPosition, stat.target.standingNode.nodeCenterPosition);
                            StateMachine.ChangeState("attackState");
                        }
                    }
                }
            }
            else if (groundHit.Length > 0)
            {
                PlayerMoveOrder(groundHit[0].point);
            }

        }
    }



    public void PlayerMoveOrder(Vector3 targetPosition, bool isMoveToAttack = false)
    {
        if (SetTargetNode(targetPosition))
        {
            PlayerMove(isMoveToAttack);
            
            StateMachine.ChangeState("moveState");
        }

    }
    #endregion
    private void InstallizeStates()
    {
        Queue<PlayerStates> states = new Queue<PlayerStates>();
        states.Enqueue(new MoveState( 1, 1, "moveState", "idleState", false));
        states.Enqueue(new IdleState(1, 1, "idleState", "idleState", true));
        states.Enqueue(new AttackState(1, stat.attackSpeed, "attackState", "idleState", false,stat));
        stateMachine = new PlayerStateMachine(states.ToArray(),GetComponent<Animator>());
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
