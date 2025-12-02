using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public struct SelectingAttackData
{
    public string attackName;

    public float damage;

    public float attackMinRange;
    public float attackMaxRange;

    public float preferMin;
    public float preferMax;

    public float moveSpeed;
    public float attackSpeed;
    public float recoveryTime;
        
    //public float attackIndex;
    public AttackType_Melee attackType;
    public bool rotateDuring;
    public bool useAgentDuring;
    public float dashDistance;
    public float dashDuration;

}
public enum AttackType_Melee { Charge, Shoot, Flamethrower, Close }

public class Enemy_Melee : EnemyBase
{
            
    public IdleState_Melee idleState {  get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }

    [Header("Attack Data")]
    public SelectingAttackData attackData;
    public List<SelectingAttackData> attackList = new();

    [Header("Links")]
    public Enemy enemyScript;
    public GameObject flameControl; // { get; private set; }
    
    //public bool isDashing;
   


    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        enemyScript = GetComponentInChildren<Enemy>();
        //flameControl = GetComponentInChildren<Enemy>();
        


    }

    private void OnEnable()
    {
        //base.OnEnable();

        
    }

    protected override void Start()
    {
        base.Start();        

        stateMachine.Initialize(idleState);

    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

    }

    //O prof criou esta função para fazer o inimigo puxar a arma das costas quando entrar no AttackState!
    //Posso adaptar para fazer o inimigo começar a se preparar para atacar (Ex.: começa a sair mais chamas do lança-chamas
    //ou o inimigo que vai usar o Dash começa a brilhar na parte onde fica seu motor)
    public void PullWeapon()
    {
        //hiddenWeapon.gameObject.SetActive(false);
        //pulledWeapon.gameObject.SetActive(true);
    }

    public bool PlayerInAttackRange()
    {
        float d = Vector3.Distance(transform.position, player.position);
        return d >= attackData.attackMinRange && d <= attackData.attackMaxRange;

        //Verifica se o player está dentro do alcance do ataque inimigo

    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackMinRange);
    }

}
