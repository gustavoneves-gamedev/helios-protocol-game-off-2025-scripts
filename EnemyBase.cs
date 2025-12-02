using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private ParticleSystem deathExplosionFX;

    public float turnSpeed;
    //Velocidade de rotação do inimigo, vou ajustar conforme o tipo de inimigo e caso esteja ou não atacando

    public float aggresionRange;

    //[Header("Attack data")]
    //public float attackRange;
    //public float attackMoveSpeed;
    public float attackTime;
    //tempo que o inimigo leva atacando (provavelmente irei mudar isso quando fizer o inimigo de fato atacar)

    [Header("Idle info")]
    public float idleTime;

    [Header("Move data")]
    public float moveSpeed;
    public float chaseSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [Header("Recovery data")]
    public float recoveryTime;
    //Tempo que o inimigo leva no Recovery State

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public Transform player;// {  get; private set; }
    private Player playerScript;

    public NavMeshAgent agent { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }


    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        //player = GameObject.Find("Player").GetComponent<Player>();
    }

    protected virtual void Start()
    {
        //GetPlayerReference();
        InitializePatrolPoints();

        Invoke("GetPlayerReference", .1f);
        //Invoke("InitializePatrolPoints", .1f);

    }
    
    

    protected virtual void Update()
    {
        
    }

    private void GetPlayerReference()
    {
        player = GameController.gameController.player.transform; // GetComponent<Transform>();
        playerScript = player.GetComponent<Player>();                                                         
    }

    public void VampirismPlayerCure(float damageToLife)
    {
        playerScript?.UpdateLife(0, damageToLife);
    }

    public void OnDeathEvent()
    {
        //enemyScript.ReceiveDamage(999999);
        deathExplosionFX.transform.parent = gameObject.transform;
        ReturnPatrolPoints();
        ObjectPool.instance.ReturnObject(gameObject);
    }

    public void CreateDeathExplosionFx()
    {


        if (deathExplosionFX != null)
        {
            deathExplosionFX.transform.parent = null;

            deathExplosionFX.Play();
            deathExplosionFX.GetComponent<AudioSource>().Play();
        }

        Invoke("OnDeathEvent", 2f);
        gameObject.SetActive(false);

    }



    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);

    }


    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
    //Isso aqui serviria para acionar o trigger de animação que está no EnemyState a partir do Animator que está aqui
    //Coloquei só para manter similaridade, o meu está funcionando sem animação. Vide Script RecoveryState_Melee e
    //EnemyState para mais detalhes.
    //OBS: O prof fez um outro script para pegar este script aqui porque o componente Animator só consegue pegar events
    //que estejam no script do mesmo gameObject que ele (Vide aula 95 - min 17 para mais detalhes)

    //Serve para verificar se o player está dentro do alcance!
    public bool PlayerInAgressionRange()
    {
        if (player != null)
        {
            return Vector3.Distance(transform.position, player.transform.position) < aggresionRange;
            //Vai retornar o estado dessa conferência acima!!

        }
        else
            return false;


    }

    //A função abaixo retorna o próximo ponto de destino do inimigo. Explicação de como funciona abaixo:
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].position;
        //Ele pega a posição do primeiro destino no array de posições e o retorna

        currentPatrolIndex++;
        //Aqui eu acrescento mais ao index para retornar um outro ponto na próxima checagem

        //Aqui eu jogo o Index de volta para o primeiro transform do array quando o index passa do Length
        if (currentPatrolIndex >= patrolPoints.Length)
        {
            currentPatrolIndex = 0;
        }

        return destination;
    }

    //Serve apenas para tirar o parentesco dos patrol points do inimigo. Isso evita que os pontos se movimentem enquanto
    //o inimigo tenta alcançá-lo
    private void InitializePatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = null;
        }
    }

    private void ReturnPatrolPoints()
    {
        foreach (Transform t in patrolPoints)
        {
            t.parent = gameObject.transform;
        }
    }


    //Na teoria era para estar sendo utilizado no Update do Move, mas eu desativei! Vide script MoveState_Melee
    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        //Qual é a diferença entre usar apenas o Target no parâmetro acima e usar "target - transform.position"

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        //NÃO FAÇO IDEIA DO QUE ESTA FÓRMULA FAZ!! Pesquisar depois!

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);

    }

}
