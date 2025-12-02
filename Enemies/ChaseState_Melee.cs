using UnityEngine;

public class ChaseState_Melee : EnemyState
{
    
    private Enemy_Melee enemy;
    private float lastTimeUpdatedDestination;

    public ChaseState_Melee(EnemyBase enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.chaseSpeed;
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.agent.destination = enemy.player.position;

        bool anyAttackReady = AnyAttackInRange();

        // Enquanto ainda está longe demais pra qualquer ataque, ele pode usar míssil guiado
        if (!anyAttackReady && enemy.enemyScript.useGuidedMissileDuringChase)
        {
            enemy.enemyScript.TryShootGuidedMissile();
        }

        if (anyAttackReady)
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }


    }

    private bool CanUpdateDestination()
    {
        if (Time.time > lastTimeUpdatedDestination + .25f)
        {
            lastTimeUpdatedDestination = Time.time;
            return true;
        }

        return false;
    }

    private bool AnyAttackInRange()
    {
        float d = Vector3.Distance(enemy.transform.position, enemy.player.position);
        foreach (var a in enemy.attackList)
            if (d >= a.attackMinRange && d <= a.attackMaxRange)
                return true;
        return false;
    }


}
