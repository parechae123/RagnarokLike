using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerDefines.Stat;
using PlayerDefines;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// 메니저 템플릿화
/// </summary>
/// <typeparam name="T">해당 메니저 클래스명</typeparam>
public class Manager<T> where T : new()
{
    protected static T instance;
    /// <summary>
    /// 인스턴스를 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public static T GetInstance()
    {
        if (instance == null)
        {
            instance = new T();
        }
        return instance;
    }
    /// <summary>
    /// 매니저를 기본값으로 설정하여 설정을 해제합니다.
    /// </summary>
    public static void Release()
    {

        instance = default(T);
    }
}
[System.Serializable]
public class Node
{
    public Vector2Int nodeCenterPosition;       //노드의 중앙 좌표값
    public bool isMoveableTile;                 //이동가능 타일인지
    public sbyte nodeFloor;                     //노드의 층(1층 = y1,-1층 = y-1
    private Stats characterOnNode;
    public Stats CharacterOnNode
    {
        get { return characterOnNode; }
        set 
        { 
            characterOnNode = value;
        }
    }
    //예약된 길인지
    public Vector3 worldPos
    {
        get {return new Vector3(nodeCenterPosition.x,((float)nodeFloor),nodeCenterPosition.y); }
    }
    public bool isEmptyNode(Stats stat)
    {
        if (characterOnNode == null)
        {
            return true;
        }
        return false;
    }
    public Node(sbyte floor, Vector2Int vec, bool isMoveable)
    {
        nodeFloor = floor;
        nodeCenterPosition = vec;
        isMoveableTile = isMoveable;
    }
    public Node connectedNode;
    //현재 노드로부터 타겟노드까지의 거리
    public int H { get; private set; }
    //현재 노드로부터 시작노드까지의 거리
    public int G { get; private set; }
    //두 거리를 합친 값
    /// <summary>
    /// 토탈코스트
    /// </summary>
    public int F { get { return H + G; } }

    public void SetGH(Vector2Int startPos, Vector2Int endPos)
    {
        G = GetDistance(startPos);
        H = GetDistance(endPos);
    }


    private int GetDistance(Vector2Int targetPos)
    {
        // 두 점 사이의 x와 y 좌표 차이 계산
        Vector2Int dist = new Vector2Int(Mathf.Abs(nodeCenterPosition.x - targetPos.x), Mathf.Abs(nodeCenterPosition.y - targetPos.y));

        // 대각선 거리 계산 (두 점 사이의 가장 큰 차이 값)
        return 10 * (dist.x + dist.y) + (14 - 2 * 10) * Mathf.Min(dist.x, dist.y);
        /*        // 두 점 사이의 x와 y 좌표 차이 계산
                Vector2Int dist = new Vector2Int(Mathf.Abs(nodeCenterPosition.x - targetPos.x), Mathf.Abs(nodeCenterPosition.y - targetPos.y));

                // 맨해튼 거리 계산 (두 점 사이의 수평 및 수직 거리 합)
                return (dist.x + dist.y) * 10;*/
    }



}
public class KeyMapManager : Manager<KeyMapManager>
{
    public Dictionary<KeyCode,ShortCutOBJ> keyMaps = new Dictionary<KeyCode,ShortCutOBJ>();
    public KeyCode combKey;
    //해당하는 애들만 받도록 Player에서 해주면 될듯
    public KeyCode[] ConvertKeyArray()
    {
        return keyMaps.Keys.ToArray<KeyCode>();
    }
    public ShortCutOBJ[] ConvertValueArray()
    {
        return keyMaps.Values.ToArray<ShortCutOBJ>();
    }

