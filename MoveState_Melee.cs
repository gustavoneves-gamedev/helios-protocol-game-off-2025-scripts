using UnityEngine;
using UnityEngine.AI;

public class MoveState_Melee : EnemyState
{

    private Enemy_Melee enemy;
    private Vector3 destination;
    
    public MoveState_Melee(EnemyBase enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        //Debug.Log("I entered move state!");

        enemy.agent.speed = enemy.moveSpeed;

        destination = enemy.GetPatrolDestination();

        enemy.agent.SetDestination(destination);
        //Define o alvo como sendo o destino definido no EnemyBase!
        //Esta função está embutida no NavMeshAgent!!
    }

    public override void Exit()
    {
        base.Exit();

        
    }

    public override void Update()
    {
        base.Update();

        

        //Verifica se o player está ao alcance. Caso esteja, muda o estado de comportamento
        if (enemy.PlayerInAgressionRange())
        {
            //Isso aqui não faz com que ele pare de andar! Ele vai andar porque no start foi definido um ponto de destino
            //para ele. Aqui eu mudo a rotação dele apenas. Quando o inimigo chegar ao destino, o destino não irá atualizar
            //e ele vai ficar parado olhando para o Player
            stateMachine.ChangeState(enemy.recoveryState);
            return;
        }

        
        //A função abaixo calcula a distância remanescente entre o inimigo e a posição deifnida como alvo.
        //Como os patrol points se atualizam quando o personagem o alcança, o destination muda de posição e força
        //o inimigo a se mover até a nova posição
        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + .05f)
            stateMachine.ChangeState(enemy.idleState);

        //enemy.agent.pathPending == false && -> isso aqui estava dentro do "if" acima

    }

    

}
