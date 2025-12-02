using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Boss : MonoBehaviour
{
    //private EnemyBase enemyBase;
    //private Enemy_Melee enemy_Melee;
    [SerializeField] private Animator anim;

    [Header("Atributes")]
    public float currentHealth;
    public float maxHealth;
    public bool isShieldUp;
    public float shieldCurrent;
    public float shieldMax;
    //public bool isReflecting;
    [SerializeField] private GameObject shield;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float originalSpeed;
    [SerializeField] private int stage = 1;
    [SerializeField] private Transform target; // Player

    //[SerializeField] private GameObject warningPrefab; // círculo vermelho
    [Header("Aerial Missile Attack")]
    [SerializeField] private GameObject bossMissilePrefab; // míssil    
    [SerializeField] private Transform[] aerialMissileLaunchPoints;
    [SerializeField] private Transform[] dropPoints;
    [SerializeField] private List<GameObject> missileInstances = new List<GameObject>();
    [SerializeField] private float cooldownBetweenMissiles = 1f;
    [SerializeField] private float cooldownBetweenAttacks = 20f;
    [SerializeField] private bool canDoAerialAttack = false;

    [Header("Missile Attack")]
    [SerializeField] private Transform[] guidedMissileLaunchPointsLeft;
    [SerializeField] private Transform[] guidedMissileLaunchPointsRight;
    [SerializeField] private float cooldownBetweenGuidedMissiles;
    private int guidedMissileCounter;

    [Header("Spawn Enemies")]
    [SerializeField] private GameObject portalVFX;
    [SerializeField] private Portal[] bossPortals;
    private int spawnCounter;

    [Header("Time Control")]
    public bool isUsingTimeControl;
    private bool canUseTimeControl;
    public bool isTimeSlow;
    [SerializeField] private GameObject slowOverlay;

    //[Header("Material Info")]
    //[SerializeField] private Material gotHitMaterial;
    //[SerializeField] private Material normalMaterial;
    //public float visualTime = 0.2f;
    //private MeshRenderer meshRenderer;

    [Header("Attack Info")]
    [SerializeField] private int attacksQuantityPerTime = 1;
    [SerializeField] private int attacksBeingPerformed;
    [SerializeField] private float flameDamage;
    private bool isBurningPlayer;


    [Header("Status")]
    private bool hasUpdatedWave = false;
    private float slowTime;

    [Header("Explosions")]
    [SerializeField] private GameObject bigExplosion1;
    [SerializeField] private GameObject bigExplosion2;
    [SerializeField] private GameObject bigExplosion3;
    [SerializeField] private GameObject bigExplosion4;
    [SerializeField] private GameObject bigExplosion5;
    

    [Header("Points & Dificult")]
    public float dificultMultiplier = 1f;
    [SerializeField] private int basePoints = 10000;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameController.gameController.boss = this;
        //anim = GetComponentInChildren<Animator>();
        if (anim == null)
            anim = GetComponent<Animator>();


        currentHealth = maxHealth;
        shieldCurrent = 2000f;
        //meshRenderer = GetComponent<MeshRenderer>();
        //normalMaterial = meshRenderer.material;
        originalSpeed = movementSpeed;

        StageOne();
        //AttackSelection();
        //target = GameController.gameController.player.transform;

    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        //hasDropped = false;
        hasUpdatedWave = false;
        currentHealth = maxHealth;
        shieldCurrent = 1000f;

        //guidedMissileTimer = 0f; // reseta cooldown do míssil guiado

        var wave = GameController.gameController?.waveController;
        if (wave != null)
            wave.enemyCounter++;   // registra que esse inimigo agora existe
    }

    private void OnDisable()
    {


        var wave = GameController.gameController?.waveController;
        if (wave != null && hasUpdatedWave == false)
        {
            wave.UpdateWaveEnemies();   // ou wave.enemyCounter--;
            hasUpdatedWave = true;
        }
    }

    private void OnStageChange()
    {
        stage++;
        

        guidedMissileCounter = 0;
        spawnCounter = 0;

        canDoAerialAttack = true;
        canUseTimeControl = true;

        if (stage == 2)
        {
            shieldCurrent = 3000f;
            isShieldUp = true;
            shield.SetActive(true);
            StageTwo();
        }

        if (stage == 3)
        {
            shieldCurrent = shieldMax;
            isShieldUp = true;
            shield.SetActive(true);
            StageThree();
        }

    }

    #region Stage Selection

    private void StageOne()
    {
        if (stage == 1)
            StartCoroutine(AttackSelectionStageOne());
    }

    private IEnumerator AttackSelectionStageOne()
    {
        //int stageOneCycles

        yield return new WaitForSeconds(5f);

        SpawnEnemies();
        spawnCounter++;

        yield return new WaitForSeconds(20f);

        GuidedMissileAttack();
        guidedMissileCounter++;

        yield return new WaitForSeconds(10f);

        if (spawnCounter >= 2)
        {
            canDoAerialAttack = true;
            AerialAttack();
            spawnCounter = 0;
        }

        StageOne();

    }

    private void StageTwo()
    {
        if (stage == 2)
            StartCoroutine(AttackSelectionStageTwo());
    }

    private IEnumerator AttackSelectionStageTwo()
    {
        yield return new WaitForSeconds(5f);

        SpawnEnemies();
        spawnCounter++;

        yield return new WaitForSeconds(15f);

        GuidedMissileAttack();
        guidedMissileCounter++;


        if (spawnCounter >= 2)
        {
            canDoAerialAttack = true;
            spawnCounter = 0;
        }

        StageTwo();
    }

    private void StageThree()
    {
        if (stage == 3)
            StartCoroutine(AttackSelectionStageThree());
    }

    private IEnumerator AttackSelectionStageThree()
    {
        if (spawnCounter == 0)
        {
            yield return new WaitForSeconds(5f);

            for (int i = 0; i < bossPortals.Length; i++)
            {
                bossPortals[i].gameObject.SetActive(false);
            }
            
            SpawnEnemies();
            canDoAerialAttack = true;
            spawnCounter++;
            yield return new WaitForSeconds(5f);
            TimeSlowAttack();
        }            

        yield return new WaitForSeconds(5f);

        canDoAerialAttack = true;
        AerialAttack();
        

        yield return new WaitForSeconds(15f);

        GuidedMissileAttack();
        guidedMissileCounter++;


        StageThree();

    }

    #endregion

    #region Boss Attacks

    #region Missile Sequence

    public void GuidedMissileAttack()
    {
        anim.SetTrigger("MissileAttack");
        anim.speed = .5f;
    }

    public void FirstLaunch()
    {
        StartCoroutine(FirstLeftHatch());
        StartCoroutine(FirstRightHatch());
    }

    public void SecondLaunch()
    {
        StartCoroutine(SecondLeftHatch());
        StartCoroutine(SecondRightHatch());
    }

    public void ThirdLaunch()
    {
        StartCoroutine(ThirdLeftHatch());
        StartCoroutine(ThirdRightHatch());
    }

    public void FourthLaunch()
    {
        StartCoroutine(FourthLeftHatch());
        StartCoroutine(FourthRightHatch());
    }

    public void FifthLaunch()
    {
        StartCoroutine(FifthLeftHatch());
        StartCoroutine(FifthRightHatch());
    }


    #region Left Launchers

    private IEnumerator FirstLeftHatch()
    {
        for (int i = 0; i < 0.1f + (stage/3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsLeft[0];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }

    private IEnumerator SecondLeftHatch()
    {
        for (int i = 0; i < 0.1f + (stage / 3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsLeft[1];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }

    private IEnumerator ThirdLeftHatch()
    {
        for (int i = 0; i < 0.1f + (stage / 3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsLeft[2];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }

    private IEnumerator FourthLeftHatch()
    {
        for (int i = 0; i < 0.1f + (stage / 3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsLeft[3];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }

    private IEnumerator FifthLeftHatch()
    {
        for (int i = 0; i < 0.1f + (stage / 3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsLeft[4];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }

    #endregion

    #region Right Launchers
    private IEnumerator FirstRightHatch()
    {
        for (int i = 0; i < 0.1f + (stage / 3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsRight[0];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }

    private IEnumerator SecondRightHatch()
    {
        for (int i = 0; i < 0.1f + (stage / 3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsRight[1];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }

    private IEnumerator ThirdRightHatch()
    {
        for (int i = 0; i < 0.1f + (stage / 3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsRight[2];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }

    private IEnumerator FourthRightHatch()
    {
        for (int i = 0; i < 0.1f + (stage / 3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsRight[3];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }

    private IEnumerator FifthRightHatch()
    {
        for (int i = 0; i < 0.1f + (stage / 3); i++)
        {
            Transform missileSpawnPoint = guidedMissileLaunchPointsRight[4];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = false;
            //bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.GuidedLaunch();
            yield return new WaitForSeconds(cooldownBetweenGuidedMissiles);
        }
    }


    #endregion

    #endregion

    #region TimeSlow

    private void TimeSlowAttack()
    {
        if (stage != 3) return;

        anim.SetTrigger("FlameGates");
        anim.speed = .5f;
    }

    public void SlowTime()
    {
        isUsingTimeControl = true;
        anim.speed = .1f;

        if(slowOverlay != null) slowOverlay.SetActive(true);

        Time.timeScale = .5f;
        GameController.gameController.player.GetComponent<PlayerMovement>().moveSpeed /= 1.25f;
        GameController.gameController.player.GetComponent<PlayerWeaponController>().bulletSpeed /= 1.25f;
    }

    public void ResumeTime()
    {
        isUsingTimeControl = false;
        anim.speed = .5f;

        if (slowOverlay != null) slowOverlay.SetActive(false);

        Time.timeScale = 1f;
        GameController.gameController.player.GetComponent<PlayerMovement>().moveSpeed *= 1.25f;
        GameController.gameController.player.GetComponent<PlayerWeaponController>().bulletSpeed *= 1.25f;
    }

    #endregion

    #region Spawn Enemy

    public void SpawnEnemies()
    {
        //ActivatePortalVFX();
        anim.SetTrigger("SpawnEnemy");
        anim.speed = .5f;
        attacksBeingPerformed++;
    }

    public void ActivateBossPortals()
    {
        StartCoroutine(PortalsActivated());
    }

    private IEnumerator PortalsActivated()
    {
        anim.speed = 0f;
        SpawnPortal();
        yield return new WaitForSeconds(5f);
        anim.speed = .5f;
    }


    public void SpawnPortal()
    {
        for (int i = 0; i < (2 * stage); i++)
        {
            Portal portal = bossPortals[Random.Range(0, bossPortals.Length)];
            if (!portal.gameObject.activeSelf)
            {
                portal.InitializePortal();
                portal.ActivatePortal();
            }
        }

    }

    public void CallAerialMissileAttack()
    {
        if (stage == 1) return;
        AerialAttack();
    }

    public void ActivatePortalVFX()
    {
        if (portalVFX == null)
        {
            Debug.LogError($"PortalVFX está nulo no Portal {name}");
            return;
        }

        //isSpawning = true;

        portalVFX.SetActive(true);

        LeanTween.cancel(portalVFX);
        portalVFX.transform.localScale = Vector3.one * 0.5f;

        LeanTween.scale(portalVFX, Vector3.one * 3f, 2f).setEaseOutQuad();
        //AJUSTAR O TEMPO PARA BATER COM A ANIMAÇÃO!!
    }

    public void HideVFX()
    {
        if (portalVFX == null || portalVFX.activeSelf == false) return;

        LeanTween.cancel(portalVFX);
        portalVFX.transform.localScale = Vector3.one * 3f;

        LeanTween.scale(portalVFX, Vector3.one * .2f, 2f).setEaseOutQuad().setOnComplete(() => portalVFX.SetActive(false));
        //portalVFX.SetActive(false);
        //Invoke("DisablePortalVFX", 2f);
        attacksBeingPerformed--;

    }

    #endregion

    #region Aerial Missile Attack

    public void AerialAttack()
    {
        if (!canDoAerialAttack) return;

        anim.SetTrigger("AerialAttack");
        anim.speed = .5f;
    }

    //AERIAL ATTACK ANIMATION VAI TRIGGAR ESTE EVENTO!!
    public void AerialMissileCall()
    {
        attacksBeingPerformed++;

        StartCoroutine(AerialMissileAttack());
    }

    private IEnumerator AerialMissileAttack()
    {
        canDoAerialAttack = false;

        //missileInstances.Clear();

        //PAUSE AERIAL ATTACK ANIMATION          
        anim.speed = 0f;

        for (int i = 0; i < (40 * stage); i++)
        {
            Transform missileSpawnPoint = aerialMissileLaunchPoints[Random.Range(0, aerialMissileLaunchPoints.Length)];
            GameObject missileInstance = ObjectPool.instance.GetObject(bossMissilePrefab, missileSpawnPoint);
            missileInstances.Add(missileInstance);
            missileInstance.transform.rotation = missileSpawnPoint.rotation;
            BossMissile bossMissileScript = missileInstance.GetComponent<BossMissile>();
            bossMissileScript.isAerialMissile = true;
            bossMissileScript.aerialMissileDropPoint = dropPoints[Random.Range(0, dropPoints.Length)];
            bossMissileScript.AerialLaunch();
            yield return new WaitForSeconds(cooldownBetweenMissiles);
        }


        anim.speed = .5f;
        //RESUME AERIAL ATTACK ANIMATION 
        attacksBeingPerformed--;
        yield return new WaitForSeconds(cooldownBetweenAttacks);



        canDoAerialAttack = true;

    }

    public void ReturnAerialMissileToPool(GameObject missileInstance)
    {
        missileInstances.Remove(missileInstance);
        ObjectPool.instance.ReturnObject(missileInstance);


    }

    #endregion




    //// Aplica o Queimando ao jogador (lança-chamas apenas)
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        Player player = other.gameObject.GetComponent<Player>();


    //        if (player != null && isBurningPlayer)
    //        {
    //            player.UpdateLife(-flameDamage);

    //            isBurningPlayer = false;
    //        }

    //    }
    //}

    #endregion

    #region Receive Damage
    // Cálculo do dano ao colidir com as lâminas do Player
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Blade"))
        {
            //Debug.Log("FUI CHAMADO!!");
            ReceiveDamage(GameController.gameController.player.GetComponent<PlayerPassiveSkills>().spinningBladeDamage);
        }
    }

    public void ReceiveDamage(float damage)
    {

        Debug.Log($"DANO: {damage}, Vida antes: {currentHealth}, Escudo: {shieldCurrent}, isShieldUp: {isShieldUp}");
        //Depois tenho que balancear isto, será que é melhor colocar proporções diferentes a depender se o dano
        // bateu no escudo ou direto na vida?
        GameController.gameController.AtualizarStatus(damage);

        //damageSuffered = damage;
        bool shieldAbsorded = false;

        if (isShieldUp == false)
        {
            currentHealth -= damage;

            if (currentHealth < (maxHealth * 2 / 3f) && stage == 1)
                OnStageChange();

            if (currentHealth < (maxHealth * 1 / 3f) && stage == 2)
                OnStageChange();
            //damageToLife = damage;
            //enemy_Melee.VampirismPlayerCure(damage);

            if (currentHealth <= 0)
            {
                //Debug.Log("SEM ESCUDO MORTE");
                OnDeathEvent();
                return;
            }

            //StopCoroutine(ChangeMaterialOnHit());
            //StartCoroutine(ChangeMaterialOnHit());
            GameController.gameController.uiController.UpdateUI();

        }

        if (isShieldUp == true && shieldCurrent > damage / 2)
        {
            shieldCurrent -= damage / 2;
            shieldAbsorded = true;
            //Colocar uma troca de material aqui para o escudo, algo azul piscando ao receber dano
            GameController.gameController.uiController.UpdateUI();
            return;
        }

        if (isShieldUp == true && shieldAbsorded == false && shieldCurrent <= damage / 2)
        {
            float remaningDamage = (damage / 2) - shieldCurrent;
            shieldCurrent -= damage / 2;
            //Colocar uma troca de material aqui para o escudo, algo azul piscando ao receber dano
            isShieldUp = false;
            shield.SetActive(false);
            currentHealth -= remaningDamage;

            if (currentHealth < (maxHealth * 2 / 3f) && stage == 1)
                OnStageChange();

            if (currentHealth < (maxHealth * 1 / 3f) && stage == 2)
                OnStageChange();

            //damageToLife = remaningDamage;
            //enemyBase.VampirismPlayerCure(remaningDamage);
            //enemy_Melee.VampirismPlayerCure(remaningDamage);

            if (currentHealth <= 0)
            {
                //Debug.Log("COM ESCUDO MORTE");
                OnDeathEvent();
                return;
            }

            //Só mudar a cor da malha do inimigo em si se algum dano de fato passar pelo escudo
            //StopCoroutine(ChangeMaterialOnHit());
            //StartCoroutine(ChangeMaterialOnHit());

            //return;
            GameController.gameController.uiController.UpdateUI();
        }


    }

    private void OnDeathEvent()
    {


        //StopAllCoroutines();



        GameController.gameController.AtualizarStatus(basePoints);

        GameController.gameController.player.GetComponent<Player>().isInvulnerable = true;

        Time.timeScale = .3f;        

        StartCoroutine(ChainExplosions());

        

        ////Tenho que reposicionar essas funções aqui embaixo, vou fazer um reparo temporário, mas vai ser melhor colcoar
        ////no próprio WaveController - item 30 no meu caderno de bugs e melhorias!
        //if (hasUpdatedWave == false)
        //{
        //    //Debug.Log($"[DEATH] Enemy dying. Calling UpdateWaveEnemies. Before = {GameController.gameController.waveController.enemyCounter}");
        //    GameController.gameController.waveController.UpdateWaveEnemies();
        //}
        ////else
        ////{
        ////    Debug.Log("[DEATH] Enemy dying, but hasUpdatedWave already true (no decrement).");
        ////}

        //hasUpdatedWave = true;

        //GameController.gameController.player.gameObject.GetComponent<PlayerPassiveSkills>().
        //    laserDetection?.UpdateEnemyArray(transform);

        

        //Chama o evento de morte no EnemyBase!!

        //enemy_Melee.CreateDeathExplosionFx();
    }

    private IEnumerator ChainExplosions()
    {
        bigExplosion1.SetActive(true);

        yield return new WaitForSeconds(.2f);

        bigExplosion2.SetActive(true);

        yield return new WaitForSeconds(.2f);

        bigExplosion3.SetActive(true);

        //yield return new WaitForSeconds(.2f);

        bigExplosion4.SetActive(true);

        yield return new WaitForSeconds(.2f);

        bigExplosion5.SetActive(true);        

        yield return new WaitForSeconds(.2f);

        gameObject.SetActive(false);

        WinGame();
    }

    private void WinGame()
    {
        StopAllCoroutines();

        GameController.gameController.uiController.WinGame();
    }

    //private IEnumerator ChangeMaterialOnHit()
    //{
    //    meshRenderer.material = gotHitMaterial;

    //    yield return new WaitForSeconds(visualTime);

    //    meshRenderer.material = normalMaterial;

    //}

    #endregion
}