    public string ExportKeyMapJson()
    {
        string path = Path.Combine(Application.persistentDataPath, "KeySetting.txt");
        if (File.Exists(path))
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(path,FileMode.Open,FileAccess.Write,FileShare.Read)))
            {
                writer.WriteLine(JsonConvert.SerializeObject(combKey));
                writer.WriteLine(JsonConvert.SerializeObject(keyMaps));
                writer.Flush();
                writer.Close();
            }
        }
        else
        {
            StreamWriter SW = new StreamWriter(path);
            //첫번째 줄은 조합키만
            SW.WriteLine(JsonConvert.SerializeObject(combKey));
            //두번째줄은 조합키 제외 모든 키
            SW.WriteLine(JsonConvert.SerializeObject(keyMaps));
            SW.Flush();
            SW.Close();
        }
        return Application.persistentDataPath+"폴더에"+"KeySetting.txt 파일로 저장되었습니다.";
    }
    public bool ImportkeyMapJson()
    {
        string path = Path.Combine(Application.persistentDataPath, "KeySetting.txt");
        if (!File.Exists(path)) return false;
        StreamReader target = new StreamReader(Path.Combine(Application.persistentDataPath, "KeySetting.txt"));

        combKey = JsonConvert.DeserializeObject<KeyCode>(target.ReadLine());
        keyMaps = JsonConvert.DeserializeObject<Dictionary<KeyCode, ShortCutOBJ>>(target.ReadLine().ToString());

        target.Close();
        if (keyMaps.Count <= 0)
        {
            Debug.Log("오브젝트 임포트 실패");
            return false;
        }
        else return true;
    }
}
public class GridManager : Manager<GridManager>
{
    public Dictionary<Vector2Int, Node> grids = new Dictionary<Vector2Int, Node>();
    /// <summary>
    /// vector3를 vector2int로 형변환(반올림)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Node PositionToNode(Vector3 position)
    {
        Vector2Int tempIntPos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        return grids.ContainsKey(tempIntPos) ? grids[new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z))] : null;
    }

    /// <summary>
    /// 코스트와 휴리스틱을 비교하여 최적의 경로를 찾음
    /// </summary>
    /// <param name="startPos">시작노드 위치</param>
    /// <param name="endPos">이동목표 노드 위치</param>
    /// <returns></returns>
    /// 
    public LinkedList<Node> PathFinding(Vector2Int startPos, Vector2Int endPos)
    {
        //열린노드 : 길이 될 수 있는(닫힌노드의 주변)
        //닫힌노드 : 열린 노드 중 가장 낮은 f비용의 노드,닫힌노드로 포함 될 시 열린노드에서 제함
        LinkedList<Node> closedNodes = new LinkedList<Node>();
        if (!grids.ContainsKey(endPos))
        {
            closedNodes.Clear();
            return closedNodes;
        }

        if (startPos == endPos) 
        {
            closedNodes.Clear();
            return closedNodes;
        } 

        LinkedList<Node> openNodes = new LinkedList<Node>();
        bool isSearchDone = false;


        closedNodes.AddLast(grids[startPos]);
        grids[startPos].SetGH(startPos, endPos);
        short whileCounterMax = (short)1000;
        while (!isSearchDone)
        {
            GetNearOpenNodes(ref openNodes, closedNodes.Last().nodeCenterPosition, closedNodes);
            Vector2Int lowerstHNodePos = Vector2Int.zero;
            int lowerstHValue = int.MaxValue;
            while (openNodes.Count > 0)
            {
                Node node = openNodes.First();
                node.SetGH(startPos, endPos);
                openNodes.RemoveFirst();
                if (lowerstHValue > node.H && node.isMoveableTile&&(node.CharacterOnNode == null|| endPos == node.nodeCenterPosition))
                {
                    lowerstHValue = node.H;
                    lowerstHNodePos = node.nodeCenterPosition;
                }


                if (endPos == node.nodeCenterPosition)
                {
                    isSearchDone = true;
                    openNodes.Clear();
                }
            }
            openNodes.Clear();
            grids[lowerstHNodePos].connectedNode = closedNodes.Last();
            
            closedNodes.AddLast(grids[lowerstHNodePos]);
            --whileCounterMax;
            if (whileCounterMax <= 0) isSearchDone = true;
        }
        if (closedNodes.Last() != grids[endPos] || closedNodes.First() != grids[startPos])
        {
            return new LinkedList<Node>();
        }

        isSearchDone = false;
        whileCounterMax = (short)1000;
        LinkedList<Node> doubleCheckedNodes = new LinkedList<Node>();
        doubleCheckedNodes.AddFirst(closedNodes.Last());
        while (!isSearchDone)
        {
            GetNearClosedNodes(ref doubleCheckedNodes, doubleCheckedNodes.First().nodeCenterPosition, closedNodes);
            if (doubleCheckedNodes.First() == grids[startPos] || whileCounterMax <=0)
            {
                isSearchDone =true;
            }
            whileCounterMax--;
        }
        if (doubleCheckedNodes.Last() != grids[endPos]|| doubleCheckedNodes.First() != grids[startPos])
        {
            return new LinkedList<Node>();
        }
        return doubleCheckedNodes;

    }

    /*    public Node[] PathFinding(Vector2Int startPos, Vector2Int endPos)
        {
            //열린노드 : 길이 될 수 있는(닫힌노드의 주변)
            //닫힌노드 : 열린 노드 중 가장 낮은 f비용의 노드,닫힌노드로 포함 될 시 열린노드에서 제함
            if (!grids.ContainsKey(endPos)) return null;
            LinkedList<Node> openNodes = new LinkedList<Node>();
            LinkedList<Node> closedNodes = new LinkedList<Node>();
            bool isSearchDone = false;


            closedNodes.AddLast(grids[startPos]);
            grids[startPos].SetGH(startPos, endPos);
            short whileCounterMax = (short)1000;
            while (!isSearchDone)
            {
                GetNearNodes(ref openNodes, closedNodes.Last().nodeCenterPosition, closedNodes);
                Vector2Int lowerstHNodePos = Vector2Int.zero;
                int lowerstHValue = int.MaxValue;
                while (openNodes.Count > 0)
                {
                    Node node = openNodes.First();
                    node.SetGH(startPos, endPos);
                    openNodes.RemoveFirst();
                    if (lowerstHValue > node.H)
                    {
                        lowerstHValue = node.H;
                        lowerstHNodePos = node.nodeCenterPosition;
                    }


                    if (endPos == node.nodeCenterPosition)
                    {
                        isSearchDone = true;
                        openNodes.Clear();
                    }
                }
                openNodes.Clear();

                if (grids[lowerstHNodePos].G > 10) grids[lowerstHNodePos].connectedNode = closedNodes.Last();
                else grids[lowerstHNodePos].connectedNode = grids[startPos];
                closedNodes.AddLast(grids[lowerstHNodePos]);
                --whileCounterMax;
                if (whileCounterMax <= 0) return closedNodes.ToArray();
            }
            //예외처리
            Node[] checkedShortestNodes = new Node[0];
            ushort tempCount = (ushort)closedNodes.Count;
            for (ushort i = 0; i < tempCount; i++)
            {
                Array.Resize(ref  checkedShortestNodes, i+1);
                checkedShortestNodes[i] = closedNodes.Last();
                closedNodes.RemoveLast();
                if (grids[startPos] == checkedShortestNodes[i].connectedNode)
                {
                    Array.Resize(ref checkedShortestNodes, i + 2);
                    checkedShortestNodes[i + 1] = grids[startPos];
                    break;
                }
            }
            return checkedShortestNodes;

        }*/
    /// <summary>
    /// 가까운 Node를 Add해주는 함수 (좌,우,하,상 순서)
    /// </summary>
    /// <param name="nearNodes">add할 linkedList<Node>    </param>
    /// <param name="position">기준점</param>
    /// <param name="closedNodes">닫힌노드,비교를 위해 필요</param>
    public void GetNearOpenNodes(ref LinkedList<Node> nearNodes, Vector2Int position/*,ref LinkedList<Vector2Int> noneWalkAbleNodes*/, LinkedList<Node> closedNodes)
    {
        //해당 가까운 노드를 등록
        Vector2Int[] tempN = NearPositions(position);
        for (sbyte i = 0; i < tempN.Length; i++)
        {
            if (grids.ContainsKey(tempN[i]))
            {
                if (grids[tempN[i]].isMoveableTile && !closedNodes.Contains(grids[tempN[i]])) nearNodes.AddLast(grids[tempN[i]]);
            }
        }
    }
    public void GetNearClosedNodes(ref LinkedList<Node> reversedClosedNode, Vector2Int position, LinkedList<Node> closedNodes)
    {
        //해당 좌표에 노드가 없거나 wallkable이 아니면 해당 배열은 null로 바꿔줌
        int lowerstGValue = int.MaxValue;
        Node tempNode = null;
        Vector2Int[] nearPositions = NearPositions(position);
        for (sbyte i = 0; i < nearPositions.Length; i++)
        {
            if (closedNodes.Contains(grids[nearPositions[i]]))
            {
                if (lowerstGValue > grids[nearPositions[i]].G&&!reversedClosedNode.Contains(grids[nearPositions[i]]))
                {
                    lowerstGValue = grids[nearPositions[i]].G;
                    tempNode = grids[nearPositions[i]];
                }
            }
        }
        if (tempNode != null)
        {
            grids[position].connectedNode = tempNode;
            reversedClosedNode.AddFirst(tempNode);
        }

    }
    /// <summary>
    /// 가까운 노드의 존재여부를 판단
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector2Int[] NearPositions(Vector2Int position)
    {
        Vector2Int[] tempVec = new Vector2Int[0];

        if (grids.ContainsKey(position + Vector2Int.left))
        {
            if (grids[position + Vector2Int.left].isMoveableTile)
            {
                Array.Resize(ref tempVec, tempVec.Length + 1);
                tempVec[tempVec.Length - 1] = position + Vector2Int.left;
            }
        }

        if (grids.ContainsKey(position + Vector2Int.right))
        {
            if (grids[position + Vector2Int.right].isMoveableTile)
            {
                Array.Resize(ref tempVec, tempVec.Length + 1);
                tempVec[tempVec.Length - 1] = position + Vector2Int.right;
            }

        }

        if (grids.ContainsKey(position + Vector2Int.down))
        {
            if (grids[position + Vector2Int.down].isMoveableTile)
            {
                Array.Resize(ref tempVec, tempVec.Length + 1);
                tempVec[tempVec.Length - 1] = position + Vector2Int.down;
            }

        }

        if (grids.ContainsKey(position + Vector2Int.up))
        {
            if (grids[position+Vector2Int.up].isMoveableTile)
            {
                Array.Resize(ref tempVec, tempVec.Length + 1);
                tempVec[tempVec.Length - 1] = position + Vector2Int.up;
            }
        }
        return tempVec;
    }
    public void AttackOrder(Stats attackerStat,Stats targetStat,int range,Action setAttackState)
    {
        if (attackerStat.target != targetStat)
        {
            attackerStat.target = targetStat;
        }
        if (targetStat == null || attackerStat == null) return;
        if(range == 10)
        {
            if (IsMeleeAttackAble(attackerStat.standingNode, targetStat.standingNode))
            {
                if (targetStat.isCharacterDie)
                {
                    return;
                }
                else
                {
                    setAttackState();
                    setAttackState = null;
                }
                return;
            }
            else
            {
                attackerStat.moveFunction(new Vector3(targetStat.standingNode.nodeCenterPosition.x, targetStat.standingNode.nodeFloor, targetStat.standingNode.nodeCenterPosition.y), setAttackState,attackerStat.CharactorAttackRange);
                setAttackState = null;
                return;
            }
        }
        else
        {
            LinkedList<Node> tempList = GridManager.instance.PathFinding(attackerStat.standingNode.nodeCenterPosition,
                targetStat.standingNode.nodeCenterPosition);

            if (tempList.Max(node => node.H) <= range)
            {
                //여기다가 스테이트머신 attack 넣으면됨
                setAttackState();
                return;
            }
            return;
        }
    }
    public bool IsInRange(Node attackerNode, Node targetNode, int range)
    {
        //GridManager의 GetDistance와 같은 식, 정적메모리 접근 과중화를 막기 위해 별도로 작성함
        // 두 점 사이의 x와 y 좌표 차이 계산
        Vector2Int dist = new Vector2Int(Mathf.Abs(attackerNode.nodeCenterPosition.x - targetNode.nodeCenterPosition.x),
            Mathf.Abs(attackerNode.nodeCenterPosition.y - targetNode.nodeCenterPosition.y));

        //         10 * (dist.x + dist.y) + (14 - 2 * 10) * Mathf.Min(dist.x, dist.y) == 대각선 거리 계산 (두 점 사이의 가장 큰 차이 값)
        if (10 * (dist.x + dist.y) + (14 - 2 * 10) * Mathf.Min(dist.x, dist.y) <= range) return true;
        else return false;
    }
    /// <summary>
    /// 목표 노드가 캐릭터 근처에 있으면 true, 없으면 false를 리턴
    /// </summary>
    /// <param name="attackerStandingNode"></param>
    /// <param name="targetStandingNode"></param>
    /// <returns></returns>
    private bool IsMeleeAttackAble(Node attackerStandingNode,Node targetStandingNode)
    {

        if (grids.ContainsKey(attackerStandingNode.nodeCenterPosition + Vector2Int.left))
        {
            if (grids[attackerStandingNode.nodeCenterPosition + Vector2Int.left] == targetStandingNode)
            {
                return true;
            }
        }

        if (grids.ContainsKey(attackerStandingNode.nodeCenterPosition + Vector2Int.right))
        {
            if (grids[attackerStandingNode.nodeCenterPosition + Vector2Int.right] == targetStandingNode)
            {
                return true;
            }

        }

        if (grids.ContainsKey(attackerStandingNode.nodeCenterPosition + Vector2Int.down))
        {
            if (grids[attackerStandingNode.nodeCenterPosition + Vector2Int.down] == targetStandingNode)
            {
                return true;
            }

        }

        if (grids.ContainsKey(attackerStandingNode.nodeCenterPosition + Vector2Int.up))
        {
            if (grids[attackerStandingNode.nodeCenterPosition + Vector2Int.up] == targetStandingNode)
            {
                return true;
            }
        }
        return false;
    }
}
public class UIManager : Manager<UIManager>
{
    public PlayerUI playerUI;
    private RectTransform mainCanvas;
    public RectTransform MainCanvas
    {
        get
        {
            if (mainCanvas == null)
            {
                mainCanvas = (RectTransform)GameObject.Find("Canvas").transform;
            }
            return mainCanvas;
        }
    }

