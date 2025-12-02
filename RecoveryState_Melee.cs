using UnityEngine;

public class RecoveryState_Melee : EnemyState
{

    private Enemy_Melee enemy;

    public RecoveryState_Melee(EnemyBase enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.attackData.recoveryTime;

        enemy.enemyScript.isDashing = false;


        if (enemy.flameControl != null)
            enemy.flameControl.SetActive(false);

        if (!enemy.agent.enabled)
        {
            enemy.agent.enabled = true;            
            enemy.agent.isStopped = false;
            enemy.agent.updateRotation = true;
            enemy.agent.updatePosition = true;
        }
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (!enemy.enemyScript.isDashing)
            enemy.transform.rotation = enemy.FaceTarget(enemy.player.transform.position);


        //PseudoAnimationTrigger();
        //Estou usando essas duas funções em conjunto para manter a similaridade com o professor
        if (stateTimer < 0)
        {

            enemy.agent.speed = enemy.chaseSpeed;
            enemy.enemyScript.isDashing = false;
            enemy.transform.rotation = enemy.FaceTarget(enemy.player.transform.position);

            if (enemy.PlayerInAttackRange())
            {
                //Debug.Log("I'm changing to Attack State!!");
                stateMachine.ChangeState(enemy.attackState);
            }
            else
            {
                //Debug.Log("I'm changing to Chase State!!");
                stateMachine.ChangeState(enemy.chaseState);
            }
        }


    }
}
