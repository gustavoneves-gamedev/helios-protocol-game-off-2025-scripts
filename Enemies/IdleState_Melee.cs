using UnityEngine;

public class IdleState_Melee : EnemyState
{

    private Enemy_Melee enemy;
    
    public IdleState_Melee(EnemyBase enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemyBase.idleTime;
    }

    public override void Update()
    {
        base.Update();

        

        //Verifica se o player está ao alcance. Caso esteja, muda o estado de comportamento
        if (enemy.PlayerInAgressionRange())
        {
            stateMachine.ChangeState(enemy.recoveryState);
            return;
        }

        //Tira o inimigo do idle para começar a patrulhar o local
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

}