    private event Action<Vector3, string, Color, float?> textEffectFuc;
    private Image draggingIconImage;
    private SlotInfo slotInfo = new SlotInfo();
    public SlotInfo SlotInfo
    {
        get 
        { 
            if(slotInfo == null) slotInfo = new SlotInfo();
            return slotInfo;
        }
    }
    /// <summary>
    /// 0 = leftHand,1 = rightHand
    /// </summary>
    public InventorySlots[] equipWindowWeapons;
    /// <summary>
    /// 0 = head, 1 = chest, 2 = pants, 3= boots , 4 = gauntlet
    /// </summary>
    public InventorySlots[] equipWindowArmors;

    //캐스팅바 시리즈
    private RectTransform outerCastBar;
    private RectTransform OuterCastBar
    {
        get 
        { 
            if(outerCastBar == null)
            {
                if (outerCastBar == null) outerCastBar = (RectTransform)MainCanvas.Find("OuterCastBar").transform;
            }
            return outerCastBar;
        }
    }

    public Transform questNoti
    {
        get
        {
            return MainCanvas.Find("QuestNotific");
        }
    }
    public TextMeshProUGUI questNotiText
    {
        get
        {
            return questNoti.Find("notificText").GetComponent<TextMeshProUGUI>();
        }
    }

