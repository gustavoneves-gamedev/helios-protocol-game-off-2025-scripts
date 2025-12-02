using UnityEngine;
using UnityEngine.AI;

public class EnemyState
{

    protected EnemyBase enemyBase;
    protected EnemyStateMachine stateMachine;

    protected string animBoolName;
    protected float stateTimer;

    public bool triggerCalled;

    public EnemyState(EnemyBase enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;  

        this.animBoolName = animBoolName;
    }


    public virtual void Enter()
    {
        //Debug.Log("I enter " + animBoolName + " state!");

        triggerCalled = false;
    }

    public virtual void Update()
    {
        //Debug.Log("Im running " + animBoolName + " state!");
        stateTimer -= Time.deltaTime;

        
    }

    public virtual void Exit()
    {
        //Debug.Log("I exit " + animBoolName + " state!");
    }

    public void AnimationTrigger() => triggerCalled = true;
    //Professor usou a função acima, mas como não vou usar animator por enquanto, colocarei um pequeno contador
    //no RecoveryState mesmo de forma temporária

    public void PseudoAnimationTrigger()
    {
        if (stateTimer < 0)
        {
            triggerCalled = true;
        }
    }

    //Não está sendo utilizado! Vou tentar usar a rotação do próprio NavMeshAgent
    public Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = enemyBase.agent;
        NavMeshPath path = agent.path;

        if (path.corners.Length < 2)
            return agent.destination;

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
                return path.corners[i + 1];
        }

        return agent.destination;

    }


}
