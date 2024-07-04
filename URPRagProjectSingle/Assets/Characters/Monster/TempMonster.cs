using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerDefines;

public class TempMonster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]Stats monsterStat;
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
        monsterStat = new Stats(GridManager.GetInstance().PositionToNode(transform.position),30,3,3,10);
        transform.position = new Vector3(monsterStat.standingNode.nodeCenterPosition.x, monsterStat.standingNode.nodeFloor+1.5f, monsterStat.standingNode.nodeCenterPosition.y);
        Debug.Log(monsterStat.standingNode.nodeCenterPosition);
    }
}
