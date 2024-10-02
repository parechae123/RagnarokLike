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
using UnityEngine.EventSystems;
using Unity.VisualScripting;


public class Player : MonoBehaviour
{
    public PlayerLevelInfo playerLevelInfo;
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
    //�̵� ���� �����ð�
    #region ��ų���� ���� ��ü
    [SerializeField]private SkillInfoInGame skillObj;
    public SkillInfoInGame SkillObj
    {
        get { return skillObj; }
        set 
        {
            skillObj = value;
            if (value == null)
            {
                isSearchCastTarget = false;
                playerCursorState.changeState(cursorState.defaultCurser);
                return;
            }

            if(value.skillPosition == SkillPosition.self)
            {
                StateMachine.ChangeState(SkillObj.skill[SkillObj.CastingSkillLevel].defaultCastingTime,
                    SkillObj,
                    null,
                    playerLevelInfo.stat.standingNode.worldPos);
            }
            else if(value.skillPosition == SkillPosition.cursor)
            {
                playerCursorState.changeState(cursorState.skillTargeting);
                isSearchCastTarget = true;
            }
        }
    }
    private bool isSearchCastTarget = false;
    #endregion
    #region ���
    [SerializeField] public Node targetNode;
    [SerializeField] private Node currentNode;
    [SerializeField] public Node CurrentNode
    {
        get { return currentNode; }
        set 
        {
            playerLevelInfo.stat.standingNode.CharacterOnNode = playerLevelInfo.stat.standingNode.CharacterOnNode == playerLevelInfo.stat ? null : playerLevelInfo.stat.standingNode.CharacterOnNode;
            playerLevelInfo.stat.standingNode = value;
            playerLevelInfo.stat.standingNode.CharacterOnNode = playerLevelInfo.stat;
            currentNode = value;
        }
    }
    public LinkedList<Node> nodePreview = new LinkedList<Node>();
    public float arriveTime;
    #endregion
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

