using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Header("Configuração por Onda")]
    public WaveInfo[] waves;   // <<< NOVO

    private WaveInfo currentWaveInfo;

    [Header("Wave info")]
    public int enemyCounter;
    [SerializeField] private int portalsToActivate;
    [SerializeField] private int currentPortals = 0;
    //[SerializeField] private int remainigPortals;
    //[SerializeField] private int currentWave;
    private float waveTimer;
    //public float waveTimeRemaining { get; private set; }
    public float waveTimeRemaining;
    //Estou deixando público apenas para testes, depois irei usar o get; private set acima
    public int waveResult;

    [Header("Tempo entre portais extras")]
    [SerializeField] private float extraPortalInterval = 2f; // segundos


    [SerializeField] private Portal[] portals;

    [SerializeField] private GameObject boss;


    
    void Start()
    {
        portals = GetComponentsInChildren<Portal>(true);
        GameController.gameController.waveController = this;
        

        //ActivatePortal();
        //InvokeRepeating("ActivatePortal", 3f, 1f);
    }    

    public void StartWave(int currentWave)
    {
        enemyCounter = 0;
        currentPortals = 0;
        extraPortalInterval += (currentWave/4f);

        if (extraPortalInterval < 0.1f)
        {
            extraPortalInterval = 1f;
        }

        int currentWaveIndex = currentWave - 1;

        if (currentWave == 7)
        {
            boss.SetActive(true);
            this.enabled = false; // <--- ESSENCIAL!
            DisablePortals();     // limpa portais da wave anterior
            return;
        }

        // Proteção para não estourar o array
        if (currentWaveIndex >= waves.Length)
            currentWaveIndex = waves.Length - 1;

        currentWaveInfo = waves[currentWaveIndex];

        portalsToActivate = currentWaveInfo.portalsToActivate;
        currentPortals = 0;

        // Informar cada portal sobre a Wave atual
        foreach (var portal in portals)
            portal.SetupWave(currentWaveInfo);

        // --------- NOVO FLUXO DE ATIVAÇÃO ---------

        // 1) Ativa até 3 portais imediatamente
        int initialBatch = Mathf.Min(3, portalsToActivate);
        for (int i = 0; i < initialBatch; i++)
        {
            ActivatePortal();
        }

        // 2) Se ainda faltarem portais, ativa 1 por vez a cada alguns segundos
        if (currentPortals < portalsToActivate)
        {
            CancelInvoke(nameof(ActivatePortal)); // só pra garantir que não ficou nada anterior
            InvokeRepeating(nameof(ActivatePortal), extraPortalInterval, extraPortalInterval);
        }
    }




    private void ActivatePortal()
    {
        //GameController.gameController.AtualizarStatus();
        
        if (currentPortals >= portalsToActivate)
        {
            CancelInvoke(nameof(ActivatePortal));
            return;
        }        
        
        int portalIndex = Random.Range(0, portals.Length);

        if (portals[portalIndex].gameObject.activeSelf)
        {
            return;
        }
        else
        {
            portals[portalIndex].InitializePortal();
            portals[portalIndex].ActivatePortal();   // ativa + inicia coroutine            
            currentPortals++;
        }
        
    }


    //Esta função impede que a Wave acabe antes da hora caso o Player mate os inimigos spanawdos muito rápido
    //antes de o portal terminar de spawnar todo mundo
    private bool CheckActivePortals()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            if (portals[i].isActive == true)
            {
                return true;
            }
            //portals[i].gameObject.SetActive(false);
        }

        return false;
    }

    public void UpdateWaveEnemies()
    {
        enemyCounter--;
        //Debug.Log($"[WAVE] Enemy removed. enemyCounter = {enemyCounter}");
        if (enemyCounter < 0)
            enemyCounter = 0;

        if (enemyCounter > 0)
            return;

        if (CheckActivePortals() == true)
        {
            return;
        }

        //Debug.Log("[WAVE] All enemies dead and no active portals. Ending wave.");
        //Depois devo separar o fim da wave da atualização de inimigos para ficar mais simples de acabar a onda
        //apenas sobrevivendo o tempo da wave
        WaveEnd();
    }

    public void TryEndWaveFromPortal()
    {
        // Garante que enemyCounter não seja negativo
        if (enemyCounter < 0)
            enemyCounter = 0;

        // Se ainda há inimigos, não encerra
        if (enemyCounter > 0)
            return;

        // Se ainda existe portal ativo, não encerra
        if (CheckActivePortals())
            return;

        // Nenhum inimigo vivo e nenhum portal ativo -> pode encerrar a wave
        WaveEnd();
    }

    private void WaveEnd()
    {
        DisablePortals();
        Debug.Log("A ONDA ACABOU!!");
        GameController.gameController.WaveEnd();
    }

    private void DisablePortals()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].isActive = false;

            //Acho que não preciso dessa linha abaixo porque estou usando o gameObject.activeSelf para verificar
            portals[i].gameObject.SetActive(false);

            //portals[i].
        }
    }

}
