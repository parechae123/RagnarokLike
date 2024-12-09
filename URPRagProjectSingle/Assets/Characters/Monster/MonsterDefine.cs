using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerDefines.States;
using PlayerDefines.Stat;


public class MIdleState : IState
{
    public string name { get { return "IdleState"; } }
    float timer;
    float searchTimer;
    bool isAgressiveMonster;
    MonsterBase monster;
    public MIdleState(MonsterBase monster)
    {
        this.monster = monster;
        isAgressiveMonster = monster.isAgressiveMob;
    }
    public void Enter()
    {
        timer = 0;
        monster.ChangeAnim(name);
        if (monster.IsInRange(monster.CurrentNode.nodeCenterPosition, Player.Instance.CurrentNode.nodeCenterPosition, monster.RecogDistance))
        {
            if (!isAgressiveMonster && monster.monsterStat.isCharacterDamaged)
            {
                if (monster.IsInRange(monster.CurrentNode.nodeCenterPosition, Player.Instance.CurrentNode.nodeCenterPosition, monster.monsterStat.CharactorAttackRange))
                {
                    monster.ChangeState(new MAttackState(monster));
                }
                else
                {
                    monster.ChangeState(new MMoveState(monster));
                }
            }
            else if (isAgressiveMonster)
            {
                if (monster.IsInRange(monster.CurrentNode.nodeCenterPosition, Player.Instance.CurrentNode.nodeCenterPosition, monster.monsterStat.CharactorAttackRange))
                {
                    monster.ChangeState(new MAttackState(monster));
                }
                else
                {
                    monster.ChangeState(new MMoveState(monster));
                }
            }
        }
    }
    public void Execute()
    {
        timer += Time.deltaTime;
        if (timer >= searchTimer)
        {
            if (monster.IsInRange(monster.CurrentNode.nodeCenterPosition,Player.Instance.CurrentNode.nodeCenterPosition,monster.RecogDistance))
            {
                if (!isAgressiveMonster && monster.monsterStat.isCharacterDamaged)
                {
                    if (monster.IsInRange(monster.CurrentNode.nodeCenterPosition, Player.Instance.CurrentNode.nodeCenterPosition, monster.monsterStat.CharactorAttackRange))
                    {
                        monster.ChangeState(new MAttackState(monster));
                    }
                    else
                    {
                        monster.ChangeState(new MMoveState(monster));
                    }
                }
                else if (isAgressiveMonster)
                {
                    if (monster.IsInRange(monster.CurrentNode.nodeCenterPosition, Player.Instance.CurrentNode.nodeCenterPosition, monster.monsterStat.CharactorAttackRange))
                    {
                        monster.ChangeState(new MAttackState(monster));
                    }
                    else
                    {
                        monster.ChangeState(new MMoveState(monster));
                    }
                }
            }
            timer = 0;
        }
    }
    public void Exit() 
    {
        timer = 0;
    }
}
public class MMoveState : IState
{
    public string name { get { return "MoveState"; } }
    float timer;
    float searchTime = 0.3f;
    MonsterBase monster;
    public MMoveState(MonsterBase monster)
    {
        this.monster = monster;
    }
    public void Enter()
    {
        timer = 0;
        monster.ChangeAnim(name);
        if (!monster.isMonsterMoving)
        {
            if (monster.blockingNode != null)
            {

                if (monster.blockingNode.CharacterOnNode != null && monster.blockingNode.CharacterOnNode != monster.monsterStat)
                {
                    if (!monster.alreadyResearch)
                    {
                        monster.alreadyResearch = true;
                        monster.MoveOrder();
                    }
                    return;
                }
            }
            monster.MoveOrder();
        }
    }
    public void Execute()
    {
        timer += Time.deltaTime;
        if (timer >= searchTime)
        {
            if (monster.IsInRange(monster.CurrentNode.nodeCenterPosition, Player.Instance.CurrentNode.nodeCenterPosition, monster.monsterStat.CharactorAttackRange))
            {
                monster.ChangeState(new MAttackState(monster));
            }
            else
            {
                if (monster.IsInRange(monster.CurrentNode.nodeCenterPosition, Player.Instance.CurrentNode.nodeCenterPosition, monster.RecogDistance))
                {
                    monster.ChangeState(new MMoveState(monster));
                    return;
                }
                else
                {
                    monster.ChangeState(new MIdleState(monster));
                }

            }
            timer = 0;
        }
    }
    public void Exit() 
    { 
        timer = 0;
    }
}
public class MAttackState : IState
{
    public string name { get { return "AttackState"; } }
    float timer;
    bool readyAttack = false;
    MonsterBase monster;
    public MAttackState(MonsterBase monster)
    {
        this.monster = monster;
    }
    public void Enter()
    {
        timer = 0;
        readyAttack = false;
        monster.ChangeAnim("IdleState");
    }
    public void Execute()
    {
        if (monster.IsInRange(monster.monsterStat.standingNode.nodeCenterPosition, monster.playerNode.nodeCenterPosition, monster.monsterStat.CharactorAttackRange))
        {
            timer += Time.deltaTime;
            if(timer > (monster.monsterStat.attackSpeed*0.7f))
            {
                if (!readyAttack ) monster.ChangeAnim(name);
                readyAttack = true;
            }
            if (monster.monsterStat.attackSpeed <= timer)
            {
                if (Player.Instance.playerLevelInfo.stat.isCharacterDie) return;
                monster.monsterStat.OnClick(Player.Instance.playerLevelInfo.stat);
                timer = 0;
                monster.ChangeState(new MAttackState(monster));
                return;
            }
        }
        else
        {
            monster.ChangeState(new MIdleState(monster));
        }
    }
    public void Exit() 
    {
        timer = 0;
    }
}

public class MDamagedState : IState
{
    public string name { get { return "DamagedState"; } }
    float timer;
    float duration = 0.2f; 
    MonsterBase monster;
    public MDamagedState(MonsterBase monster)
    {
        this.monster = monster;
    }
    public void Enter()
    {
        timer = 0;
        monster.ChangeAnim(name);
    }
    public void Execute()
    {
        if (timer >= duration)
        {
            monster.ChangeState(new MIdleState(monster));
        }
    }
    public void Exit() 
    {
        timer = 0;
    }
}
public class MDieState : IState
{
    public string name { get { return "DieState"; } }
    MonsterBase monster;
    private float removeTime { get { return 2f; } }
    public float timer;
    public MDieState(MonsterBase monster)
    {
        this.monster = monster;
    }
    public void Enter()
    {
        timer = 0;
        monster.ChangeAnim(name);
        monster.monsterStat.HPBar = null;
    }
    public void Execute()
    {
        timer += Time.deltaTime;
        if (removeTime<= timer)
        {
            MonsterManager.GetInstance().AddRespawnList(monster);
            monster.CurrentNode = null;
            timer = 0;
        }
    }
    public void Exit() 
    {
        
    }
}