        //��Ÿ�� �κ� �����ʿ�
    }
    private void Start()
    {
        if (playerLevelInfo.stat == null) playerLevelInfo.stat = new PlayerStat(currentNode, 100, 3, 1, 10,1);
        playerLevelInfo.baseLevelUP += playerLevelInfo.BaseLevelUP;
        playerLevelInfo.jobLevelUP += playerLevelInfo.JobLevelUP;
        InstallizeStates();
        SetCurrentNodeAndPosition();
        playerLevelInfo.stat.moveFunction += PlayerMoveOrder;
        playerCursorState.changeState(cursorState.defaultCurser);
    }
    public void Update()
    {
        MouseBinding();
        //����׿� Ű�� ������
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            playerLevelInfo.GetJobEXP(100);
        }
        StateMachine.CurrentState.Execute();

    }
    public bool SetTargetNode(Vector3 point)
    {
        Node tempNode = GridManager.GetInstance().PositionToNode(point);

        if (tempNode != null)
        {
            if (targetNode == tempNode) return false;        //������ ���ߴ� ������ ���� ���� ������ ����Ͻ� �۵� ����
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
        CurrentNode = GridManager.GetInstance().PositionToNode(transform.position);
    }
    /// <summary>
    /// �÷��̾��� ��ġ�� ����� �߾����� �ٲ��ִ� �Լ�
    /// </summary>
    private void SetPlayerPositionToCenterPos()
    {
        transform.position = new Vector3(CurrentNode.nodeCenterPosition.x, CurrentNode.nodeFloor + (playerSR.bounds.size.y) + 0.5f, CurrentNode.nodeCenterPosition.y);
    }
    #endregion
    #region ������,���� ����

    public void PlayerMove(bool isMoveToAttack = false)
    {
        float moveSpeedPerSec = 1 / playerLevelInfo.stat.moveSpeed;
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
                if (nodePreview.First().CharacterOnNode != null && playerLevelInfo.stat != nodePreview.First().CharacterOnNode)
                {
                    Array.Resize(ref tempNodePosArray, i);
                    nodePreview.Clear();
                    break;
                }
                tempNodePosArray[i] = nodePreview.First().worldPos;
                tempNodePosArray[i].y += Player.instance.playerSR.bounds.size.y;
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
                if (GridManager.GetInstance().AttackOrder(playerLevelInfo.stat, playerLevelInfo.stat.target,playerLevelInfo.stat.charactorAttackRange))
                {
                    //���� ���°� attackState�� �ƴ� ���
                    if (StateMachine.CurrentState != StateMachine.SearchState("attackState"))
                    {
                        StateMachine.SetDirrection(ref playerLookDir, playerLevelInfo.stat.standingNode.nodeCenterPosition, playerLevelInfo.stat.target.standingNode.nodeCenterPosition);
                        //attackState�� �ٲߴϴ�
                        StateMachine.ChangeState("attackState");
                    }
                }
            }).SetEase(Ease.Linear);
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
    public void PlayerMove(SkillInfoInGame skill, byte arriveDistanceFromTargetPos = 0)
    {
        float moveSpeedPerSec = 1 / playerLevelInfo.stat.moveSpeed;
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
                if (arriveDistanceFromTargetPos != 0)
                {
                    if (arriveDistanceFromTargetPos >= nodePreview.First().H)
                    {
                        Array.Resize(ref tempNodePosArray, i);
                        nodePreview.Clear();
                        break;
                    }
                }
                if (nodePreview.First().CharacterOnNode != null && playerLevelInfo.stat != nodePreview.First().CharacterOnNode)
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
        DOPath(transform, tempPath, tempNodePosArray.Length * moveSpeedPerSec).SetEase(Ease.Linear).OnComplete(() =>
        {
            //���� ���°� CastingState�� �ƴ� ���
            if (StateMachine.CurrentState != StateMachine.SearchState("castingState"))
            {
                stateMachine.SetDirrection(ref playerLookDir, playerLevelInfo.stat.standingNode.nodeCenterPosition, playerLevelInfo.stat.target.standingNode.nodeCenterPosition);
                //CastingState�� �ٲߴϴ�
                StateMachine.ChangeState(SkillObj.skill[SkillObj.CastingSkillLevel].defaultCastingTime,SkillObj, playerLevelInfo.stat.target, playerLevelInfo.stat.target.standingNode.worldPos);
                SkillObj = null;
            }
        }).SetEase(Ease.Linear);
        arriveTime = tempNodePosArray.Length * moveSpeedPerSec;
        return;



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
    #region Ű ���ε�
    /// <summary>
    /// ���콺�� �������� ���
    /// </summary>
    public void MouseBinding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] groundHit = Physics.RaycastAll(ray, 1000f, 8);
        RaycastHit[] monsterHit = Physics.RaycastAll(ray, 1000f, 64);

        if (isSearchCastTarget)
        {
            if (targetCell.gameObject.activeSelf) targetCell.gameObject.SetActive(false);
            if ( groundHit.Length == 0) return;
            if(monsterHit.Length> 0)
            {
                GetCastingTarget(GridManager.GetInstance().PositionToNode(monsterHit[0].point).nodeCenterPosition);
            }
            else
            {
                Node tempNode = GridManager.GetInstance().PositionToNode(groundHit[0].point);
                if (tempNode == null) return;
                GetCastingTarget(tempNode.nodeCenterPosition);
            }
            
            return;
        }


        if (EventSystem.current.IsPointerOverGameObject())
        {
            playerCursorState.changeState(cursorState.defaultCurser);
            return;
        }
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (monsterHit.Length > 0)
            {
                if (SetTargetMonster(monsterHit[0].transform))
                {
                    if (GridManager.GetInstance().AttackOrder(playerLevelInfo.stat
                        , GridManager.GetInstance().PositionToNode(monsterHit[0].point)?.CharacterOnNode
                        ,playerLevelInfo.stat.charactorAttackRange
                        ))
                    {
                        //���� ���°� attackState�� �ƴ� ���
                        if (StateMachine.CurrentState != StateMachine.SearchState("attackState"))
                        {
                            //attackState�� �ٲߴϴ�
                            StateMachine.SetDirrection(ref playerLookDir, playerLevelInfo.stat.standingNode.nodeCenterPosition, playerLevelInfo.stat.target.standingNode.nodeCenterPosition);
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
    public void GetCastingTarget(Vector2Int pos)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SkillObj = null;
                return;
            }

            if (isSearchCastTarget)
            {
                if (SkillObj.objectiveType == ObjectiveType.OnlyTarget)
                {
                    if (GridManager.GetInstance().grids[pos].CharacterOnNode == null)
                    {
                        SkillObj = null;
                        return;
                    }
                    //����� ���� ��
                    else
                    {
                        //������ �������� ���� �Ǻ�, ���� ���� ��
                        playerLevelInfo.stat.target = GridManager.GetInstance().grids[pos].CharacterOnNode;
                        if (GridManager.GetInstance().IsInRange(playerLevelInfo.stat, playerLevelInfo.stat.target, playerLevelInfo.stat.charactorAttackRange))
                        {
                            playerCursorState.changeState(cursorState.skillTargeting);
                            StateMachine.ChangeState(SkillObj.skill[SkillObj.CastingSkillLevel].defaultCastingTime,
                                SkillObj,
                                playerLevelInfo.stat.target,
                                playerLevelInfo.stat.target.standingNode.worldPos);
                        }
                        //���� �Ұ� ��
                        else
                        {
                            //�����ؼ� ����
                            if (SetTargetNode(GridManager.GetInstance().grids[pos].worldPos))
                            {
                                StateMachine.ChangeState("moveState");
                                PlayerMove(SkillObj, SkillObj.skill[SkillObj.CastingSkillLevel].skillRange);

                            }

                        }
                    }
                }
                else if (SkillObj.objectiveType == ObjectiveType.Bounded)
                {
                    playerCursorState.changeState(cursorState.skillTargeting);
                    StateMachine.ChangeState(SkillObj.skill[SkillObj.CastingSkillLevel].defaultCastingTime,
                        SkillObj,
                        playerLevelInfo.stat.target,
                        playerLevelInfo.stat.target.standingNode.worldPos);
                }
            }
            isSearchCastTarget = false;
            playerCursorState.changeState(cursorState.defaultCurser);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse2)) SkillObj = null;
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
        states.Enqueue(new AttackState(1, playerLevelInfo.stat.attackSpeed, "attackState", "idleState", false, playerLevelInfo.stat));
        states.Enqueue(new CastingState(1, playerLevelInfo.stat.attackSpeed, "castingState", "idleState", false));
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
[System.Serializable]
public class PlayerLevelInfo
{
    public PlayerStat stat;                                                 //�÷��̾� ����
    public PlayerSkillTreeForPhase[] playerOwnSkills = new PlayerSkillTreeForPhase[0];     //�÷��̾ ������ �ִ� ��ų ����Ʈ
    #region ���̽� ���� ���� ����
    public Action baseLevelUP, jobLevelUP;
    public byte baseLevel;
    private byte maxBaseLevel = 99;
    private float MaxBaseExp
    {
        get
        {
            return 342 * ((float)baseLevel * 1.6f);
        }
    }
    private float currBaseExp;
    private float CurrBaseExp
    {
        get { return currBaseExp; }
        set 
        {
            if (maxBaseLevel <= baseLevel)
            {
                currBaseExp = 0;
                return;
            }
            while (value > maxBaseLevel)
            {
                value = value - MaxBaseExp;
                currBaseExp = 0;
                Debug.Log("���� �� �ƽ�����ġ" + MaxBaseExp);
                Debug.Log("���� ����ġ" + (currBaseExp + value));
                if (value > 0) baseLevelUP.Invoke();
                else break;

            }
            currBaseExp = value;
        }
    }
    short usedStatusPoint =0;
    private short statutsPoint;
    public short LeftStatusPoint
    {
        get
        {
            return (short)(statutsPoint - usedStatusPoint);
        }
    }
    #endregion


    #region �ⷹ�� ���� ����
    public byte jobLevel;
    private byte maxJobLevel = 50;
    private float MaxJobExp
    {
        get
        {
            return 10 * (jobLevel * 1.6f);
        }
    }

    [SerializeField]private float currJobExp;
    private float CurrJobExp
    {
        get { return currJobExp; }
        set
        {
            if (maxJobLevel <= jobLevel)
            {
                currJobExp = 0;
                return;
            }
            while (value > MaxJobExp)
            {
                value = value-MaxJobExp;
                currJobExp = 0;
                if (value > 0) jobLevelUP.Invoke();
                else break;

            }
            currJobExp = value;
        }
    }

    public byte usedSkillPoint;
    public byte skillPoint;
    public byte LeftSkillPoint
    {
        get { return (byte)(skillPoint - usedSkillPoint); }
    }


    #endregion



    #region BaseLevel���� �Լ�
    public void BaseLevelUP()
    {
        statutsPoint += (short)(3 + (baseLevel / 5));
        baseLevel += 1;
    }
    public void GetBaseEXP(float exp)
    {
        CurrBaseExp += exp;
    }
    #endregion
    #region JobLevel���� �Լ�
    public bool LearnSkill(SkillIconsInSkilltree skill,int targetSkillIndex,SkillInfoInGame skillInfo,int classPhase)
    {
        if (LeftSkillPoint <= 0) return false;
        if (playerOwnSkills.Length < classPhase + 1) Array.Resize(ref playerOwnSkills, classPhase + 1);

        if (playerOwnSkills[classPhase] == null) playerOwnSkills[classPhase] = new PlayerSkillTreeForPhase();
        bool[] isLeanAble = isLearnAble(targetSkillIndex,classPhase, skill);
        //���ེų üũ
        for (int i = 0; i < isLeanAble.Length; i++)
        {
            if (isLeanAble[i] == false)
            {
                return false;
            }
        }

        if (playerOwnSkills[classPhase] != null)
        {
            foreach (SkillInfoInGame item in playerOwnSkills[classPhase].playerOwnSkills)
            {
                if (item.skillName == skill[targetSkillIndex].thisSkill.skillName)
                {
                    if (item.nowSkillLevel < item.maxSkillLevel)
                    {
                        item.nowSkillLevel++;
                        usedSkillPoint++;
                        return true;
                    }
                    return false;
                }
            }
        }
        if(playerOwnSkills[classPhase].playerOwnSkills.Length< targetSkillIndex + 1) Array.Resize(ref playerOwnSkills[classPhase].playerOwnSkills, targetSkillIndex + 1);
        playerOwnSkills[classPhase].playerOwnSkills[targetSkillIndex] = skillInfo;
        playerOwnSkills[classPhase].playerOwnSkills[targetSkillIndex].nowSkillLevel = 1;
        usedSkillPoint++;
        return true;
    }
    public void JobLevelUP()
    {
        skillPoint += 1;
        jobLevel += 1;
    }
    public void GetJobEXP(float exp)
    {
        CurrJobExp += exp;
    }
    #endregion
    /// <summary>
    /// ��ų�� ��� �� �ִ��� ���θ� ��ȯ
    /// </summary>
    /// <param name="targetLearnSkillIndex"></param>
    /// <returns></returns>
    public bool[] isLearnAble(int targetLearnSkillIndex,int classPhase, SkillIconsInSkilltree skillTree)
    {
        bool[] tempBool = new bool[skillTree[targetLearnSkillIndex].skillGetConditions.Length];
        (string,byte,bool)[] conditionSkillNames = new (string, byte,bool)[skillTree[targetLearnSkillIndex].skillGetConditions.Length];
        
        if (skillTree[targetLearnSkillIndex].skillGetConditions.Length <= 0) return tempBool;
        else
        {
            for (int i = 0; i < skillTree[targetLearnSkillIndex].skillGetConditions.Length; i++)
            {
                conditionSkillNames[i].Item1 = skillTree[skillTree[targetLearnSkillIndex].skillGetConditions[i].targetIndex].thisSkill.skillName;
                conditionSkillNames[i].Item2 = skillTree[targetLearnSkillIndex].skillGetConditions[i].targetLevel;
                conditionSkillNames[i].Item3 = false;
            }
            if(playerOwnSkills.Length > 0)
            {
                foreach (SkillInfoInGame item in playerOwnSkills[classPhase].playerOwnSkills)
                {
                    for (int i = 0; i < conditionSkillNames.Length; i++)
                    {
                        if (conditionSkillNames[i].Item1 == item.skillName)
                        {
                            if (conditionSkillNames[i].Item2 <= item.nowSkillLevel)
                            {
                                conditionSkillNames[i].Item3 = true;
                            }
                        }
                    }
                }
            }
            else
            {
                return new bool[1] { false };
            }

        }
        for (int i = 0; i < conditionSkillNames.Length; i++)
        {
            tempBool[i] = conditionSkillNames[i].Item3;
        }
        return tempBool;
    }
}
[System.Serializable]
public class PlayerSkillTreeForPhase
{
    public SkillInfoInGame[] playerOwnSkills = new SkillInfoInGame[0];
}