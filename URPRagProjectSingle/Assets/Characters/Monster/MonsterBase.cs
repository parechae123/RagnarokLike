using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerDefines;
using PlayerDefines.Stat;
using Unity.VisualScripting;
using System.Linq;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Plugins;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using UnityEngine.UI;

public class MonsterBase : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MonsterStat monsterStat;
    

    public Vector3 initialPos = new Vector3(-1000,-1000,-1000);
    Queue<Node> path = new Queue<Node>();
    [SerializeField] int recogDistance;
    int RecogDistance { get { return recogDistance * 10; } }
    public float respawnSec = 0;
    Node playerNode
    {
        get { return Player.Instance.playerLevelInfo.stat.standingNode; }
    }
    [SerializeField] private bool isMonsterMoving = false;
    private Node lastNode;
    private Node blockingNode;
    private SpriteRenderer monsterSR;
    [SerializeField] private float searchTimer;
    [SerializeField] private float searchDelay;
    [SerializeField] private bool alreadyResearch;
    private float baseEXP = 1000;
    private float jobEXP = 50;
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

    public void Start()
    {
        if(initialPos == Vector3.one*(-1000)) initialPos = transform.position;

        monsterStat = new MonsterStat(GridManager.GetInstance().PositionToNode(initialPos), 30, 10, 1, 3, 10, 1,0);
        transform.position = new Vector3(monsterStat.standingNode.nodeCenterPosition.x, monsterStat.standingNode.nodeFloor + 1.5f, monsterStat.standingNode.nodeCenterPosition.y);
        Debug.Log(monsterStat.standingNode.nodeCenterPosition);

        monsterSR = GetComponent<SpriteRenderer>();
        monsterStat.dieFunctions = null;
        monsterStat.dieFunctions += MonsterDie;
    }
    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        if (IsInRange(monsterStat.standingNode.nodeCenterPosition, playerNode.nodeCenterPosition, RecogDistance))
        {
            //���ݷ���
            searchTimer += Time.deltaTime;
            monsterStat.statTimer += Time.deltaTime;
            if(monsterStat.attackSpeed <= monsterStat.statTimer)
            {
                if (IsInRange(monsterStat.standingNode.nodeCenterPosition,playerNode.nodeCenterPosition,monsterStat.CharactorAttackRange))
                {
                    if (Player.Instance.playerLevelInfo.stat.isCharacterDie) return;
                    monsterStat.AttackTarget(Player.Instance.playerLevelInfo.stat);
                    monsterStat.statTimer = 0;
                    return;
                }
            }
            //�̵�����
            if (searchTimer > searchDelay)
            {
                if (!isMonsterMoving)
                {
                    if (blockingNode != null)
                    {
                        
                        if (blockingNode.CharacterOnNode != null&& blockingNode.CharacterOnNode != monsterStat)
                        {
                            if(!alreadyResearch)
                            {
                                alreadyResearch = true;
                                MoveOrder();
                            }
                            return;
                        }
                    }
                    MoveOrder();

                }
            }
            if (searchTimer > searchDelay)
            {
                searchTimer = 0;
            }
        }
        if (monsterStat.isCharacterDamaged)
        {
            monsterStat.HPBar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + monsterSR.bounds.size.y, transform.position.z));
        }

    }
    private bool IsInRange(Vector2Int startPos, Vector2Int endPos, int distance)
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
        MonsterManager.GetInstance().AddRespawnList(this);
        Player.Instance.playerLevelInfo.GetBaseEXP(baseEXP);
        Player.Instance.playerLevelInfo.GetJobEXP(jobEXP);
        monsterStat.HPBar = null;
    }
    public void MoveOrder()
    {
        LinkedList<Node> list = PathFinding(monsterStat.standingNode.nodeCenterPosition, playerNode.nodeCenterPosition);
        if (list.Count > 0)
        {
            if (list.First() == this.CurrentNode) list.RemoveFirst();
            if (list.Last() == this.playerNode) list.RemoveLast();


            path = new Queue<Node>(list);
            if (path.Count > 0) { StartCoroutine(Movement(path)); }
        }
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
        Vector3 lastStandingPos;
        while (nodeQueue.Count > 0)
        {
            float timer = 0;
            nextNode = nodeQueue.Dequeue();
            monsterOnNode = GridManager.GetInstance().PositionToNode(transform.position);
            playerLastNode = playerNode;
            while (timer <= secPerMove)
            {
                timer += Time.deltaTime;
                monsterOnNode = GridManager.GetInstance().PositionToNode(transform.position);
                tempVec = (nextNode.worldPos - transform.position) * (secPerMove * timer);
                tempVec.y = nextNode.nodeFloor;
                transform.position = tempVec + transform.position;

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
                    isMonsterMoving = false;
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
            lastStandingPos = nextNode.worldPos;
            lastStandingPos.y += monsterSR.bounds.size.y;
            transform.position = lastStandingPos;
        }
        alreadyResearch = false;
        isMonsterMoving = false;
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
}
