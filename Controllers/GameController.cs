using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController gameController;
    public UIController uiController;
    public WaveController waveController;
    public RewardController rewardController;
    public ScoreController scoreController;
    public float points;
    public Boss boss;
    //public float life;

    [Header("Player info")]
    public GameObject player;
    public float playerMaxHealth;
    public float playerMaxHealthIncrement;
    public float playerMaxShield;
    public float playerMaxShieldIncrement;
    public float playerSpeed;
    public float playerSpeedIncrement;
    public float playerDamage;
    public float playerDamageIncrement;
    public float playerBulletSpeed;
    public float playerBulletSpeedIncrement;
    public float playerRange;
    public float playerRangeIncrement;

    [SerializeField] private int enemyQuantity;
    public bool isGamePaused;


    [Header("Wave info")]
    public int currentWave;
    [SerializeField] private int waveCounter;

    public float waveTimeRemaining;
    //Estou deixando público apenas para testes, depois irei usar o get; private set acima
    private bool waveHasBegun;


    private void Awake()
    {
        if (gameController == null)
            gameController = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);


    }

    #region Reset Game

    public void ResetGame()
    {
        // Pontos / combo / tempo
        points = 0;
        waveTimeRemaining = 0;
        currentWave = 0;

        // Flags gerais
        isGamePaused = false;

        // Increments de status do player (escala, dano etc)
        playerMaxHealthIncrement = 0;
        playerMaxShieldIncrement = 0;
        playerSpeedIncrement = 0;
        playerDamageIncrement = 0;
        playerBulletSpeedIncrement = 0;
        playerRangeIncrement = 0;

        // Se quiser, pode garantir valores base aqui também,
        // caso eles não venham do prefab:
        // playerMaxHealth = 100;
        // playerMaxShield = 100;
        // playerSpeed = 8;
        // ...

        // Zerar coisas internas do ScoreController (combo, rank, etc.)
        if (scoreController != null)
            scoreController.ResetScore();   // cria esse método lá

        // WaveController e Boss serão reatribuídos na nova cena
        waveController = null;
        rewardController = null;
        uiController = null;
        boss = null;
    }

    #endregion


    //Esta função é chamada no start do UI CONTROLLER inicialmente - Ler comentário em conjunto com o método seguinte
    //será chamada após o jogador escolher sua recompensa, ela irá somar mais 1 ao contador de ondas
    //e chamar a próxima onda de forma atualizada, ainda preciso testar se é melhor chamar esta ou a StartWave diretamente
    public void PseudoStart()
    {
        currentWave++;
        //Debug.Log("DEI PLAY NO START DO GAMECONTROLLER");
        uiController.UpdateTopRightWave(currentWave);

        Invoke("StartWave", 1f);

    }

    

    //Esta função será chamada após o jogador escolher sua recompensa, ela irá somar mais 1 ao contador de ondas
    //e chamar a próxima onda de forma atualizada  - Ler comentário em conjunto com o método anterior
    private void StartWave()
    {
        //currentWave++;
        //Por enquanto estou desabilitando aqui para não sobrepor a soma no método anterior

        if (waveController != null)
        {
            waveController.StartWave(currentWave);
        }

    }

    private void Update()
    {
        

        //GameCheat
        //if (Input.GetKeyDown(KeyCode.F12))
        //{
        //    WaveEnd();
        //}

        //PELO AMOR DE DEUS, ISTO É SÓ PARA TESTE!!
        //NÃO ESQUEÇA DE TIRAR ESTA BOSTA, SUA MULA!!!
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    GameController.gameController.uiController.WinWave();
        //}
    }

    //IPC!!! -> Melhorar isto! Impedir que o jogador escolha a recompensa caso o level mude!! Posso fazer
    //no RewardController RewardSelected()
    public void WaveEnd()
    {
        Debug.Log("Você venceu!");
        rewardController.AssignReward();
        uiController.WinWave();
    }

    public void AtualizarStatus(float pontos = 0)
    {
        //points += pontos;

        if (player != null && player.GetComponent<Player>().currentHealth <= 0)
        {
            Debug.Log("Você perdeu!");
            uiController.Defeat();
            //points = 0;
            //uiController.AtualizarPontosUI();
        }

        if(pontos != 0) 
            scoreController.IncrementCombo(pontos);

        uiController.UpdateUI();
    }

    public void UpdatePlayerStatus()
    {

    }


}
