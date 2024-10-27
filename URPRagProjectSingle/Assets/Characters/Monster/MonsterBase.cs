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
            //직전노드에 이 몬스터가 자료헹에 들어가 있다면 null로 바꿔줌,그게 아니면 원래 Stat을 유지\
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
            //공격로직
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
            //이동로직
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
        //GridManager의 GetDistance와 같은 식, 정적메모리 접근 과중화를 막기 위해 별도로 작성함
        // 두 점 사이의 x와 y 좌표 차이 계산
        Vector2Int dist = new Vector2Int(Mathf.Abs(startPos.x - endPos.x), Mathf.Abs(startPos.y - endPos.y));

        //         10 * (dist.x + dist.y) + (14 - 2 * 10) * Mathf.Min(dist.x, dist.y) == 대각선 거리 계산 (두 점 사이의 가장 큰 차이 값)
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
                    //위 과정이 끝나도
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
        // 닫힌 노드 리스트: 탐색 완료된 노드들을 저장
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
        // 도착지(endPos)가 Grid에 없을 경우 빈 리스트 반환
        if (!GridManager.GetInstance().grids.ContainsKey(endPos))
        {
            closedNodes.Clear();
            return closedNodes;
        }

        // 출발지와 도착지가 동일할 경우 빈 리스트 반환
        if (startPos == endPos)
        {
            closedNodes.Clear();
            return closedNodes;
        }

        // 열린 노드 리스트: 탐색 중인 노드들을 저장
        LinkedList<Node> openNodes = new LinkedList<Node>();

        // 탐색 종료 여부를 결정하는 플래그
        bool isSearchDone = false;

        // 시작 노드를 닫힌 리스트에 추가하고 G, H 값 계산
        closedNodes.AddLast(GridManager.GetInstance().grids[startPos]);
        GridManager.GetInstance().grids[startPos].SetGH(startPos, endPos);
        //endPos = GridManager.GetInstance().GetNearOpenNodes(ref endPos,);
        // 무한 루프 방지를 위한 카운터 (최대 1000번 루프)
        short whileCounterMax = (short)1000;

        // 경로 탐색 시작
        while (!isSearchDone)
        {
            // 열린 노드를 탐색하여 주변 노드(인접한 타일)를 openNodes에 추가
            GridManager.GetInstance().GetNearOpenNodes(ref openNodes, closedNodes.Last().nodeCenterPosition, closedNodes);

            Vector2Int lowerstHNodePos = Vector2Int.zero; // 가장 낮은 H값을 가진 노드의 좌표
            int lowerstHValue = int.MaxValue; // H값 초기화

            // 열린 노드들을 탐색하여 H값이 가장 낮은 노드를 찾음
            while (openNodes.Count > 0)
            {
                Node node = openNodes.First(); // 열린 노드 중 첫 번째 노드를 가져옴
                node.SetGH(startPos, endPos);  // G, H 값 계산
                openNodes.RemoveFirst();       // 해당 노드를 열린 리스트에서 제거

                // H값이 가장 낮고, 이동 가능한 타일이며, 캐릭터가 없거나 목적지인 경우
                if (lowerstHValue > node.H && node.isMoveableTile && (node.CharacterOnNode == null || endPos == node.nodeCenterPosition))
                {
                    lowerstHValue = node.H;
                    lowerstHNodePos = node.nodeCenterPosition;
                }

                // 만약 현재 노드가 도착지라면 탐색 종료
                if (endPos == node.nodeCenterPosition)
                {
                    isSearchDone = true;
                    openNodes.Clear(); // 탐색 완료 후 열린 노드 리스트 초기화
                }
            }
            openNodes.Clear(); // 열린 노드 초기화

            // 가장 낮은 H 값을 가진 노드를 닫힌 노드에 추가하고, 이전 노드와 연결
            GridManager.GetInstance().grids[lowerstHNodePos].connectedNode = closedNodes.Last();
            closedNodes.AddLast(GridManager.GetInstance().grids[lowerstHNodePos]);

            // 카운터 감소, 1000번 이상 반복되면 탐색 종료
            --whileCounterMax;
            if (whileCounterMax <= 0) isSearchDone = true;
        }

        // 탐색 종료 후 경로가 유효하지 않다면 빈 리스트 반환
        if (closedNodes.Last() != GridManager.GetInstance().grids[endPos] || closedNodes.First() != GridManager.GetInstance().grids[startPos])
        {
            return new LinkedList<Node>();
        }

        // 경로가 제대로 생성되었을 경우, 경로를 추적하여 반환
        isSearchDone = false;
        whileCounterMax = (short)1000;
        LinkedList<Node> doubleCheckedNodes = new LinkedList<Node>();
        doubleCheckedNodes.AddFirst(closedNodes.Last());

        // 닫힌 노드에서 시작 노드로 다시 경로를 추적
        while (!isSearchDone)
        {
            GridManager.GetInstance().GetNearClosedNodes(ref doubleCheckedNodes, doubleCheckedNodes.First().nodeCenterPosition, closedNodes);

            // 시작 노드에 도착하거나 카운터가 0 이하가 되면 종료
            if (doubleCheckedNodes.First() == GridManager.GetInstance().grids[startPos] || whileCounterMax <= 0)
            {
                isSearchDone = true;
            }
            whileCounterMax--;
        }

        // 경로가 유효하지 않을 경우 빈 리스트 반환
        if (doubleCheckedNodes.Last() != GridManager.GetInstance().grids[endPos] || doubleCheckedNodes.First() != GridManager.GetInstance().grids[startPos])
        {
            return new LinkedList<Node>();
        }

        // 완성된 경로 리스트 반환
        return doubleCheckedNodes;
    }
}
