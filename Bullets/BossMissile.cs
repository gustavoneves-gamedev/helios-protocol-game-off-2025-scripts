using System.Collections;
using UnityEngine;

public class BossMissile : MonoBehaviour
{
    public bool isAerialMissile;
    public Transform aerialMissileDropPoint;

    [SerializeField] private AudioSource audioSource;

    private Boss boss;
    private Transform target;

    [SerializeField] private Transform VFXTrail;

    [SerializeField] private GameObject warningArea;
    //private Transform defaultWarningAreaTransform;

    private Vector3 warningDefaultLocalPos;
    private Quaternion warningDefaultLocalRot;

    [SerializeField] private float missileSpeed;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Explosão")]
    [SerializeField] private ParticleSystem explosionPrefab;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float lifeTime = 10f;

    Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boss = GameController.gameController.boss;
        target = GameController.gameController.player.transform;
        rb = GetComponent<Rigidbody>();

        // Guarda a posição/rotação LOCAL originais do círculo
        warningDefaultLocalPos = warningArea.transform.localPosition;
        warningDefaultLocalRot = warningArea.transform.localRotation;
    }

    private void FixedUpdate()
    {
        if (isAerialMissile) return;
        ApplyRotation();
    }

    public void GuidedLaunch()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        VFXTrail.localScale /= 3f;

        audioSource.Play();

        missileSpeed = 10f;
        missileSpeed *= Random.Range(0.8f, 3.5f);

        //rb.linearVelocity = transform.forward * missileSpeed;
    }

    private void ApplyRotation()
    {
        if (target == null) return;

        // Direção atual e direção desejada
        Vector3 direction = (target.position - transform.position).normalized;

        // Calcula a rotação desejada
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Rotação suave
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
            rotationSpeed * Time.fixedDeltaTime);

        // Aplicar velocidade na frente do míssil
        rb.linearVelocity = transform.forward * missileSpeed;
    }


    public void AerialLaunch()
    {
        isAerialMissile = true;   // só pra garantir

        if (audioSource != null)
            audioSource.enabled = false;   // <-- DESLIGA O SOM

        rb = GetComponent<Rigidbody>();
        missileSpeed = 50f;
        rb.linearVelocity = transform.forward * missileSpeed;

        StartCoroutine(AerialDrop());
    }

    private IEnumerator AerialDrop()
    {
        yield return new WaitForSeconds(5f);


        transform.position = aerialMissileDropPoint.position;
        transform.rotation = aerialMissileDropPoint.rotation;
        rb.linearVelocity = transform.forward * missileSpeed;

        warningArea.transform.parent = null;

        // Se o dropPoint estiver no ar, força o Y no nível do chão (ajuste esse valor se necessário)
        Vector3 groundPos = aerialMissileDropPoint.position;
        groundPos.y = 1f; // ou a altura exata do seu chão

        warningArea.transform.position = groundPos;
        warningArea.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        warningArea.SetActive(true);


    }

    private void OnTriggerEnter(Collider other)
    {

        explosionPrefab.transform.parent = null;

        if (!isAerialMissile)
            explosionPrefab.Play();

        if (isAerialMissile)
        {
            explosionRadius = 10f;
            damage = 30f;
        }
        else
        {
            explosionRadius = 5f;
            damage = 20f;
        }


        // Dano (OverlapSphere)
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {

            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<Player>();
                player?.UpdateLife(-damage);
            }
        }

        if (isAerialMissile == false)
            VFXTrail.localScale *= 3f;

        warningArea.transform.parent = transform;
        warningArea.transform.localPosition = warningDefaultLocalPos;
        warningArea.transform.localRotation = warningDefaultLocalRot;
        warningArea.SetActive(false);


        Invoke("ReturnExplosionToParent", 2f);
        gameObject.SetActive(false);


    }

    private void ReturnExplosionToParent()
    {
        explosionPrefab.transform.parent = transform;
        boss.ReturnAerialMissileToPool(gameObject);
    }


}
