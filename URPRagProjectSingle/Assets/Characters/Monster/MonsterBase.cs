using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerDefines;
using PlayerDefines.Stat;
using System.Linq;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Plugins;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using PlayerDefines.States;

public class MonsterBase : MonoBehaviour, ICameraTracker
{
    // Start is called before the first frame update
    [System.Serializable]
    public class DropTable
    {
        public bool isWeaponDrop;
        public int weaponRate;
        public bool isArmorDrop;
        public int armorRate;
        public DropInfos[] dropInfos;
        [System.Serializable]
        public struct DropInfos
        {
            public SlotType slotType;
            public int code;
            public int dropRate;
        }
    }
    public DropTable dropItems;
    [SerializeField] public MonsterStat monsterStat;
    public Vector3 initialPos = new Vector3(-1000, -1000, -1000);
    Queue<Node> path = new Queue<Node>();
    [SerializeField] int recogDistance;
    public int RecogDistance { get { return recogDistance * 10; } }
    public float respawnSec = 0;
    public Node playerNode
    {
        get { return Player.Instance.playerLevelInfo.stat.standingNode; }
    }
    public bool isMonsterMoving = false;
    private Node lastNode;
    public Node blockingNode;
    private SpriteRenderer monsterSR;

    [SerializeField] public bool alreadyResearch;
    private float baseEXP = 1000;
    private float jobEXP = 50;
    public bool isFlip
    {
        set
        {
            monsterSR.flipX = value;
        }
    }
    public Node CurrentNode
    {
        get { return monsterStat.standingNode; }
        set
        {
            lastNode = monsterStat.standingNode;
            //������忡 �� ���Ͱ� �ڷ��� �� �ִٸ� null�� �ٲ���,�װ� �ƴϸ� ���� Stat�� ����\
            if (monsterStat.standingNode == null) return;
            monsterStat.standingNode.CharacterOnNode = monsterStat.standingNode.CharacterOnNode == monsterStat ? null : monsterStat.standingNode.CharacterOnNode;
            monsterStat.standingNode = value;
            if (monsterStat.standingNode == null) return;
            monsterStat.standingNode.CharacterOnNode = monsterStat;
        }
    }
    public IState currentStates;
    public bool isAgressiveMob = false;
    private Animator animator;
    public void Start()
    {
        if (initialPos == Vector3.one * (-1000)) initialPos = transform.position;

        if (monsterStat == null)
        {
            monsterStat = new MonsterStat(0, GridManager.GetInstance().PositionToNode(initialPos), 30, 10, 2, 3, 10, 1, 0);
        }
        else
        {
            monsterStat = new MonsterStat(monsterStat.monsterLevel
                , GridManager.GetInstance().PositionToNode(initialPos)
                , monsterStat.defaultMaxHP, monsterStat.defaultSP, monsterStat.MoveSpeed, monsterStat.attackSpeed, monsterStat.attackDamage,
                monsterStat.pureAttackRange, monsterStat.Evasion);
        }

        transform.position = CurrentNode.worldPos;
        //new Vector3(monsterStat.standingNode.nodeCenterPosition.x, monsterStat.standingNode.nodeFloor + 1.5f, monsterStat.standingNode.nodeCenterPosition.y);

        Debug.Log(monsterStat.standingNode.nodeCenterPosition);


        monsterSR = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        monsterStat.dieFunctions = null;
        monsterStat.dieFunctions += MonsterDie;
        currentStates = new MIdleState(this);
        RegistCameraAction();

    }
    private void Update()
    {

        if (monsterStat.isCharacterDamaged && !monsterStat.isCharacterDie)
        {
            monsterStat.HPBar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + monsterSR.bounds.size.y, transform.position.z));
        }
        else if (CurrentNode == null) { return; }
        currentStates?.Execute();



    }

    #region ī�޶� ����
    public void RegistCameraAction()
    {
        PlayerCam.Instance.rotDirrection += animationDirrection;
        PlayerCam.Instance.rotations += FollowCamera;
        animationDirrection();
    }
    public void UnRegistCameraAction()
    {
        PlayerCam.Instance.rotDirrection -= animationDirrection;
    }
    public void FollowCamera()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    #endregion

    public void ChangeAnim(string name)
    {
        animator.Play(name);
    }

    public void ChangeState(IState nextState) //�̻�� �Լ�, monster stateMachine �۾� �Ϸ�� �ʿ�
    {
        currentStates.Exit();
        currentStates = nextState;
        currentStates.Enter();

    }
    public bool IsInRange(Vector2Int startPos, Vector2Int endPos, int distance)
    {
        //GridManager�� GetDistance�� ���� ��, �����޸� ���� ����ȭ�� ���� ���� ������ �ۼ���
        // �� �� ������ x�� y ��ǥ ���� ���
        Vector2Int dist = new Vector2Int(Mathf.Abs(startPos.x - endPos.x), Mathf.Abs(startPos.y - endPos.y));

        //         10 * (dist.x + dist.y) + (14 - 2 * 10) * Mathf.Min(dist.x, dist.y) == �밢�� �Ÿ� ��� (�� �� ������ ���� ū ���� ��)
        if (10 * (dist.x + dist.y) + (14 - 2 * 10) * Mathf.Min(dist.x, dist.y) <= distance) return true;
        else return false;
    }
    private void OnDrawGizmos()
    {
        if (path == null) return;
        if (path.Count < 0) return;
        Node[] tempNodeArray = path.ToArray();
        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(tempNodeArray[i].nodeCenterPosition.x, transform.position.y, tempNodeArray[i].nodeCenterPosition.y), Vector3.one);
        }
    }
    public void MonsterDie()
    {
        ChangeState(new MDieState(this));
        Player.Instance.playerLevelInfo.GetBaseEXP(baseEXP);
        Player.Instance.playerLevelInfo.GetJobEXP(jobEXP);
        ItemDrop();

    }
    public bool RollDice(int chance)
    {
        int tempCode = UnityEngine.Random.Range(0, 101);
        Debug.Log($"�ֻ���{tempCode},�����{chance}");
        return tempCode <= chance;
    }
    public void ItemDrop()
    {
        if (dropItems.isWeaponDrop|| dropItems.isArmorDrop)
        {
            bool weaponDrop = RollDice(dropItems.weaponRate);
            bool armorDrop = RollDice(dropItems.armorRate);
            if ( (weaponDrop && armorDrop) && (dropItems.isWeaponDrop && dropItems.isArmorDrop) ) MonsterManager.GetInstance().Drop.SpawnEquipItem(transform.position, monsterStat.monsterLevel);
            else if (weaponDrop&& dropItems.isWeaponDrop) MonsterManager.GetInstance().Drop.SpawnEquipItem(transform.position, monsterStat.monsterLevel,true,false);
            else if(armorDrop&& dropItems.isArmorDrop) MonsterManager.GetInstance().Drop.SpawnEquipItem(transform.position, monsterStat.monsterLevel, false, true);
        }
        for (int i = 0; i < dropItems.dropInfos.Length;i++)
        {
            if (!RollDice(dropItems.dropInfos[i].dropRate)) continue;
            switch (dropItems.dropInfos[i].slotType)
            {
                case SlotType.ConsumableItem:
                    MonsterManager.GetInstance().Drop.SpawnCosume(transform.position, ResourceManager.GetInstance().PosionDatas.items[dropItems.dropInfos[i].code]);
                    break;
                case SlotType.MISC:
                    MonsterManager.GetInstance().Drop.SpawnMisc(transform.position, ResourceManager.GetInstance().MiscDatas.items[dropItems.dropInfos[i].code]);
                    break;
            }
        }
    }
    public bool MoveOrder()
    {
        LinkedList<Node> list = PathFinding(monsterStat.standingNode.nodeCenterPosition, playerNode.nodeCenterPosition);
        if (list.Count > 0)
        {
            if (list.First() == this.CurrentNode) list.RemoveFirst();
            if (list.Last() == this.playerNode) list.RemoveLast();


            path = new Queue<Node>(list);
            if (path.Count > 0) { StartCoroutine(Movement(path)); }
            return true;
        }
        return false;
    }
    public Vector2Int SetFlipDIr()
    {
        Vector2Int tempPos = CurrentNode.nodeCenterPosition - Player.Instance.CurrentNode.nodeCenterPosition;
        if (tempPos.x != 0)
        {
            if (tempPos.x > 0)
            {
                tempPos = Vector2Int.right;
            }
            else if (tempPos.x < 0)
            {
                tempPos = Vector2Int.left;
            }
        }
        else
        {
            if (tempPos.y > 0)
            {
                tempPos = Vector2Int.up;
            }
            else if (tempPos.y < 0)
            {
                tempPos = Vector2Int.down;
            }
        }
        return tempPos;
    }

    private void animationDirrection()
    {
        if(CurrentNode == null) return;
        Vector2Int tempVecInt = SetFlipDIr() - PlayerCam.Instance.CameraDirrection;
        //Debug.Log(tempVecInt);
        sbyte maxValue = (sbyte)Mathf.Max(tempVecInt.x, tempVecInt.y);
        sbyte minValue = (sbyte)Mathf.Min(tempVecInt.x, tempVecInt.y);
        if (maxValue == default(sbyte) && minValue == default(sbyte))
        {
            isFlip = false;
            return;
        }
        else if (maxValue == (sbyte)2 || minValue == (sbyte)-2)
        {
            isFlip = true;
        }
        else
        {
            if (PlayerCam.Instance.CameraDirrection.x == 0)
            {
                if ((maxValue < default(sbyte) && minValue < default(sbyte)) || (maxValue > default(sbyte) && minValue > default(sbyte)))
                {
                    isFlip = true;
                    return;
                }
                else if ((maxValue > default(sbyte) && minValue < default(sbyte)) || (maxValue < default(sbyte) && minValue < default(sbyte)))
                {
                    isFlip = false;
                    return;
                }
            }
            else
            {
                if ((maxValue < default(sbyte) && minValue < default(sbyte)) || (maxValue > default(sbyte) && minValue > default(sbyte)))
                {
                    isFlip = false;
                    return;
                }
                else if ((maxValue > default(sbyte) && minValue < default(sbyte)) || (maxValue < default(sbyte) && minValue < default(sbyte)))
                {
                    isFlip = true;
                    return;
                }
            }

        }
        isFlip = true;
        return;
    }

    public IEnumerator Movement(Queue<Node> nodeQueue)
    {
        isMonsterMoving = true;
        float secPerMove = 1f / monsterStat.MoveSpeed;
        Node nextNode;
        Node monsterOnNode;
        Node playerLastNode;
        Vector3 tempVec;
//        LinkedList<Node> list;
        Vector3 moveStartPos = Vector3.zero;
        while (nodeQueue.Count > 0)
        {
            float timer = 0;
            nextNode = nodeQueue.Dequeue();
            monsterOnNode = GridManager.GetInstance().PositionToNode(transform.position);
            playerLastNode = playerNode;
            moveStartPos = transform.position;
            moveStartPos.y = 0f;
            while (timer <= secPerMove)
            {
                timer += Time.deltaTime;

                monsterOnNode = GridManager.GetInstance().PositionToNode(transform.position);
                tempVec = (nextNode.worldPos - moveStartPos) * (timer /secPerMove);
                tempVec.y = nextNode.nodeFloor;
                transform.position = tempVec + moveStartPos;

                if ((monsterOnNode.CharacterOnNode != null && monsterOnNode.CharacterOnNode != this.monsterStat) || playerNode != playerLastNode || (nextNode.CharacterOnNode != null && nextNode.CharacterOnNode != this.monsterStat))
                {
                    /*                    list = GridManager.GetInstance().PathFinding(monsterStat.standingNode.nodeCenterPosition, playerNode.nodeCenterPosition);

                                        if (list.Count > 0)
                                        {
                                            if (list.First() == this.CurrentNode) list.RemoveFirst();
                                            if (list.Last() == this.playerNode) list.RemoveLast();
                                        }*/
                    //�� ������ ������
                    
                    if(nextNode.CharacterOnNode != null)blockingNode = nextNode;

                    //if (timer < secPerMove) continue;

                    //isMonsterMoving = false;
                    
                    StartCoroutine(CanceledMoveProccess(blockingNode != nextNode ? nextNode : CurrentNode));
                    //if (list.Count <= 0) isMonsterMoving = false;
                    //else StartCoroutine(Movement(list.Count >= 2 ? new Queue<Node>(list) : new Queue<Node>(new Node[1] { lastNode })));
                    yield break;

                }
                else
                {
                    blockingNode = null;
                    CurrentNode = GridManager.GetInstance().PositionToNode(transform.position);
                }

                yield return null;
            }
            yield return null;
    

        }
        alreadyResearch = false;
        isMonsterMoving = false;
    }
    public IEnumerator CanceledMoveProccess(Node nextNode) 
    {
        float timer = 0f;
        float secPerMove = 1f / monsterStat.MoveSpeed;
        Vector3 moveStartPos = transform.position;
        moveStartPos.y = 0f;
        Vector3 tempVec;
        Debug.LogError($"�̶� �����?");
        while (true)
        {
            timer += Time.deltaTime;
            tempVec = (nextNode.worldPos - moveStartPos) * (timer/ secPerMove);
            yield return null;
            tempVec.y = nextNode.nodeFloor;
            if (nextNode.CharacterOnNode != monsterStat&& nextNode.CharacterOnNode != null) 
            {
                isMonsterMoving = false;
                yield break;
            }
            CurrentNode = GridManager.GetInstance().PositionToNode(transform.position);
            transform.position = tempVec + moveStartPos;
            if (timer>= secPerMove)
            {
                isMonsterMoving = false;
                yield break;
            }
        }
    }
    public LinkedList<Node> PathFinding(Vector2Int startPos, Vector2Int endPos)
    {
        // ���� ��� ����Ʈ: Ž�� �Ϸ�� ������ ����
        LinkedList<Node> closedNodes = new LinkedList<Node>();
        Vector2Int[] endPosList = GridManager.GetInstance().NearPositions(endPos);
        int lowerstF = int.MaxValue;
        for (int i = 0; i < endPosList.Length; i++)
        {
            if (GridManager.GetInstance().grids[endPosList[i]].CharacterOnNode == this.monsterStat) return closedNodes;
            if(GridManager.GetInstance().grids[endPosList[i]].CharacterOnNode == null)
            {
                GridManager.GetInstance().grids[endPosList[i]].SetGH(startPos, endPosList[i]);
                if (GridManager.GetInstance().grids[endPosList[i]].F < lowerstF)
                {
                    lowerstF = GridManager.GetInstance().grids[endPosList[i]].F;
                    endPos = endPosList[i];
                }
            }
        }
        // ������(endPos)�� Grid�� ���� ��� �� ����Ʈ ��ȯ
        if (!GridManager.GetInstance().grids.ContainsKey(endPos))
        {
            closedNodes.Clear();
            return closedNodes;
        }

        // ������� �������� ������ ��� �� ����Ʈ ��ȯ
        if (startPos == endPos)
        {
            closedNodes.Clear();
            return closedNodes;
        }

        // ���� ��� ����Ʈ: Ž�� ���� ������ ����
        LinkedList<Node> openNodes = new LinkedList<Node>();

        // Ž�� ���� ���θ� �����ϴ� �÷���
        bool isSearchDone = false;

        // ���� ��带 ���� ����Ʈ�� �߰��ϰ� G, H �� ���
        closedNodes.AddLast(GridManager.GetInstance().grids[startPos]);
        GridManager.GetInstance().grids[startPos].SetGH(startPos, endPos);
        //endPos = GridManager.GetInstance().GetNearOpenNodes(ref endPos,);
        // ���� ���� ������ ���� ī���� (�ִ� 1000�� ����)
        short whileCounterMax = (short)1000;

        // ��� Ž�� ����
        while (!isSearchDone)
        {
            // ���� ��带 Ž���Ͽ� �ֺ� ���(������ Ÿ��)�� openNodes�� �߰�
            GridManager.GetInstance().GetNearOpenNodes(ref openNodes, closedNodes.Last().nodeCenterPosition, closedNodes);

            Vector2Int lowerstHNodePos = Vector2Int.zero; // ���� ���� H���� ���� ����� ��ǥ
            int lowerstHValue = int.MaxValue; // H�� �ʱ�ȭ

            // ���� ������ Ž���Ͽ� H���� ���� ���� ��带 ã��
            while (openNodes.Count > 0)
            {
                Node node = openNodes.First(); // ���� ��� �� ù ��° ��带 ������
                node.SetGH(startPos, endPos);  // G, H �� ���
                openNodes.RemoveFirst();       // �ش� ��带 ���� ����Ʈ���� ����

                // H���� ���� ����, �̵� ������ Ÿ���̸�, ĳ���Ͱ� ���ų� �������� ���
                if (lowerstHValue > node.H && node.isMoveableTile && (node.CharacterOnNode == null || endPos == node.nodeCenterPosition))
                {
                    lowerstHValue = node.H;
                    lowerstHNodePos = node.nodeCenterPosition;
                }

                // ���� ���� ��尡 ��������� Ž�� ����
                if (endPos == node.nodeCenterPosition)
                {
                    isSearchDone = true;
                    openNodes.Clear(); // Ž�� �Ϸ� �� ���� ��� ����Ʈ �ʱ�ȭ
                }
            }
            openNodes.Clear(); // ���� ��� �ʱ�ȭ

            // ���� ���� H ���� ���� ��带 ���� ��忡 �߰��ϰ�, ���� ���� ����
            GridManager.GetInstance().grids[lowerstHNodePos].connectedNode = closedNodes.Last();
            closedNodes.AddLast(GridManager.GetInstance().grids[lowerstHNodePos]);

            // ī���� ����, 1000�� �̻� �ݺ��Ǹ� Ž�� ����
            --whileCounterMax;
            if (whileCounterMax <= 0) isSearchDone = true;
        }

        // Ž�� ���� �� ��ΰ� ��ȿ���� �ʴٸ� �� ����Ʈ ��ȯ
        if (closedNodes.Last() != GridManager.GetInstance().grids[endPos] || closedNodes.First() != GridManager.GetInstance().grids[startPos])
        {
            return new LinkedList<Node>();
        }

        // ��ΰ� ����� �����Ǿ��� ���, ��θ� �����Ͽ� ��ȯ
        isSearchDone = false;
        whileCounterMax = (short)1000;
        LinkedList<Node> doubleCheckedNodes = new LinkedList<Node>();
        doubleCheckedNodes.AddFirst(closedNodes.Last());

        // ���� ��忡�� ���� ���� �ٽ� ��θ� ����
        while (!isSearchDone)
        {
            GridManager.GetInstance().GetNearClosedNodes(ref doubleCheckedNodes, doubleCheckedNodes.First().nodeCenterPosition, closedNodes);

            // ���� ��忡 �����ϰų� ī���Ͱ� 0 ���ϰ� �Ǹ� ����
            if (doubleCheckedNodes.First() == GridManager.GetInstance().grids[startPos] || whileCounterMax <= 0)
            {
                isSearchDone = true;
            }
            whileCounterMax--;
        }

        // ��ΰ� ��ȿ���� ���� ��� �� ����Ʈ ��ȯ
        if (doubleCheckedNodes.Last() != GridManager.GetInstance().grids[endPos] || doubleCheckedNodes.First() != GridManager.GetInstance().grids[startPos])
        {
            return new LinkedList<Node>();
        }

        // �ϼ��� ��� ����Ʈ ��ȯ
        return doubleCheckedNodes;
    }
    public bool Respawn()
    {
        Node respawnNode = GridManager.GetInstance().PositionToNode(initialPos);
        if(respawnNode == null) return false;
        if (respawnNode.CharacterOnNode != null) return false;
        monsterStat.HP = float.MaxValue; 
        gameObject.SetActive(true);

        monsterStat.standingNode = respawnNode;
        respawnNode.CharacterOnNode = monsterStat;
        CurrentNode = respawnNode;
        transform.position = respawnNode.worldPos;
        ChangeState(new MIdleState(this));
        return true;
    }
}
