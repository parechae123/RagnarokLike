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


public class Player : MonoBehaviour,ICameraTracker
{
    public PlayerLevelInfo playerLevelInfo;
    private static Player instance;
    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("Player").GetOrAddComponent<Player>();
            }

            return instance;
        }
    }
    //이동 도착 예정시간
    #region 스킬시전 관련 객체
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
                skillIndicator.OnOff(false,false);
                skillIndicator.OnOff(true,false);
                return;
            }

            if(value.skillPosition == SkillPosition.self)
            {
                StateMachine.ChangeState(SkillObj.skill[SkillObj.CastingSkillLevel].defaultCastingTime,
                    SkillObj,
                    null,
                    playerLevelInfo.stat.standingNode.worldPos);
                skillIndicator.SetBoundary(false,value.GetBoundary());
                skillIndicator.SetPosition(false,CurrentNode.worldPos);
            }
            else if(value.skillPosition == SkillPosition.cursor)
            {
                playerCursorState.changeState(cursorState.skillTargeting);
                skillIndicator.SetBoundary(true, value.GetRangeBoundary());
                skillIndicator.OnOff(true, true);
                skillIndicator.SetBoundary(false,value.GetBoundary());
                isSearchCastTarget = true;
            }
        }
    }
    private bool isSearchCastTarget = false;
    public bool isMotionBookCancel = false;
    public PlaySequence playSequence = PlaySequence.nonCombat;
    #endregion
    #region 노드
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
            skillIndicator.SetPosition(true, value.worldPos);
            currentNode = value;
        }
    }
    public LinkedList<Node> nodePreview = new LinkedList<Node>();
    public float arriveTime;
    #endregion
    private DialogStateMachine currDialog;
    public DialogStateMachine CurrDialog
    {
        get 
        {
            if (currDialog == null) playSequence = PlaySequence.nonCombat; 
            return currDialog; }
        set 
        { 
            if(value== null)playSequence = PlaySequence.nonCombat;
            currDialog = value; }
    }
    [SerializeField]
    public PlayerStateMachine stateMachine;
    public PlayerStateMachine StateMachine
    {
        get { return stateMachine; }
    }
    [SerializeField]Transform shadowTR;
    private SpriteRenderer playerSR;
    [SerializeField] private Transform targetCell;
    [SerializeField] CursorStates playerCursorState = new CursorStates();
    [SerializeField]Vector2Int playerLookDir;
    public KeyCode[] boundedKeys = new KeyCode[0];
    public KeyCode combKey;
    public SkillIndicators skillIndicator;
    [System.Serializable]
    public class playerEffects
    {
        public playerEffects(Transform tr)
        {
            this.tr = tr;
        }
        Transform tr;
        public ParticleSystem jobLevelUp
        {
            get { return tr.Find("JobLevelUp").GetComponent<ParticleSystem>(); }
        }
        public ParticleSystem baseLevelUp
        {
            get { return tr.Find("BaseLevelUp").GetComponent<ParticleSystem>(); }
        }
        public ParticleSystem healthPosion
        {
            get { return tr.Find("HPPosion").GetComponent<ParticleSystem>(); }
        }
        public ParticleSystem manaPosion
        {
            get { return tr.Find("ManaPosion").GetComponent<ParticleSystem>(); }
        }
    }
    [SerializeField] public playerEffects defaultEffects;
    public Vector2Int PlayerLookDir
    {
        get { return playerLookDir; }
    }
    public void Awake()
    {
        instance = this;
        if (playerLevelInfo.stat == null)
        {
            playerLevelInfo.stat = new PlayerStat(currentNode, 100, 100, 3, 1, 10, 1, 100);
            QuestManager.GetInstance().AcceptQuest(0);
        }
        if(skillIndicator == null) skillIndicator = new SkillIndicators();
        playerSR = GetComponent<SpriteRenderer>();
        playerLevelInfo.baseLevelUP += playerLevelInfo.BaseLevelUP;
        playerLevelInfo.jobLevelUP += playerLevelInfo.JobLevelUP;
        playerLevelInfo.stat.dieFunctions += PlayerDie;
        //쿨타임 부분 수정필요
    }
    private void Start()
    {
        InstallizeStates();
        SetCurrentNodeAndPosition();
        SetPlayerPositionToCenterPos();
        RegistCameraAction();
        playerLevelInfo.stat.moveFunction += PlayerMoveOrder;
        playerCursorState.changeState(cursorState.defaultCurser);
        defaultEffects = new playerEffects(transform.Find("HeadPoint"));
        SetBindKey();
        
    }

    public void Update()
    {
        if (playerLevelInfo != null) if (playerLevelInfo.stat != null) if (playerLevelInfo.stat.isCharacterDie) return;
        shadowTR.position = transform.position;
        MouseBinding();
        InTime();
        SkillManager.GetInstance().UpdateSkillCoolTime();
        //디버그용 키로 빼야함
        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            playerLevelInfo.GetJobEXP(100);
            playerLevelInfo.GetBaseEXP(10000);
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            UIManager.GetInstance().equipInven.GetItems(new Weapons("10001", "shield", ResourceManager.GetInstance().ItemIconAtlas.GetSprite("Wooden Shield"), new BaseJobType[1] { BaseJobType.Novice }, 0, 0, EquipPart.RightHand, 10, true, WeaponType.Shield,
                new IApixBase<WeaponApixType> { statLine = (BasicStatTypes.Str,10),abilityApixes = new (WeaponApixType, float)[3] {(WeaponApixType.AttackSpeed,0.3f), (WeaponApixType.CastingSpeed, 0.1f), (WeaponApixType.MATK, 0.3f) } }));
            
        }
        if(Input.GetKeyDown(KeyCode.I))
        {
            UIManager.GetInstance().equipInven.GetItems(new Weapons("10002","twoHanded",playerSR.sprite, new BaseJobType[1] { BaseJobType.Novice }, 0, 0, EquipPart.TwoHanded, 0, false, WeaponType.Bow,
                new IApixBase<WeaponApixType> { statLine = (BasicStatTypes.AGI, 10), abilityApixes = new (WeaponApixType, float)[3] { (WeaponApixType.AttackSpeed, 0.3f), (WeaponApixType.CastingSpeed, 0.3f), (WeaponApixType.MATK, 0.3f) } }));
            
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            UIManager.GetInstance().equipInven.GetItems(new Armors("20001","hat", playerSR.sprite, new BaseJobType[1] { BaseJobType.Novice }, 0, 0, EquipPart.Head, 10,
                new IApixBase<ArmorApixType> { statLine = (BasicStatTypes.Str, 10), abilityApixes = new (ArmorApixType, float)[3] { (ArmorApixType.MaxHp, 0.3f), (ArmorApixType.MaxMana, 0.3f), (ArmorApixType.Evasion, 0.3f) } }, ArmorMat.Leather,true));
            
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            UIManager.GetInstance().equipInven.GetItems(new Armors("20002","gabba", playerSR.sprite, new BaseJobType[1] { BaseJobType.Novice }, 0, 0, EquipPart.Chest, 10,
                new IApixBase<ArmorApixType> { statLine = (BasicStatTypes.Str, 10), abilityApixes = new (ArmorApixType, float)[3] { (ArmorApixType.MaxHp, 0.3f), (ArmorApixType.MaxMana, 0.3f), (ArmorApixType.Evasion, 0.3f) } }, ArmorMat.PlateArmor,false));

        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            UIManager.GetInstance().equipInven.GetItems(new Weapons("10002","Bow",playerSR.sprite, new BaseJobType[1] { BaseJobType.Novice }, 0, 0, EquipPart.LeftHand, 0, false, WeaponType.Bow,
                new IApixBase<WeaponApixType> { statLine = (BasicStatTypes.AGI, 10), abilityApixes = new (WeaponApixType, float)[3] { (WeaponApixType.AttackSpeed, 0.3f), (WeaponApixType.CastingSpeed, 0.3f), (WeaponApixType.MATK, 0.3f) } }));
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            UIManager.GetInstance().consumeInven.GetItems(new Potions("30001","체력 포션", ResourceManager.GetInstance().ItemIconAtlas.GetSprite("Red Potion"), 100,PotionType.HP,100));
        }
        if(Input.GetKeyDown(KeyCode.LeftBracket))
        {
            UIManager.GetInstance().consumeInven.GetItems(new Potions("30002","마나 포션", ResourceManager.GetInstance().ItemIconAtlas.GetSprite("Blue Potion"), 100,PotionType.SP,100));
        }
        KeyBoardBinding();
        StateMachine.CurrentState.Execute();

    }

    private void InTime() 
    {
        playerLevelInfo.stat.statTimer += Time.deltaTime;
        if (playerLevelInfo.stat.statTimer >= playerLevelInfo.stat.RegenTime) 
        {
            playerLevelInfo.stat.HP += playerLevelInfo.stat.HPRegen;

            playerLevelInfo.stat.SP += playerLevelInfo.stat.SPRegen;

            playerLevelInfo.stat.statTimer = 0;
        }
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
            arriveTime = 0;
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
    public void SetBindKey()
    {
        boundedKeys = KeyMapManager.GetInstance().ConvertKeyArray();
        combKey = KeyMapManager.GetInstance().combKey;
        foreach ( KeyCode codes in boundedKeys)
        {
            if (KeyMapManager.GetInstance().keyMaps[codes].SlotNumber>= 0)
            {
                ShortCutOBJ temp = KeyMapManager.GetInstance().keyMaps[codes];
                temp.subScribFuncs += UIManager.GetInstance().MainCanvas.transform.Find("QuickSlots").GetChild(KeyMapManager.GetInstance().keyMaps[codes].SlotNumber).GetComponent<QuickSlot>().GetSlotKey;
                KeyMapManager.GetInstance().keyMaps[codes] = temp; 
            }
        }
        
    }

    public void RegistCameraAction()
    {
        PlayerCam.Instance.rotations += FollowCamera;
        PlayerCam.Instance.rotDirrection += StateMachine.CamRotAnimChange;
    }
    public void UnRegistCameraAction()
    {
        PlayerCam.Instance.rotations -= FollowCamera;
        PlayerCam.Instance.rotDirrection -= StateMachine.CamRotAnimChange;
    }
    public void FollowCamera()
    {
        transform.rotation = Camera.main.transform.rotation;
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
        transform.position = CurrentNode.worldPos;
    }
    #endregion
    #region 움직임,공격 관련

    public void PlayerMove(Action arriveAction, int arriveDistanceFromTargetPos = -1)
    {
        float moveSpeedPerSec = 1 / playerLevelInfo.stat.MoveSpeed;
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
                if (arriveDistanceFromTargetPos != -1)
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
                tempNodePosArray[i] = nodePreview.First().worldPos;
                //tempNodePosArray[i].y += playerSR.bounds.size.y;
            }
            else
            {
                --i;
            }
            nodePreview.RemoveFirst();
        }

        Path tempPath = new Path(PathType.Linear, tempNodePosArray, 1);

        transform.DOKill(false);
        arriveTime = tempNodePosArray.Length * moveSpeedPerSec;
        DOPath(transform, tempPath, tempNodePosArray.Length * moveSpeedPerSec).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (arriveAction != null) 
            { 
                arriveAction();
                arriveAction = null;
            }
        }).SetEase(Ease.Linear);
        return;


    }
    public TweenerCore<Vector3, Path, PathOptions> DOPath(Transform target, Path path, float duration, PathMode pathMode = PathMode.Full3D)
    {
        TweenerCore<Vector3, Path, PathOptions> tweenerCore = DOTween.To(PathPlugin.Get(), () => target.position, delegate (Vector3 x)
        {
            stateMachine.SetDirrection(ref playerLookDir,target.position,x);

            
            StateMachine.AnimationChange();
            if (playSequence == PlaySequence.dialog|| playSequence == PlaySequence.die|| playSequence == PlaySequence.shopping)
            {
                target.DOKill(false);
                targetNode = null;
                SetPlayerPositionToCenterPos();
                //PlayerMoveOrder(path.wps[path.wpLengths.Length-1]);
                stateMachine.ChangeState("idleState");
                return;
            }
            Node nextNode = GridManager.GetInstance().PositionToNode(x);
            if (null == nextNode.CharacterOnNode|| playerLevelInfo.stat == nextNode.CharacterOnNode)
            {
                CurrentNode = nextNode;
            }
            else
            {
                target.DOKill(false);
                targetNode = null;
                SetPlayerPositionToCenterPos();
                //PlayerMoveOrder(path.wps[path.wpLengths.Length-1]);
                stateMachine.ChangeState("idleState");
                return;
            }
            target.position = x;

        }, path, duration).SetTarget(target);
        tweenerCore.plugOptions.mode = pathMode;
        return tweenerCore;
    }
    #endregion
    #region 키 바인딩
    public void KeyBoardBinding()
    {

        for (byte i = 0; i < boundedKeys.Length; i++)
        {
            if (KeyMapManager.GetInstance().keyMaps[boundedKeys[i]].needCombKey)
            {
                if (Input.GetKey(combKey))
                {
                    if (Input.GetKeyDown(boundedKeys[i]))
                    {
                        KeyMapManager.GetInstance().keyMaps[boundedKeys[i]].subScribFuncs?.Invoke();
                    } 
                }
                
            }
            else
            {
                if (Input.GetKeyDown(boundedKeys[i]))
                {
                    KeyMapManager.GetInstance().keyMaps[boundedKeys[i]].subScribFuncs?.Invoke();
                }
            }
        }
    }
    /// <summary>
    /// 마우스는 고정값을 사용
    /// </summary>
    public void MouseBinding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] groundHit = Physics.RaycastAll(ray, 1000f, 8);
        RaycastHit[] itemHit = Physics.RaycastAll(ray, 1000f, 128);
        RaycastHit[] npcHit = Physics.RaycastAll(ray, 1000f, 256);
        RaycastHit[] monsterHit = Physics.RaycastAll(ray, 1000f, 64);

        if (isSearchCastTarget)
        {
            
            if (targetCell.gameObject.activeSelf) targetCell.gameObject.SetActive(false);
            if ( groundHit.Length == 0) return;
            if(monsterHit.Length> 0)
            {
                skillIndicator.SetPosition(false, GridManager.GetInstance().PositionToNode(monsterHit[0].point).worldPos);
                GetCastingTarget(GridManager.GetInstance().PositionToNode(monsterHit[0].point).nodeCenterPosition);
            }
            else
            {
                Node tempNode = GridManager.GetInstance().PositionToNode(groundHit[0].point);
                if (tempNode == null) return;
                skillIndicator.SetPosition(false, tempNode.worldPos);
                GetCastingTarget(tempNode.nodeCenterPosition);
            }
            
            return;
        }

        if (playSequence == PlaySequence.dialog)
        {
            playerCursorState.changeState(cursorState.dialog);
            if (targetCell.gameObject.activeSelf) targetCell.gameObject.SetActive(false);
        }
        else if (EventSystem.current.IsPointerOverGameObject())
        {
            playerCursorState.changeState(cursorState.defaultCurser);
        }
        else if (npcHit.Length >0)
        {
            playerCursorState.changeState(cursorState.dialog);
            if (targetCell.gameObject.activeSelf) targetCell.gameObject.SetActive(false);
        }
        else if (itemHit.Length > 0)
        {
            playerCursorState.changeState(cursorState.itemCursor);
            if (targetCell.gameObject.activeSelf) targetCell.gameObject.SetActive(false);
        }
        else if (monsterHit.Length > 0)
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
                    targetCell.position = GridManager.GetInstance().PositionToNode(groundHit[0].point).worldPos;
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
            if (playSequence == PlaySequence.dialog)
            {
                currDialog?.NextDialog();
                return;
            }
            if (playSequence == PlaySequence.die || playSequence == PlaySequence.shopping || EventSystem.current.IsPointerOverGameObject()) return;


            if (npcHit.Length > 0)
            {
                GridManager.GetInstance().PositionToNode(npcHit[0].transform.position).CharacterOnNode.OnClick();
            }

            if(itemHit.Length > 0)
            {
                if (GridManager.GetInstance().IsInRange(playerLevelInfo.stat.standingNode, GridManager.GetInstance().PositionToNode(itemHit[0].transform.position),10)) 
                {
                    itemHit[0].transform.GetComponent<DropItems>().GetItem();
                }
                else
                {
                    PlayerMoveOrder(GridManager.GetInstance().PositionToNode(itemHit[0].transform.position).worldPos, () => { itemHit[0].transform.GetComponent<DropItems>().GetItem(); });
                }
                
            }
            if (monsterHit.Length > 0)
            {
                if (SetTargetMonster(monsterHit[0].transform))
                {
                    GridManager.GetInstance().AttackOrder(playerLevelInfo.stat, GridManager.GetInstance().PositionToNode(monsterHit[0].point)?.CharacterOnNode, playerLevelInfo.stat.CharactorAttackRange, SetAttackState);
                }
            }
            else if (groundHit.Length > 0)
            {
                PlayerMoveOrder(groundHit[0].point,null);
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
                    //대상이 있을 시
                    else
                    {
                        //공격이 가능한지 먼저 판별, 공격 가능 시
                        playerLevelInfo.stat.target = GridManager.GetInstance().grids[pos].CharacterOnNode;
                        if (skillObj.skillType != SkillType.buff && playerLevelInfo.stat.target == playerLevelInfo.stat) { SkillObj = null; playerLevelInfo.stat.target = null;return; }

                        if (GridManager.GetInstance().IsInRange(playerLevelInfo.stat.standingNode, playerLevelInfo.stat.target.standingNode, playerLevelInfo.stat.CharactorAttackRange))
                        {
                            playerCursorState.changeState(cursorState.skillTargeting);

                            StateMachine.ChangeState(SkillObj.skill[SkillObj.CastingSkillLevel].defaultCastingTime,
                            SkillObj,
                            playerLevelInfo.stat.target,
                            playerLevelInfo.stat.target.standingNode.worldPos);
                        }
                        //공격 불가 시
                        else
                        {
                            //접근해서 실행
                            if (SetTargetNode(GridManager.GetInstance().grids[pos].worldPos))
                            {
                                if (stateMachine.CurrentState.stateName == "castingState") return;
                                PlayerMove(SetCastingState, SkillObj.skill[SkillObj.CastingSkillLevel].skillRange);
                                StateMachine.ChangeState("moveState");

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
        else if (Input.GetKeyDown(KeyCode.Mouse1)) SkillObj = null;
    }
    public void SetCharactorDirrection(Vector3 start, Vector3 end)
    {
        stateMachine.SetDirrection(ref playerLookDir, start, end);
    }

/*    public void PlayerMoveOrder(Vector3 targetPosition, bool isMoveToAttack = false)
    {
        if (!StateMachine.CurrentState.isCancelableState)
        {
            isMotionBookCancel = true;
            return;
        }
        if (SetTargetNode(targetPosition))
        {
            PlayerMove(isMoveToAttack);
            StateMachine.ChangeState("moveState");
        }
    }*/
    public void PlayerMoveOrder(Vector3 targetPosition,Action arriveAction,int range = -1)
    {
        range = range != -1 ? range - 1 : range ;
        if (playSequence == PlaySequence.dialog || playSequence == PlaySequence.die || playSequence == PlaySequence.shopping) return;
        if (!StateMachine.CurrentState.isCancelableState)
        {
            isMotionBookCancel = true;
            return;
        }
        if (SetTargetNode(targetPosition))
        {
            PlayerMove(arriveAction,range);
            StateMachine.ChangeState("moveState");
        }
    }
    #endregion
    private void InstallizeStates()
    {
        Queue<PlayerStates> states = new Queue<PlayerStates>();
        states.Enqueue(new MoveState( 1, 1, "moveState", "idleState", true));
        states.Enqueue(new IdleState(1, 1, "idleState", "idleState", true));
        states.Enqueue(new AttackState(1, playerLevelInfo.stat.attackSpeed, "attackState", "idleState", false, playerLevelInfo.stat));
        states.Enqueue(new CastingState(1, playerLevelInfo.stat.attackSpeed, "castingState", "idleState", false));
        states.Enqueue(new DamagedState(1, 0.3f, "damagedState", "idleState", false));
        states.Enqueue(new DieState(1, -1, "dieState", "dieState", false));
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
    public void PlayerDie()
    {
        stateMachine.ChangeState(stateMachine.SearchState("dieState") as DieState);
        //TODO : 사망 연출 이곳에 등록
    }
    public void SetAttackState()
    {
        //현재 상태가 attackState가 아닐 경우
        if (StateMachine.CurrentState != StateMachine.SearchState("attackState"))
        {
            //attackState로 바꿉니다
            StateMachine.SetDirrection(ref playerLookDir, playerLevelInfo.stat.standingNode.nodeCenterPosition, playerLevelInfo.stat.target.standingNode.nodeCenterPosition);
            StateMachine.ChangeState("attackState");
        }
    }
    public void SetCastingState()
    {
        if (StateMachine.CurrentState != StateMachine.SearchState("castingState"))
        {
            stateMachine.SetDirrection(ref playerLookDir, playerLevelInfo.stat.standingNode.nodeCenterPosition, playerLevelInfo.stat.target.standingNode.nodeCenterPosition);
            //CastingState로 바꿉니다
            StateMachine.ChangeState(SkillObj.skill[SkillObj.CastingSkillLevel].defaultCastingTime, SkillObj, playerLevelInfo.stat.target, playerLevelInfo.stat.target.standingNode.worldPos);
            SkillObj = null;
        }
    }
}
[System.Serializable]
public class PlayerLevelInfo
{                                                 
    public PlayerStat stat;                                             //플레이어 스텟
    public PlayerSkillTreeForPhase[] playerOwnSkills = new PlayerSkillTreeForPhase[0];     //플레이어가 가지고 있는 스킬 리스트

    #region 베이스 레벨 관련 변수
    public Action baseLevelUP, jobLevelUP;
    public byte baseLevel;
    [SerializeField]private byte maxBaseLevel = 99;
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
            while (value > MaxBaseExp)
            {
                value = value - MaxBaseExp;
                currBaseExp = 0;
                Debug.Log("다음 베이스 맥스경험치" + MaxBaseExp);
                Debug.Log("현재 베이스 경험치" + (currBaseExp + value));
                if (value > 0) baseLevelUP.Invoke();
                else break;

            }
            currBaseExp = value;
        }
    }
    public short usedStatusPoint =0;
    private short statusPoint;
    /// <summary>
    /// Get == statusPoint-usedStatusPoint
    /// Set == usedStatusPoint
    /// </summary>
    public short LeftStatusPoint
    {
        get
        {
            return (short)(statusPoint - usedStatusPoint);
        }
    }
    
    #endregion


    #region 잡레벨 관련 변수
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



    #region BaseLevel관련 함수
    public void BaseLevelUP()
    {
        statusPoint += (short)(3 + (baseLevel / 5));
        baseLevel += 1;
        stat.HP = float.MaxValue;
        stat.SP = float.MaxValue;
        UIManager.GetInstance().SpawnFloatText(Player.Instance.transform.position + (Vector3.up * 2.5f), "Level UP", Color.white, 1.3f);
        UIManager.GetInstance().MainCanvas.Find("UIPopups").Find("StatPopUp").gameObject.SetActive(true);
        Player.Instance.defaultEffects.baseLevelUp.Play(true);
        UIManager.GetInstance().UpdateLevel();
    }
    public void GetBaseEXP(float exp)
    {
        CurrBaseExp += exp;
        UIManager.GetInstance().UpdateExp(CurrBaseExp, MaxBaseExp, CurrJobExp, MaxJobExp);
        Player.Instance.defaultEffects.jobLevelUp.Play(true);
    }
    #endregion
    #region JobLevel관련 함수
    public bool LearnSkill(SkillIconsInSkilltree skill,int targetSkillIndex,SkillInfoInGame skillInfo,int classPhase)
    {
        if (LeftSkillPoint <= 0) return false;
        if (playerOwnSkills.Length < classPhase + 1) Array.Resize(ref playerOwnSkills, classPhase + 1);

        if (playerOwnSkills[classPhase] == null) playerOwnSkills[classPhase] = new PlayerSkillTreeForPhase();
        bool[] isLeanAble = isLearnAble(targetSkillIndex,classPhase, skill);
        //선행스킬 체크
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
                if (item == null) continue;
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

        UIManager.GetInstance().SpawnFloatText(Player.Instance.transform.position + (Vector3.up * 3.9f), "Skill UP", Color.cyan, 1.3f);
        UIManager.GetInstance().MainCanvas.Find("UIPopups").Find("SkillPopUp").gameObject.SetActive(true);
        UIManager.GetInstance().UpdateLevel();
    }
    public void GetJobEXP(float exp)
    {
        CurrJobExp += exp;
        UIManager.GetInstance().UpdateExp(CurrBaseExp, MaxBaseExp, CurrJobExp, MaxJobExp);
    }
    #endregion
    /// <summary>
    /// 스킬을 배울 수 있는지 여부를 반환
    /// </summary>
    /// <param name="targetLearnSkillIndex"></param>
    /// <returns></returns>
    public bool[] isLearnAble(int targetLearnSkillIndex,int classPhase, SkillIconsInSkilltree skillTree)
    {
        //TODO : index 3이상부터 오류 발생 수정 시급
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
                    if (item == null) continue;
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