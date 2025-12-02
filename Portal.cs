using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{

    [SerializeField] private GameObject dasher;
    [SerializeField] private GameObject ranger;
    [SerializeField] private GameObject flameThrower;
    [SerializeField] private WaveController waveController;
    //[SerializeField] private GameObject towerCannon;

    [Header("Spawn")]
    public bool isActive;    
    public int enemiesToSpawn;
    [SerializeField] private int enemyCounter;
    private float cooldownDefault = 5;
    //private float cooldown;
    [SerializeField] private bool isInitialSpawn;
    private bool isSpawning;

    [Header("VFX")]
    [SerializeField] private GameObject portalVFX;

    private WaveInfo waveInfo;
    private Coroutine spawnRoutine;

    
    void Start()
    {
        waveController = GetComponentInParent<WaveController>();
        //InitializePortal();

    }

    public void SetupWave(WaveInfo info)
    {
        waveInfo = info;
        InitializePortal();

    }

    public void InitializePortal()
    {
        enemiesToSpawn = (waveInfo != null) ? waveInfo.enemiesPerPortal : 5;
        enemyCounter = 0;
        isInitialSpawn = true;
        isSpawning = false;
        isActive = false;

        // Assim que o portal for ativado pelo WaveController,
        // começamos o ciclo de VFX + Spawn
        //StartSpawning();
    }

    public void ActivatePortal()
    {
        isActive = true;
        gameObject.SetActive(true);  // garante que está ativo
        StartSpawning();             // agora PODE começar coroutine
    }

    private void StartSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private System.Collections.IEnumerator SpawnLoop()
    {
        isSpawning = true;

        // --- Abre o portal (cresce) ---
        ActivatePortalVFX();

        // espera o portal "abrir" um pouquinho
        yield return new WaitForSeconds(0.12f);

        // --- Spawna inimigos até atingir o limite ---
        while (enemyCounter < enemiesToSpawn)
        {
            bool spawned = SpawnEnemy();
            if (!spawned)
                break;

            // 3 primeiros mais rápidos, depois cooldown normal (igual sua lógica)
            float delay = (enemyCounter < 3) ? 3f : cooldownDefault;
            if (enemyCounter >= 3) isInitialSpawn = false;

            yield return new WaitForSeconds(delay);
        }

        // --- Parou de spawna, agora fecha o portal ---
        isSpawning = false;
        isActive = false;
        HideVFX();

        // Avisar o WaveController que este portal terminou
        if (waveController != null)
        {
            waveController.TryEndWaveFromPortal();
        }
    }
    
    private void ActivatePortalVFX()
    {
        if (portalVFX == null)
        {
            Debug.LogError($"PortalVFX está nulo no Portal {name}");
            return;
        }

        //isSpawning = true;

        portalVFX.SetActive(true);

        LeanTween.cancel(portalVFX);
        portalVFX.transform.localScale = Vector3.one;

        LeanTween.scale(portalVFX, Vector3.one * 5.2f, 0.12f).setEaseOutQuad();
        //InvokeRepeating("SpawnEnemy", 2f, cooldown);
    }

    private void HideVFX()
    {
        if (portalVFX == null || portalVFX.activeSelf == false) return;

        isSpawning = false;

        LeanTween.cancel(portalVFX);
        portalVFX.transform.localScale = Vector3.one * 5.2f;

        LeanTween.scale(portalVFX, Vector3.one * .2f, 2f).setEaseOutQuad().setOnComplete(() => portalVFX.SetActive(false));
        //portalVFX.SetActive(false);
        //Invoke("DisablePortalVFX", 2f);

    }


    //Melhorar este método para usar um array!!
    private bool SpawnEnemy()
    {
        if (waveInfo == null)
        {
            Debug.LogWarning($"WaveInfo nulo no Portal {name}. Usando apenas Dasher.");
        }

        //GameObject enemyToSpawn = null;

        // Monta uma lista de tipos válidos desta onda
        var list = new System.Collections.Generic.List<GameObject>();

        if (waveInfo == null || waveInfo.useDasher) list.Add(dasher);
        if (waveInfo != null && waveInfo.useRanger) list.Add(ranger);
        if (waveInfo != null && waveInfo.useFlameThrower) list.Add(flameThrower);

        if (list.Count == 0)
        {
            Debug.LogWarning($"Nenhum tipo de inimigo habilitado neste portal ({name}).");
            return false;
        }

        // Escolhe um aleatório
        GameObject enemyToSpawn = list[Random.Range(0, list.Count)];

        ObjectPool.instance.GetObject(enemyToSpawn, transform);

        enemyCounter++;
        return true;
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        isSpawning = false;
    }



}
