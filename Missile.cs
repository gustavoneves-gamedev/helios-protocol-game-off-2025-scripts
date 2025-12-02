using Unity.VisualScripting;
using UnityEngine;

public enum MissileMode
{
    AerialDrop,     // Ataque 1: cai reto em direção ao alvo no chão
    ZoneSeeking,    // Ataque 2: sai na horizontal e vai curvando até uma zona
    Guided          // Ataque 3: segue um Transform (Player)
}

public class Missile : MonoBehaviour
{
    [Header("Geral")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float turnSpeed = 5f;

    [Header("Aerial")]
    [SerializeField] private float downForce = 0.5f; // se quiser adicionar um "peso" extra na queda

    [Header("Explosão")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float lifeTime = 10f; // segurança para Guided não ficarem eternos

    private MissileMode mode;

    private GameObject linkedWarning;

    // Alvo fixo no chão (usado em AerialDrop e ZoneSeeking)
    private Vector3 targetPos;

    // Alvo dinâmico (Guided)
    private Transform targetTransform;

    private bool initialized;
    private float currentLifeTime;

    private void OnEnable()
    {
        // Reset básico quando o objeto volta do pool
        initialized = false;
        targetTransform = null;
        currentLifeTime = 0f;
    }

    private void Update()
    {
        if (!initialized)
            return;

        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= lifeTime)
        {
            Explode(transform.position);
            return;
        }

        switch (mode)
        {
            case MissileMode.AerialDrop:
                UpdateAerialDrop();
                break;

            case MissileMode.ZoneSeeking:
                UpdateZoneSeeking();
                break;

            case MissileMode.Guided:
                UpdateGuided();
                break;
        }
    }

    #region Init Methods

    // Ataque 1: míssil já nasce lá em cima e cai verticalmente na posição do targetPos
    public void InitAerial(Vector3 groundTarget, float fallSpeed)
    {
        mode = MissileMode.AerialDrop;
        targetPos = groundTarget;
        speed = fallSpeed;
        initialized = true;
    }

    // Ataque 2: mira numa zona fixa no chão, mas começa indo na direção do launchPoint.forward
    public void InitZoneSeeking(Vector3 groundTarget, float moveSpeed, float turnSpeed, GameObject warning = null)
    {
        mode = MissileMode.ZoneSeeking;
        targetPos = groundTarget;
        speed = moveSpeed;
        this.turnSpeed = turnSpeed;

        linkedWarning = warning;

        initialized = true;
    }

    // Ataque 3: míssil teleguiado no Player (ou outro Transform)
    public void InitGuided(Transform target, float moveSpeed, float turnSpeed)
    {
        mode = MissileMode.Guided;
        targetTransform = target;
        speed = moveSpeed;
        this.turnSpeed = turnSpeed;
        initialized = true;
    }

    #endregion

    #region Update Modes

    private void UpdateAerialDrop()
    {
        // Mantém XZ travado na posição do alvo, só move no Y
        Vector3 pos = transform.position;
        pos.x = targetPos.x;
        pos.z = targetPos.z;

        pos.y -= speed * Time.deltaTime + downForce * Time.deltaTime;
        transform.position = pos;

        // Quando chegar no chão (ou abaixo do alvo), explode
        if (transform.position.y <= targetPos.y + 0.1f)
        {
            Explode(targetPos);
        }
    }

    private void UpdateZoneSeeking()
    {
        // Direção fixa para o alvo definido na inicialização
        Vector3 dir = (targetPos - transform.position);

        if (dir.sqrMagnitude <= 0.0001f)
        {
            Explode(targetPos);
            return;
        }

        Vector3 dirNorm = dir.normalized;

        // Movimento direto para o alvo
        transform.position += dirNorm * speed * Time.deltaTime;

        // (Opcional) rotaciona o modelo para olhar pro alvo
        if (dirNorm.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirNorm);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                turnSpeed * Time.deltaTime * 60f
            );
        }

        // Se chegou perto o suficiente, explode
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            Explode(targetPos);
        }
    }

    private void UpdateGuided()
    {
        if (targetTransform == null)
        {
            // se perder o alvo, explode na posição atual
            Explode(transform.position);
            return;
        }

        Vector3 dir = (targetTransform.position - transform.position);
        if (dir.sqrMagnitude <= 0.01f)
        {
            Explode(transform.position);
            return;
        }

        // Gira em direção ao alvo
        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            turnSpeed * Time.deltaTime * 60f
        );

        // Anda pra frente
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    #endregion

    #region Explosão / Colisão

    private void Explode(Vector3 explosionCenter)
    {
        // VFX (aqui você ainda está usando Instantiate, mas pode pôr no pool depois)
        if (explosionPrefab != null)
        {
            GameObject explosionVFX = Instantiate(explosionPrefab, explosionCenter, Quaternion.identity);
            Destroy(explosionVFX, 2f);
        }

        // Dano (OverlapSphere)
        Collider[] hits = Physics.OverlapSphere(explosionCenter, explosionRadius);
        foreach (Collider hit in hits)
        {
            
            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<Player>();
                player?.UpdateLife(-damage);
            }
        }

        // Se houver warning ligado, devolve pro pool
        if (linkedWarning != null)
        {
            ObjectPool.instance.ReturnObject(linkedWarning);
            linkedWarning = null;
        }

        // Volta pro Pool
        ObjectPool.instance.ReturnObject(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // aqui você pode filtrar (por ex. ignorar inimigos, só explodir no chão e no Player)
        // if (other.CompareTag("Enemy")) return;

        Explode(transform.position);
    }

    #endregion
}