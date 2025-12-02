using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    //private EnemyBase enemyBase;
    private Enemy_Melee enemy_Melee;
    
    [Header("Atributes")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    private float baseMaxHealth;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float originalSpeed;

    [Header("Drop")]
    //[SerializeField] private GameObject dropPrefab;
    [SerializeField] private GameObject lifeUpPrefab;
    [SerializeField] private GameObject shieldUpPrefab;
    [SerializeField] private GameObject explosiveShotPrefab;
    

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform gunPointR;
    [SerializeField] private Transform gunPointL;
    [SerializeField] private float range;
    //[SerializeField] private float damage;
    [SerializeField] private float bulletSpeed;
    private float bulletImpactForce;
    [SerializeField] private bool canShoot;
    [SerializeField] private float defaultShotCooldown;
    [SerializeField] private float shotCooldown;

    [Header("References")]
    [SerializeField] private Transform target; // Player
    [SerializeField] private GameObject warningPrefab; // círculo vermelho
    [SerializeField] private GameObject missilePrefab; // míssil
    private GameObject lastWarning;
    [SerializeField] private Transform missileLaunchPoint1;
    [SerializeField] private Transform missileLaunchPoint2;

    [Header("Missile Attack")]
    [SerializeField] private float warningTime = 1.5f;   // tempo que a zona vermelha fica antes do impacto
    [SerializeField] private float missileStartHeight = 15f; // altura inicial do míssil
    [SerializeField] private float missileSpeed = 20f;   // velocidade de queda
    [SerializeField] private float attackCooldown = 5f;
    [SerializeField] private bool isAerialMissileAttack;
    [SerializeField] private Vector3 missileSpawnPos;
    [SerializeField] private float missileTurnSpeed = 5f;
    

    private bool canAttack = true;

    [Header("Guided Missile Attack")]
    [SerializeField] private Transform guidedGunPoint;      // onde o míssil guiado nasce
    [SerializeField] private float guidedMissileSpeed = 35f;
    [SerializeField] private float guidedMissileTurnSpeed = 1.5f;  // pouca rotação
    [SerializeField] private float guidedMissileCooldown = 3f;      // tempo entre mísseis guiados
    private float guidedMissileTimer;

    // Flags por inimigo (configura no Inspector por prefab)
    [Header("Missile Usage Flags")]
    public bool useZoneMissileDuringShoot;      // Ranger
    public bool useGuidedMissileDuringShoot;    // Dasher
    public bool useGuidedMissileDuringChase;    // FlameThrower

    [Header("Material Info")]
    [SerializeField] private Material gotHitMaterial;
    [SerializeField] private Material normalMaterial;
    public float visualTime = 0.2f;
    private MeshRenderer meshRenderer;

    [Header("Attack Info")]
    public bool isDashing;
    public bool wasDashDamageApplied;
    public bool isBurningPlayer;


    [Header("Defense Info")]
    public bool isShieldUp;
    [SerializeField] private float shieldCurrentLife;
    [SerializeField] private float shieldMaxLife;
    public bool isReflecting;
    //public float damageToLife;

    [Header("Status")]
    [SerializeField] private bool isBurning;
    private bool hasDropped = false;
    private bool hasUpdatedWave = false;
    private int burnCounter;
    [SerializeField] private bool isFreenzing;
    private int freenzingCounter;
    private float slowTime;

    [Header("Points & Dificult")]
    public float dificultMultiplier = 1f;
    [SerializeField] private int basePoints = 100;
    private bool isCompensatingTimeScale;


    private void Awake()
    {
        baseMaxHealth = maxHealth;   // salva o valor “de fábrica”
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        shieldCurrentLife = shieldMaxLife;
        meshRenderer = GetComponent<MeshRenderer>();
        normalMaterial = meshRenderer.material;
        originalSpeed = movementSpeed;

        target = GameController.gameController.player.transform;

        shotCooldown = defaultShotCooldown;
        //enemyBase = GetComponentInParent<EnemyBase>();
        enemy_Melee = GetComponentInParent<Enemy_Melee>();

        
    }

    /// <summary>
    /// CUIDADO COM CHAT, AINDA NÃO TESTEI!!!
    /// </summary>
    private void Update()
    {
        if (GameController.gameController.boss == null) return;

        if (GameController.gameController.boss.isUsingTimeControl || isCompensatingTimeScale == false)
        {
            movementSpeed *= 2;
            bulletSpeed *= 2;
            isCompensatingTimeScale = true;
        }

        if (GameController.gameController.boss.isUsingTimeControl == false || isCompensatingTimeScale == true)
        {
            movementSpeed /= 2;
            bulletSpeed /= 2;
            isCompensatingTimeScale = false;
        }

    }

    private void OnEnable()
    {
        hasDropped = false;
        hasUpdatedWave = false;

        int wave = Mathf.Max(1, GameController.gameController.currentWave);

        // Exemplo: +12% de vida por wave
        float healthMultiplier = 1f + 0.1f * (wave - 1);
        // ou algo mais suave: Mathf.Pow(1.08f, wave - 1);

        maxHealth = baseMaxHealth * healthMultiplier;
        currentHealth = maxHealth;

        shieldCurrentLife = shieldMaxLife;

        guidedMissileTimer = 0f; // reseta cooldown do míssil guiado

        var waveController = GameController.gameController?.waveController;
        if (waveController != null)
            waveController.enemyCounter++;   // registra que esse inimigo agora existe
    }

    private void OnDisable()
    {
        

        // Se o inimigo morrer ANTES do míssil assumir responsabilidade:
        if (lastWarning != null)
        {
            ObjectPool.instance.ReturnObject(lastWarning);
            lastWarning = null;
        }


        var wave = GameController.gameController?.waveController;
        if (wave != null && hasUpdatedWave == false)
        {
            wave.UpdateWaveEnemies();   // ou wave.enemyCounter--;
            hasUpdatedWave = true;
        }
    }

    // Cálculo do dano de Dash e do dano sofrido ao colidir com as lâminas do Player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            
            if (player != null && isDashing && !wasDashDamageApplied)
            {
                player.UpdateLife(-enemy_Melee.attackData.damage);
                wasDashDamageApplied = true;
            }            

        }

        if (other.gameObject.CompareTag("Blade"))
        {
            ReceiveDamage(GameController.gameController.player.GetComponent<PlayerPassiveSkills>().spinningBladeDamage);
        }
    }


    // Aplica o Queimando ao jogador (lança-chamas apenas)
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            
            if (player != null && isBurningPlayer)
            {
                player.UpdateLife(-enemy_Melee.attackData.damage);
                //wasDashDamageApplied = true;
                //Debug.Log("QUEIME DESGRAÇADO, QUEIMEEEEE!!!!");
                isBurningPlayer = false;
            }

        }
    }


    public void Fire()
    {
        if (canShoot)
        {
            GameObject newBullet1 = ObjectPool.instance.GetObject(bulletPrefab, gunPointR);
            GameObject newBullet2 = ObjectPool.instance.GetObject(bulletPrefab, gunPointL);

            //newBullet1.transform.position = GunPoint1().position;
            newBullet1.transform.rotation = Quaternion.LookRotation(gunPointR.forward);

            //newBullet2.transform.position = GunPoint2().position;
            newBullet2.transform.rotation = Quaternion.LookRotation(gunPointL.forward);

            Rigidbody rbNewBullet1 = newBullet1.GetComponent<Rigidbody>();
            Rigidbody rbNewBullet2 = newBullet2.GetComponent<Rigidbody>();

            EnemyBulletScript bulletScript1 = newBullet1.GetComponent<EnemyBulletScript>();
            bulletScript1.BulletSetup(range, enemy_Melee.attackData.damage, bulletImpactForce);

            EnemyBulletScript bulletScript2 = newBullet2.GetComponent<EnemyBulletScript>();
            bulletScript2.BulletSetup(range, enemy_Melee.attackData.damage, bulletImpactForce);

            rbNewBullet1.linearVelocity = gunPointR.forward * bulletSpeed;
            rbNewBullet2.linearVelocity = gunPointL.forward * bulletSpeed;

            //shotCooldown = defaultShotCooldown;
            //canShoot = false;

        }

    }

    
    #region Missile Attack

    public void TryStartZoneMissileAttack()
    {
        if (!useZoneMissileDuringShoot) return;   // só quem tiver essa flag true
        if (!canAttack || target == null) return;

        StartCoroutine(MissileAttack());
    }

    private IEnumerator MissileAttack()
    {
        canAttack = false;

        // 1) Escolhe o ponto alvo (posição do player, ajustada para o chão)
        Vector3 targetPos = GetGroundPositionNearTarget();

        // 2) Cria a zona vermelha no chão (usando Pool)
        lastWarning = ObjectPool.instance.GetObject(warningPrefab, transform);
        //warning.transform.position = targetPos;
        lastWarning.transform.position = new Vector3(targetPos.x, targetPos.y - 2.5f, targetPos.z);
        lastWarning.transform.rotation = Quaternion.Euler(90, 0, 0);

        //float originalTurnSpeed = enemy_Melee.turnSpeed;
        //enemy_Melee.turnSpeed = 0;

        // 3) Espera o tempo de aviso
        yield return new WaitForSeconds(warningTime);

        //enemy_Melee.turnSpeed = originalTurnSpeed;
        // enemy_Melee.agent.updateRotation = true;

        // 4) Spawn do míssil (ZoneSeeking) a partir do launchPoint
        Vector3 missileSpawnPos = missileLaunchPoint1.position;

        GameObject missileObj = ObjectPool.instance.GetObject(missilePrefab, missileLaunchPoint1);

        missileObj.transform.position = missileSpawnPos; 
        missileObj.transform.rotation = missileLaunchPoint1.rotation;

        Missile missile = missileObj.GetComponent<Missile>();
        if (missile != null)
        {
            // Ataque 2: vai curvando até a zona
            missile.InitZoneSeeking(targetPos, missileSpeed, missileTurnSpeed, lastWarning);
            lastWarning = null;
            //missile.InitZoneSeeking(warning.transform.position, missileSpeed, missileTurnSpeed);
        }

        // 5) Some com a zona vermelha (volta pro Pool)
        //ObjectPool.instance.ReturnObject(warning);

        // 6) Cooldown entre ataques de míssil normal
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private Vector3 GetGroundPositionNearTarget()
    {
        // Aqui você pode brincar:
        // - usar exatamente o target.position
        // - adicionar um pequeno random para não ser perfeito
        Vector3 pos = target.position;

        // Raycast para encaixar certinho no chão (caso o terreno seja irregular)
        Ray ray = new Ray(pos + Vector3.up * 10f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f))
        {
            pos = hit.point;
        }

        return pos;
    }

    public void TryShootGuidedMissile()
    {
        if (target == null) return;

        guidedMissileTimer -= Time.deltaTime;
        if (guidedMissileTimer > 0f)
            return; // ainda em cooldown

        guidedMissileTimer = guidedMissileCooldown;

        // Escolhe o ponto de spawn
        Transform spawn = guidedGunPoint != null ? guidedGunPoint : missileLaunchPoint1;

        GameObject missileObj = ObjectPool.instance.GetObject(missilePrefab, guidedGunPoint);
        missileObj.transform.position = spawn.position;
        missileObj.transform.rotation = spawn.rotation;

        Missile missile = missileObj.GetComponent<Missile>();
        if (missile != null)
        {
            // Pouca rotação = difícil mas não “cola” no player
            missile.InitGuided(target, guidedMissileSpeed, guidedMissileTurnSpeed);
        }
    }

    #endregion


    public void ReceiveDamage(float damage)
    {
        //Depois tenho que balancear isto, será que é melhor colocar proporções diferentes a depender se o dano
        // bateu no escudo ou direto na vida?
        GameController.gameController.AtualizarStatus(damage);
        
        //damageSuffered = damage;
        bool shieldAbsorded = false;

        if (isShieldUp == false)
        {
            currentHealth -= damage;
            //damageToLife = damage;
            enemy_Melee.VampirismPlayerCure(damage);

            if (currentHealth <= 0)
            {
                //Debug.Log("SEM ESCUDO MORTE");
                OnDeathEvent();
                return;
            }

            StopCoroutine(ChangeMaterialOnHit());
            StartCoroutine(ChangeMaterialOnHit());

            if (isBurning == true)
            {
                burnCounter = 3;
                StopCoroutine(ApplyBurn(0f));
                StartCoroutine(ApplyBurn(damage / 10f));
            }

            if (isFreenzing == true)
            {
                StopCoroutine(IsFreezing(0));
                StartCoroutine(IsFreezing(1));
            }

            //return;
        }

        if (isShieldUp == true && shieldCurrentLife > damage)
        {
            shieldCurrentLife -= damage;
            shieldAbsorded = true;
            //Colocar uma troca de material aqui para o escudo, algo azul piscando ao receber dano
            return;
        }

        if (isShieldUp == true && shieldAbsorded == false && shieldCurrentLife <= damage)
        {
            float remaningDamage = damage - shieldCurrentLife;
            shieldCurrentLife -= damage;
            //Colocar uma troca de material aqui para o escudo, algo azul piscando ao receber dano
            isShieldUp = false;
            currentHealth -= remaningDamage;
            //damageToLife = remaningDamage;
            //enemyBase.VampirismPlayerCure(remaningDamage);
            enemy_Melee.VampirismPlayerCure(remaningDamage);

            if (currentHealth <= 0)
            {
                //Debug.Log("COM ESCUDO MORTE");
                OnDeathEvent();
                return;
            }

            //Só mudar a cor da malha do inimigo em si se algum dano de fato passar pelo escudo
            StopCoroutine(ChangeMaterialOnHit());
            StartCoroutine(ChangeMaterialOnHit());

            if (isBurning == true)
            {
                burnCounter = 3;
                StopCoroutine(ApplyBurn(0f));
                StartCoroutine(ApplyBurn(damage / 10f));
            }

            if (isFreenzing == true)
            {
                StopCoroutine(IsFreezing(0));
                StartCoroutine(IsFreezing(1));
            }

            //return;
        }


    }

    private void OnDeathEvent()
    {
        
        
        StopAllCoroutines();
        isBurning = false;
        isFreenzing = false;
        
        int lootType = Random.Range(1, 100);

        Vector3 dropPosition = transform.position;

        if (lootType <= 50 && hasDropped == false)
        {
            //hasDropped = true;
        }

        if (lootType > 50 && lootType <= 75 && hasDropped == false)
        {
            GameObject drop = ObjectPool.instance.GetObject(lifeUpPrefab, transform);
            drop.transform.position = dropPosition;
            //drop.GetComponent<DropScript>().UpdateDropType(1);
            //gameObject.SetActive(false);
            //return;
            //hasDropped = true;
        }

        if (lootType > 75 && lootType <= 100 && hasDropped == false)
        {
            GameObject dropDrop = ObjectPool.instance.GetObject(shieldUpPrefab, transform);
            dropDrop.transform.position = dropPosition;
           // dropDrop.GetComponent<DropScript>().UpdateDropType(2);
            //gameObject.SetActive(false);
            //return;

        }

        

        hasDropped = true;

        GameController.gameController.AtualizarStatus(basePoints);
        StopAllCoroutines();

        //Tenho que reposicionar essas funções aqui embaixo, vou fazer um reparo temporário, mas vai ser melhor colcoar
        //no próprio WaveController - item 30 no meu caderno de bugs e melhorias!
        if (hasUpdatedWave == false)
        {
            //Debug.Log($"[DEATH] Enemy dying. Calling UpdateWaveEnemies. Before = {GameController.gameController.waveController.enemyCounter}");
            GameController.gameController.waveController.UpdateWaveEnemies();
        }
        //else
        //{
        //    Debug.Log("[DEATH] Enemy dying, but hasUpdatedWave already true (no decrement).");
        //}

        hasUpdatedWave = true;

        GameController.gameController.player.gameObject.GetComponent<PlayerPassiveSkills>().
            laserDetection?.UpdateEnemyArray(transform);

        //Chama o evento de morte no EnemyBase!!
        
        enemy_Melee.CreateDeathExplosionFx();
    }

    private IEnumerator ApplyBurn(float damage)
    {
        //posso também criar uma variável para aumentar o dano da queimadura
        if (burnCounter <= 0)
        {
            isBurning = false;
            StopCoroutine(ApplyBurn(0f));
        }

        if (isBurning == true)
        {
            currentHealth -= damage;
            StopCoroutine(ChangeMaterialOnHit());
            StartCoroutine(ChangeMaterialOnHit());
            burnCounter--;
        }

        yield return new WaitForSeconds(2f);//posso criar uma variável depois para aumentar a duração do fogo

        StartCoroutine(ApplyBurn(damage));

    }

    private IEnumerator IsFreezing(int frostPower)
    {
        //posso criar uma variável depois para congelar o cara mais rápido
        freenzingCounter += frostPower;
        movementSpeed = originalSpeed / 2;

        if (freenzingCounter >= 4)
        {
            movementSpeed = 0;
            freenzingCounter = 0;
            slowTime = 3f;
        }

        yield return new WaitForSeconds(slowTime);
        //posso criar uma variável depois para aumentar a duração do gelo

        movementSpeed = originalSpeed;
        slowTime = 2f;

    }

    private IEnumerator ChangeMaterialOnHit()
    {
        meshRenderer.material = gotHitMaterial;

        yield return new WaitForSeconds(visualTime);

        meshRenderer.material = normalMaterial;

    }

    

}
