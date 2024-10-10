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

public class TempMonster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]Stats monsterStat;
    Queue<Node> path = new Queue<Node>();
    [SerializeField] int recogDistance;
    int RecogDistance { get { return recogDistance * 10; } }
    Node playerNode
    {
        get { return Player.Instance.playerLevelInfo.stat.standingNode; }
    }
    private bool isMonsterMoving = false;
    private Node lastNode;
    private SpriteRenderer monsterSR;
    public Node CurrentNode
    {
        get { return monsterStat.standingNode; }
        set
        {
            lastNode = monsterStat.standingNode;
            monsterStat.standingNode.CharacterOnNode = monsterStat.standingNode.CharacterOnNode == monsterStat ? null : monsterStat.standingNode.CharacterOnNode;
            monsterStat.standingNode = value;
            monsterStat.standingNode.CharacterOnNode = monsterStat;
        }
    }

    void Start()
    {
        monsterStat = new Stats(GridManager.GetInstance().PositionToNode(transform.position),30,10,1,3,10,1);
        transform.position = new Vector3(monsterStat.standingNode.nodeCenterPosition.x, monsterStat.standingNode.nodeFloor+1.5f, monsterStat.standingNode.nodeCenterPosition.y);
        Debug.Log(monsterStat.standingNode.nodeCenterPosition);
        monsterSR = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (IsInRange(monsterStat.standingNode.nodeCenterPosition, playerNode.nodeCenterPosition, RecogDistance))
        {
            LinkedList<Node> list = GridManager.GetInstance().PathFinding(monsterStat.standingNode.nodeCenterPosition, playerNode.nodeCenterPosition);
            
            if (list.Count > 0)
            {
                if(list.First() == this.CurrentNode) list.RemoveFirst();
                if(list.Last() == this.playerNode) list.RemoveLast();


                if (!isMonsterMoving)
                {
                    path = new Queue<Node>(list);
                    if(path.Count > 0) { StartCoroutine(Movement(path)); }
                }
            }
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
    public IEnumerator Movement(Queue<Node> nodeQueue)
    {
        isMonsterMoving = true;
        float secPerMove = 1f / monsterStat.MoveSpeed;
        while (nodeQueue.Count > 0)
        {
            float timer = 0;
            Node nextNode = nodeQueue.Dequeue();
            Node monsterOnNode = GridManager.GetInstance().PositionToNode(transform.position);
            Node playerLastNode = playerNode;
            while (timer <= secPerMove)
            {
                timer += Time.deltaTime;
                monsterOnNode = GridManager.GetInstance().PositionToNode(transform.position);
                Vector3 tempVec = (nextNode.worldPos - transform.position) * (secPerMove * timer);
                tempVec.y = nextNode.nodeFloor;
                transform.position = tempVec + transform.position;

                if ((monsterOnNode.CharacterOnNode != null && monsterOnNode.CharacterOnNode != this.monsterStat) || playerNode != playerLastNode||(nextNode.CharacterOnNode != null&& nextNode.CharacterOnNode != this.monsterStat))
                {
                    LinkedList<Node> list = GridManager.GetInstance().PathFinding(monsterStat.standingNode.nodeCenterPosition, playerNode.nodeCenterPosition);

                    if (list.Count > 0)
                    {
                        if (list.First() == this.CurrentNode) list.RemoveFirst();
                        if (list.Last() == this.playerNode) list.RemoveLast();
                    }
                    //위 과정이 끝나도
                    if (list.Count > 0) StartCoroutine(Movement(list.Count >= 2 ? new Queue<Node>(list) : new Queue<Node>(new Node[1] { lastNode })));
                    else isMonsterMoving = false;
                    yield break;

                }
                else
                {
                    CurrentNode = GridManager.GetInstance().PositionToNode(transform.position);
                }

                yield return null;
            }
            Vector3 lastStandingPos = nextNode.worldPos;
            lastStandingPos.y += monsterSR.bounds.size.y;
            transform.position = lastStandingPos;
        }
        isMonsterMoving = false;
    }
}
