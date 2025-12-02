using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{

    public float impactForce;

    private BoxCollider cd;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;
        
    private Vector3 startPosition;
    public float damage = 2f;
    public float range;
    private bool bulletDisabled;
    
    [SerializeField] private GameObject explosionRadius;
    [SerializeField] private GameObject bulletImpactFX;
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private GameObject smokeFX;
    //[SerializeField] private bool isMissile;

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    

    //Aqui eu estou ativando a bala antes de criá-la e dispará-la através do WeaponController
    //O range virá como input porque vai depender da arma equipada!
    public void BulletSetup(float range, float damage, float impactForce)
    {
        this.impactForce = impactForce;
        
        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.time = .25f;
        }

        startPosition = transform.position;
        this.range = range; //+ 1
        this.damage = damage;

    }

    
    void Update()
    {
        FadeTrailIfNeeded();

        DisableBulletIfNeeded();

        ReturnToPoolIfNeeded();

    }
    
    //Esta função serve para desabilitar a bala caso ela percorra uma distância muito longa
    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > range && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    //Serve para retornar a bala ao Pool caso o trail renderer chegue a zero. Trabalha em conjunto com a função
    //DisableBulletIfNeeded para retornar a bala ao Pool (a função de cima só a desabilita)
    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }

    //Retorna a bala ao Pool
    private void ReturnBulletToPool()
    {
        ObjectPool.instance.ReturnObject(gameObject);
    }
       
    //Reduz o rastro de forma mais natural no caso de armas de curto alcance, sem parar do nada dando a impressão de bug
    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > range - 1.5f)
        {
            trailRenderer.time -= 2 * Time.deltaTime; //Devo testar esse valor 2 depois
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        Player player = collision.gameObject.GetComponent<Player>();
        
        //CreateImpactFx();

        //if (explosionRadius != null)
        //{
        //   GameObject explosion = ObjectPool.instance.GetObject(explosionRadius, transform);
        //    //explosion.transform.position = transform.position;
        //    ObjectPool.instance.ReturnObject(explosion, .5f);

        //    CreateExplosionFx();            
        //}

        
        if (player != null)
        {
                        
            player.UpdateLife(-damage);            
            
        }

        ReturnBulletToPool();

        
    }   


    private void CreateImpactFx() //Collision collision -> Isso estava dentro do parênteses!
    {

        GameObject newImpactFx = ObjectPool.instance.GetObject(bulletImpactFX, transform);
        ObjectPool.instance.ReturnObject(newImpactFx, 1);

    }

    private void CreateExplosionFx()
    {
        GameObject newExplosionFx = ObjectPool.instance.GetObject(explosionFX, transform);
        GameObject newSmokeFx = ObjectPool.instance.GetObject(smokeFX, transform);

        ObjectPool.instance.ReturnObject(newExplosionFx, 1);
        ObjectPool.instance.ReturnObject(newSmokeFx, 1);

    }   


}