    private Image playerHPBar;
    private Image PlayerHPBar
    {
        get 
        {
            if (playerHPBar == null) playerHPBar = MainCanvas.Find("PlayerHPBar").GetComponent<Image>();
            return playerHPBar;
        }
    }
    private Text hpText;
    private Text HpText
    {
        get 
        {
            if (hpText == null)
            {
                hpText = PlayerHPBar.transform.Find("SliderText").GetComponent<Text>();
            }
            return hpText;
        }
    }
    private Slider BaseEXPBar
    {
        get 
        {
            return MainCanvas.Find("BaseExperienceBar").GetComponent<Slider>();
        }
    }
    private Slider JobEXPBar
    {
        get 
        {
            return MainCanvas.Find("JobExperienceBar").GetComponent<Slider>();
        }
    }
    private Text BaseLevelText
    {
        get
        {
            return BaseEXPBar.transform.Find("BaseLevelBox").GetChild(0).GetComponent<Text>();
        }
    }
    private Text JobLevelText
    {
        get
        {
            return JobEXPBar.transform.Find("JobLevelBox").GetChild(0).GetComponent<Text>();
        }
    }
    public Inventory<Consumables> consumeInven = new Inventory<Consumables>("CosumeableTab");
    public Inventory<Equips> equipInven = new Inventory<Equips>("EquipTabs");
    public Inventory<Miscs> miscInven = new Inventory<Miscs>("MiscTabs");
    float playerGold;
    public float PlayerGold
    {
        get { return playerGold; } 
        set 
        {
            double result = playerGold + value;
            if ((result) <= float.MaxValue&&result >= 0) 
            {
                playerGold = value;
            }
        }
    }
    public bool IsEnoughGold(float pay)
    {
        if(PlayerGold-pay >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateLevel()
    {
        BaseLevelText.text = Player.Instance.playerLevelInfo.baseLevel.ToString();
        JobLevelText.text = Player.Instance.playerLevelInfo.jobLevel.ToString();
    }
    public void UpdateExp(float currBase,float maxBase,float currJob,float maxJob)
    {
        Slider tempBase = BaseEXPBar;
        Slider tempJob = JobEXPBar;
        tempBase.value = currBase;
        tempBase.maxValue = maxBase;
        tempJob.value = currJob;
        tempJob.maxValue = maxJob;
    }
    /// <summary>
    /// Max,CurrntHP
    /// </summary>
    public (float,float) PlayerMaxCurrHP
    {
        set 
        {
            PlayerHPBar.material.SetFloat("_FillAmount", value.Item2 / value.Item1); 
            //HpText.text = value.Item2.ToString("0") + '/' + value.Item1.ToString("0");
        }
    }

    private Image playerSPBar;
    private Image PlayerSPBar
    {
        get 
        {
            if (playerSPBar == null) playerSPBar = MainCanvas.Find("PlayerSPBar").GetComponent<Image>();
            return playerSPBar;
        }
    }
    private Text spText;
    private Text SpText
    {
        get 
        {
            if (spText == null)
            {
                spText = PlayerSPBar.transform.Find("SliderText").GetComponent<Text>();
            }
            return spText;
        }
    }
    /// <summary>
    /// Max,CurrntHP
    /// </summary>
    public (float,float) PlayerMaxCurrSP
    {
        set 
        {
            PlayerSPBar.material.SetFloat("_FillAmount", value.Item2 / value.Item1);
            //SpText.text = value.Item2.ToString("0") +'/'+ value.Item1.ToString("0");
        }
    }

    private Image innerCastBar;
    public Image InnerCastBar 
    {
        get
        {
            if(innerCastBar == null)
            {
                if(innerCastBar == null) innerCastBar = OuterCastBar.Find("InnerCastBar").GetComponent<Image>();
            }
            return innerCastBar;
        }
    }
    Slider MonsterHPBarOrigin
    {
        get { return MainCanvas.Find("MonsterHPBar").GetComponent<Slider>(); }
    }
    Queue<Slider> monsterHPBarPool = new Queue<Slider>();
    
    public Transform DialogPannel { get { return MainCanvas.Find("Dialog"); } }
    public bool isDiaListUp;
    private TextMeshProUGUI dialogText;
    public TextMeshProUGUI DialogText
    {
        get 
        { 
            if (dialogText == null) 
            {
                dialogText = DialogPannel.Find("InnterPannel").Find("Context").Find("DialogText").GetComponent<TextMeshProUGUI>(); 
            } 
            return dialogText;
        }
    }
    private TextMeshProUGUI nameText;
    public TextMeshProUGUI NameText
    {
        get { if (nameText == null) { nameText = DialogPannel.Find("InnterPannel").Find("NameBar").Find("NPCNameText").GetComponent<TextMeshProUGUI>(); } return nameText; }
    }
    public void HPBarEnqueue(Slider target)
    {
        target.transform.position = new Vector3(9990, 999, 9999);
        monsterHPBarPool.Enqueue(target);
        target.gameObject.SetActive(false);
    }
    public Slider HPBarDequeue()
    {
        if(monsterHPBarPool.Count > 0)
        {
            Slider temp = monsterHPBarPool.Dequeue();
            temp.gameObject.SetActive(true);
            return temp;
        }
        else
        {
            Slider newOBJ =  GameObject.Instantiate(MonsterHPBarOrigin.gameObject,MainCanvas).GetComponent<Slider>();
            return newOBJ;
        }

    }
    public void SetCastingBarValue(float max,float curr)
    {
        CastingBarOnOff(true);
        InnerCastBar.fillAmount = curr/max;
    }
    public void CastingBarOnOff(bool isTurnOn)
    {
        OuterCastBar.gameObject.SetActive(isTurnOn);
    }
    public void DraggingIcons(Vector2 pos,Sprite sprite) 
    {

        if (draggingIconImage == null) IconOnOFF(true);

        if (!draggingIconImage.gameObject.activeSelf) draggingIconImage.gameObject.SetActive(true);

        if (draggingIconImage.sprite != sprite) 
        {
            draggingIconImage.sprite = sprite;
        }
        draggingIconImage.rectTransform.position = pos;
    }
    public void IconOnOFF(bool isTurnOFF)
    {
        if (draggingIconImage == null)
        {
            draggingIconImage = new GameObject("DraggingIcons").AddComponent<Image>();
            draggingIconImage.rectTransform.SetParent(GameObject.Find("Canvas").transform);
        }
        draggingIconImage.gameObject.SetActive(isTurnOFF);
    }
    public void ResetUI()
    {
        playerUI ??= new PlayerUI();
        playerUI?.ResetUI();
    }
    public void FontParticleRegist(Action<Vector3, string, Color, float?> action)
    {
        textEffectFuc = null;
        textEffectFuc += action;
    }
    public void SpawnFloatText(Vector3 pos, string str, Color color, float size)
    {
        textEffectFuc?.Invoke(pos, str, color, size);
    }
}
public class SkillManager : Manager<SkillManager>
{
    public List<SkillInfoInGame> skillInfo = new List<SkillInfoInGame>();
    public List<buffTime> buffTimer = new List<buffTime>();
    public Dictionary<string, EffectController> skillEffects = new Dictionary<string, EffectController>();
    public Image[] buffIcons;
    public Image[] BuffIcons
    {
        get
        {
            if(buffIcons == null)
            {
                buffIcons = new Image[5];
                for (byte i = 0; i < buffIcons.Length; i++)
                {
                    buffIcons[i] = UIManager.GetInstance().MainCanvas.Find("Buffs").GetChild(i).GetChild(0).GetComponent<Image>();
                    buffIcons[i].transform.parent.gameObject.SetActive(false);
                }
            }
            return buffIcons;
        }
    }
    public HashSet<int> buffIconMap = new HashSet<int>();
    public class buffTime
    {
        public buffTime(string buffName,int leftTime,byte buffLevel,Action action, Action<string> hashAction,bool isPlayerAccepted)
        {
            this.buffName = buffName;
            this.leftTick = leftTime;
            this.originTick = leftTime;
            this.buffLevel = buffLevel;
            this.removeFunc = action;
            this.hashAction = hashAction;
            if (isPlayerAccepted) 
            {
                iconIndex = SkillManager.GetInstance().GetIconIndex();
                if(iconIndex > -1)
                {
                    icon = GetInstance().BuffIcons[iconIndex];
                    icon.sprite = ResourceManager.GetInstance().SkillIconAtlas.GetSprite(buffName);
                    icon.transform.parent.GetComponent<Image>().sprite = ResourceManager.GetInstance().SkillIconAtlas.GetSprite(buffName);
                    GetInstance().buffIconMap.Add(iconIndex);
                    if (!icon.transform.parent.gameObject.activeSelf) icon.transform.parent.gameObject.SetActive(true);
                }
            }
        }
        public string buffName;
        public int leftTick;
        public int originTick;
        public byte buffLevel;

        private sbyte iconIndex;
        private Image icon { get; set; }
        public Action removeFunc;
        public Action<string> hashAction;

        public void UpdateTimer()
        {
            leftTick -= 1;
            if(icon != null)
            {
                icon.fillAmount = leftTick / Mathf.Floor(originTick);
                
            }
        }
        public void End()
        {
            removeFunc.Invoke();
            hashAction.Invoke(buffName);
            if(iconIndex >= 0)
            {
                GetInstance().buffIconMap.Remove(iconIndex);
                icon.transform.parent.gameObject.SetActive(false);
            }
            GetInstance().buffTimer.Remove(this);
        }
    }

    public bool SkillEffect(Vector3 position, float size, string effectName,float rot)
    {
        if (size <= 0) size = 1;
        if (skillEffects.ContainsKey(effectName))
        {
            if (skillEffects[effectName] != null)
            {
                skillEffects[effectName].PlayOrder(position, size*Vector3.one,rot);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public void RegistVFXDict(string name,GameObject prefab)
    {
        if(skillEffects.ContainsKey(name))
        {
            if (skillEffects[name] != null) return;
            skillEffects[name] = new GameObject(name + "Effect").AddComponent<EffectController>();
            skillEffects[name].prefab = prefab;
            skillEffects[name].SetDurationTime();
            return;
        }
        skillEffects.Add(name, new GameObject(name + "Effect").AddComponent<EffectController>());
        skillEffects[name].prefab = prefab;
        skillEffects[name].SetDurationTime();
    }
    public sbyte GetIconIndex()
    {
        for (sbyte i = 0; i < BuffIcons.Length; i++)
        {
            if (!buffIconMap.Contains(i))
            {
                return i;
            }
        }
        return -1;
    }
    float skillTimer = 0;
    float tickTime = 0.3f;
    float GC
    {
        get { return Player.Instance.playerLevelInfo.stat.GlobalCooltimePercent * 1.2f; }
    }
    public void SetSkillCoolTime(string name,int tick)
    {
        int tempTick = Mathf.RoundToInt((GC / tickTime));
        foreach (SkillInfoInGame skill in skillInfo)
        {
            if (skill.skillName != name)
            {
                if(tempTick < skill.leftTick)
                {
                    continue;
                }
                else
                {
                    skill.leftTick = tempTick;
                    skill.originTick = tempTick;
                }
            }
            else
            {
                skill.originTick = tick < tempTick ? tempTick : tick;
                skill.leftTick = tick < tempTick ? tempTick : tick;
            }
        }
    }
    public void RegistBuffTimer(buffTime buffTimer)
    {
        this.buffTimer.Add(buffTimer);
    }
    public void UpdateSkillCoolTime()
    {
        skillTimer += Time.deltaTime;
        if (tickTime > skillTimer) return;
        if (buffTimer.Count > 0)
        {
            for (int i = 0; i < buffTimer.Count; i++)
            {
                buffTimer[i].UpdateTimer();
                if (buffTimer[i].leftTick <= 0)
                {
                    buffTimer[i].End();
                    i--;
                }
            } 
        }
        for (int i = 0; i< skillInfo.Count; i++)
        {
            if (skillInfo[i].leftTick > 0)
            {
                if (skillInfo[i].leftTick == 1)
                {
                    skillInfo[i].iconRenderer.fillAmount = 1;
                    skillInfo[i].leftTick = 0;
                    skillInfo[i].originTick = 0;
                    continue;
                }
                else
                {
                    skillInfo[i].iconRenderer.fillAmount = (skillInfo[i].originTick - skillInfo[i].leftTick) /Mathf.Floor(skillInfo[i].originTick);
                }
                skillInfo[i].leftTick--;
                
            }
            else continue;
        }
        skillTimer = 0;
    }
    public void AddSkillInfo(SkillInfoInGame addItem)
    {
        skillInfo.Add(addItem);
    }
}
public class MonsterManager : Manager<MonsterManager>
{
    List<RespawnBox> respawnMonsters = new List<RespawnBox>();
    private DropManaging drop;
    public DropManaging Drop
    {
        get { if (drop == null) drop = new DropManaging(); return drop;  }
    }
    public void AddRespawnList(MonsterBase targetMonster)
    {
        targetMonster.gameObject.SetActive(false);
        respawnMonsters.Add(new RespawnBox(targetMonster, targetMonster.respawnSec));
        targetMonster.CurrentNode = null;
    }
    public void Respawn(int target)
    {

        respawnMonsters[target].leftRespawnTime += 10;
        if (respawnMonsters[target].monster.Respawn())
        {
            respawnMonsters.RemoveAt(target);
            return;
        }
        

    }
    public void UpdateRespawnTime(float deltatime)
    {
        for (int i = respawnMonsters.Count-1; i >= 0; i--)
        {
            respawnMonsters[i].UntillRespawn(deltatime);
            if (respawnMonsters[i].leftRespawnTime <= 0)
            {
                Respawn(i);

                break;
            }
        }
    }

}
public class QuestManager : Manager<QuestManager>
{
    private List<Quest> acceptedQuests;
    public List<Quest> AcceptedQuests
    {
        get
        {
            if (acceptedQuests == null) acceptedQuests = new List<Quest>();
            return acceptedQuests;
        }
    }
    private List<Quest> clearedQuests;
    public List<Quest> ClearedQuests
    {
        get
        {
            if (clearedQuests == null) clearedQuests = new List<Quest>();
            return clearedQuests;
        }
    }
    public Action<string> huntEvent;
    public Action<string> collectEvent;
    public Action<string> conversationEvent;
    public Action<string> interactiveEvent;

    public void AcceptQuest(Quest quest)
    {
        AcceptedQuests.Add(quest);
    }
    public void ClearQuest(Quest quest)
    {
        int tempNum = acceptedQuests.IndexOf(quest);
        if (tempNum == -1) return;
        ClearedQuests.Add(AcceptedQuests[tempNum]);
        AcceptedQuests[tempNum].QuestClear();
        AcceptedQuests.RemoveAt(tempNum);
    }
    public void ConditionCheck()
    {

    }
}