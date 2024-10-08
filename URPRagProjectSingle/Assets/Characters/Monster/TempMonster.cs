using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerDefines;
using PlayerDefines.Stat;
using Unity.VisualScripting;
using System.Linq;

public class TempMonster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]Stats monsterStat;
    LinkedList<Node> path = new LinkedList<Node>();
    Node targetNode
    {
        get { return Player.Instance.playerLevelInfo.stat.standingNode; }
    }
    public Node CurrentNode
    {
        get { return monsterStat.standingNode; }
        set
        {
            monsterStat.standingNode.CharacterOnNode = monsterStat.standingNode.CharacterOnNode == monsterStat ? null : monsterStat.standingNode.CharacterOnNode;
            monsterStat.standingNode = value;
            monsterStat.standingNode.CharacterOnNode = monsterStat;
        }
    }
    void Start()
    {
        monsterStat = new Stats(GridManager.GetInstance().PositionToNode(transform.position),30,10,3,3,10,1);
        transform.position = new Vector3(monsterStat.standingNode.nodeCenterPosition.x, monsterStat.standingNode.nodeFloor+1.5f, monsterStat.standingNode.nodeCenterPosition.y);
        Debug.Log(monsterStat.standingNode.nodeCenterPosition);
        path = GridManager.GetInstance().PathFinding(monsterStat.standingNode.nodeCenterPosition, targetNode.nodeCenterPosition);
    }
    private void Update()
    {
        if (path.Last<Node>() != targetNode)
        {
            Debug.Log("플레이어 탐색중");
            path = GridManager.GetInstance().PathFinding(monsterStat.standingNode.nodeCenterPosition, targetNode.nodeCenterPosition);
        }
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
}
