using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ScoreController;

public class UIController : MonoBehaviour
{
    
    [Header("HUD Top_Left")]
    public Slider lifeBar;
    public TextMeshProUGUI currentLife;
    public TextMeshProUGUI maxLife;
    public Slider shieldBar;
    public TextMeshProUGUI currentShield;
    public TextMeshProUGUI maxShield;
    public TextMeshProUGUI explosiveShots;
    private GameObject player;

    [Header("HUD Top_Middle")]
    public GameObject bossPanel;
    public Slider bossLifeBar;
    public TextMeshProUGUI bossCurrentLife;
    public TextMeshProUGUI bossMaxLife;
    public Slider bossShieldBar;
    //public TextMeshProUGUI bossCurrentShield;
    //public TextMeshProUGUI bossMaxShield;
    private Boss boss;

    [Header("HUD Top_Middle")]
    public GameObject tutorial0;
    public GameObject tutorial1;
    public GameObject tutorial2;
    public GameObject tutorial3;
    public GameObject tutorial4;

    [Header("Win/Defeat Screen")]
    public GameObject winMessage;
    public GameObject defeatMessage;

    [Header("RewardScreen")]
    public GameObject rewardPanel;

    [Header("Audio Info")]    
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider SFXVolume;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private bool isGamePaused;

    [Header("Player Status")]
    [SerializeField] private TextMeshProUGUI maxHealthStatus;
    [SerializeField] private TextMeshProUGUI maxHealthStatusIncrement;
    [SerializeField] private TextMeshProUGUI maxShieldStatus;
    [SerializeField] private TextMeshProUGUI maxShieldStatusIncrement;
    [SerializeField] private TextMeshProUGUI SpeedStatus;
    [SerializeField] private TextMeshProUGUI SpeedStatusIncrement;
    [SerializeField] private TextMeshProUGUI DamageStatus;
    [SerializeField] private TextMeshProUGUI DamageStatusIncrement;
    [SerializeField] private TextMeshProUGUI BulletSpeedStatus;
    [SerializeField] private TextMeshProUGUI BulletSpeedStatusIncrement;
    [SerializeField] private TextMeshProUGUI RangeStatus;
    [SerializeField] private TextMeshProUGUI RangeStatusIncrement;

    [Header("Score & Combo Info")]
    public TextMeshProUGUI waveText;        // WAVE 01
    public TextMeshProUGUI scoreWaveText;   // SCORE 0000
    public TextMeshProUGUI timeText;        // TIME 00:00
    public TextMeshProUGUI comboText;       // COMBO × 1.0
    public Image rankBadge;                  // ícone/cor por rank
    public TextMeshProUGUI rankMedal;

    public Sprite rankD, rankC, rankB, rankA, rankS, rankSS;
    // ou use uma única sprite e mude a cor

    public string D, C, B, A, S, SS;

      


    void Start()
    {
        GameController.gameController.uiController = this;

        Invoke("FirstTutorial", 1f);
        //AudioController.audioController.ChangeMasterVolume(volumeSlider.value);
        
    }

    #region First Tutorial

    private void FirstTutorial()
    {
        isGamePaused = true;
        GameController.gameController.isGamePaused = isGamePaused;

        if (tutorial0 == null) return;
        tutorial0.SetActive(true);
        UpdateStatus();
        var cg = tutorial0.GetComponent<CanvasGroup>();
        if (cg == null) cg = tutorial0.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        var rt = tutorial0.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.9f;

        LeanTween.value(tutorial0, 0f, 1f, 0.25f).setOnUpdate(a => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one, 0.25f).setEaseOutBack().setOnComplete(() => Time.timeScale = 0);
    }

    public void FirstNextTutorial()
    {
        AudioController.audioController.onClick.Play();
        var cg = tutorial0.GetComponent<CanvasGroup>();
        var rt = tutorial0.GetComponent<RectTransform>();
        LeanTween.value(tutorial0, cg.alpha, 0f, 0.2f).setOnUpdate((float a) => cg.alpha = a).setIgnoreTimeScale(true);
        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setIgnoreTimeScale(true).setOnComplete(() => tutorial0.SetActive(false));

        tutorial1.SetActive(true);
        var cg1 = tutorial1.GetComponent<CanvasGroup>();
        if (cg1 == null) cg1 = tutorial1.AddComponent<CanvasGroup>();
        cg1.alpha = 0f;
        var rt1 = tutorial1.GetComponent<RectTransform>();
        rt1.localScale = Vector3.one * 0.9f;

        LeanTween.value(tutorial1, 0f, 1f, 0.25f).setOnUpdate((float a) => cg1.alpha = a).setIgnoreTimeScale(true);
        LeanTween.scale(rt1, Vector3.one, 0.25f).setIgnoreTimeScale(true).setEaseOutBack();
    }

    public void EndFirstTutorial()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        GameController.gameController.isGamePaused = isGamePaused;

        CanvasGroup cg = tutorial1.GetComponent<CanvasGroup>();
        RectTransform rt = tutorial1.GetComponent<RectTransform>();

        LeanTween.value(tutorial1, 1f, 0f, 0.2f)
            .setOnUpdate(a => cg.alpha = a)
            .setOnComplete(() => tutorial1.SetActive(false));

        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setEaseInBack();

        InitiateGameController();
    }

    #endregion

    #region Second Tutorial
    public void SecondTutorial()
    {
        isGamePaused = true;
        GameController.gameController.isGamePaused = isGamePaused;

        tutorial2.SetActive(true);
        UpdateStatus();
        var cg = tutorial2.GetComponent<CanvasGroup>();
        if (cg == null) cg = tutorial2.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        var rt = tutorial2.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.9f;

        LeanTween.value(tutorial2, 0f, 1f, 0.25f).setOnUpdate(a => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one, 0.25f).setEaseOutBack().setOnComplete(() => Time.timeScale = 0);
    }

    public void EndSecondTutorial()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        GameController.gameController.isGamePaused = isGamePaused;

        CanvasGroup cg = tutorial2.GetComponent<CanvasGroup>();
        RectTransform rt = tutorial2.GetComponent<RectTransform>();

        LeanTween.value(tutorial2, 1f, 0f, 0.2f)
            .setOnUpdate(a => cg.alpha = a)
            .setOnComplete(() => tutorial2.SetActive(false));

        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setEaseInBack();

        //InitiateGameController();
    }

    #endregion

    public void InitiateGameController()
    {
        GameController.gameController.PseudoStart();
    }

    private void Initialize()
    {
        player = GameController.gameController.player;
       

        UpdateUI();

    }
    


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    #region Score and Time

    public void UpdateTopRightWave(int wave)
    {
        if (waveText == null) return;

        waveText.text = $"WAVE {wave:00}";
    }

    public void UpdateTopRightScoreWave(int score)
    {
        if (scoreWaveText == null) return;

        scoreWaveText.text = $"SCORE {score:n0}";
    }

    public void UpdateTopRightTimer(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        timeText.text = $"TIME {m:00}:{s:00}";
    }

    public void UpdateTopRightCombo(float multiplier, StyleRank rank, float comboValue)
    {

        //Debug.Log(rank);
        //comboText.text = $"COMBO × {multiplier:0.0}";
        //rankBadge.sprite = rank switch
        //{
        //    StyleRank.SS => rankSS,
        //    StyleRank.S => rankS,
        //    StyleRank.A => rankA,
        //    StyleRank.B => rankB,
        //    StyleRank.C => rankC,
        //    _ => rankD
        //};

        comboText.text = $"COMBO × {multiplier:0.0}";
        rankMedal.text = rank switch
        {
            StyleRank.SS => SS,
            StyleRank.S => S,
            StyleRank.A => A,
            StyleRank.B => B,
            StyleRank.C => C,
            _ => D
        };

        // (opcional) animar badge conforme comboValue (escala/pulso)
        // StartCoroutine(Pulse(rankBadge.rectTransform));

    }
    #endregion

    public void UpdateUI()
    {
        if (player == null || AudioController.audioController == null)
        {
            Invoke("Initialize", .02f);            
            return;
        }
        
        lifeBar.value = player.GetComponent<Player>().currentHealth / player.GetComponent<Player>().maxHealth;
        currentLife.text = player.GetComponent<Player>().currentHealth + "/";
        maxLife.text = player.GetComponent<Player>().maxHealth+"";

        //MUDAR A COR DA BARRA DE VIDA PARA VERMELHO QUANDO ESTIVER ABAIXO DE 30% !!!
        //if (lifeBar.value < 0.3f)
        //{
        //    lifeBar.
        //}

        shieldBar.value = player.GetComponent<Player>().currentShield / player.GetComponent<Player>().maxShield;
        currentShield.text = player.GetComponent<Player>().currentShield + "/";
        maxShield.text = player.GetComponent<Player>().maxShield + "";

        boss = GameController.gameController.boss;

        if (boss == null) return;


        bossPanel.SetActive(true);
        bossLifeBar.value = boss.currentHealth / boss.maxHealth;
        bossCurrentLife.text = boss.currentHealth + "/";
        bossMaxLife.text = boss.maxHealth + "";

        bossShieldBar.value = boss.shieldCurrent / boss.shieldMax;

        //explosiveShots.text = $"x {player.GetComponent<Player>().explosiveShots}";
    }

    #region Status

    public void UpdateStatus()
    {
        maxHealthStatus.text = "" + 
            (GameController.gameController.playerMaxHealth/10f + GameController.gameController.playerMaxHealthIncrement)/5;
        
        maxHealthStatusIncrement.text = "(+" + (GameController.gameController.playerMaxHealthIncrement/5)+")";

        
        maxShieldStatus.text = "" + 
            (GameController.gameController.playerMaxShield/10f + GameController.gameController.playerMaxShieldIncrement)/5;

        maxShieldStatusIncrement.text = "(+" + (GameController.gameController.playerMaxShieldIncrement)/5+")";


        SpeedStatus.text = ""+ 
            (GameController.gameController.playerSpeed/8f + GameController.gameController.playerSpeedIncrement)*2;

        SpeedStatusIncrement.text = "(+" + (GameController.gameController.playerSpeedIncrement)*2 + ")";


        DamageStatus.text = "" + 
            (GameController.gameController.playerDamage + GameController.gameController.playerDamageIncrement)/2;

        DamageStatusIncrement.text = "(+" + (GameController.gameController.playerDamageIncrement)/2 + ")";

        BulletSpeedStatus.text = "" + 
            (GameController.gameController.playerBulletSpeed/2 + GameController.gameController.playerBulletSpeedIncrement);

        BulletSpeedStatusIncrement.text = "(+" + GameController.gameController.playerBulletSpeedIncrement + ")";


        RangeStatus.text = "" + 
            (GameController.gameController.playerRange/3 + GameController.gameController.playerRangeIncrement);

        RangeStatusIncrement.text = "(+" + GameController.gameController.playerRangeIncrement + ")";
    }


    #endregion


    #region Pause Menu

    public void PauseGame()
    {
        AudioController.audioController.onClick.Play();

        if (!isGamePaused)
        {
            ShowPause();
        }
        else if (isGamePaused)
        {
            HidePause();
        }        

    }

    public void ShowPause()
    {
        isGamePaused = true;
        GameController.gameController.isGamePaused = isGamePaused;

        pauseMenu.SetActive(true);
        UpdateStatus();
        var cg = pauseMenu.GetComponent<CanvasGroup>();
        if (cg == null) cg = pauseMenu.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        var rt = pauseMenu.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.9f;

        LeanTween.value(pauseMenu, 0f, 1f, 0.25f).setOnUpdate(a => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one, 0.25f).setEaseOutBack().setOnComplete(() => Time.timeScale = 0);

        
    }

    public void HidePause()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        GameController.gameController.isGamePaused = isGamePaused;

        CanvasGroup cg = pauseMenu.GetComponent<CanvasGroup>();
        RectTransform rt = pauseMenu.GetComponent<RectTransform>();

        LeanTween.value(pauseMenu, 1f, 0f, 0.2f)
            .setOnUpdate(a => cg.alpha = a)
            .setOnComplete(() => pauseMenu.SetActive(false));

        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setEaseInBack();

        
    }

    public void OptionsMenu()
    {
        //pausePanel.SetActive(false);
        //optionsPanel.SetActive(true);

        CanvasGroup cg = pausePanel.GetComponent<CanvasGroup>();
        RectTransform rt = pausePanel.GetComponent<RectTransform>();

        //LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setEaseInBack();
        LeanTween.value(pausePanel, 1f, .15f, .2f)
            .setOnUpdate(a => cg.alpha = a).setIgnoreTimeScale(true)
            .setOnComplete(() => pausePanel.SetActive(true));
        //ATENÇÃO!!! Se eu for mexer nisto, é só mudar aqui acima de true para false e descomentar a linha anterior!!!
              

        optionsPanel.SetActive(true);        
        var cg2 = optionsPanel.GetComponent<CanvasGroup>();
        if (cg2 == null) cg2 = optionsPanel.AddComponent<CanvasGroup>();
        cg2.alpha = 0f;
        var rt2 = optionsPanel.GetComponent<RectTransform>();
        rt2.localScale = Vector3.one * 0.9f;

        LeanTween.value(optionsPanel, 0f, 1f, .3f).setOnUpdate((float b) => cg2.alpha = b).setIgnoreTimeScale(true);
        LeanTween.scale(rt2, Vector3.one, .6f).setEaseOutBack().setIgnoreTimeScale(true);


    }   


    public void BackToPauseMenu()
    {
        //optionsPanel.SetActive(false);
        //pausePanel.SetActive(true);

        CanvasGroup cg = optionsPanel.GetComponent<CanvasGroup>();
        RectTransform rt = optionsPanel.GetComponent<RectTransform>();

        LeanTween.value(optionsPanel, 1f, 0f, 0.2f)
            .setOnUpdate(a => cg.alpha = a).setIgnoreTimeScale(true)
            .setOnComplete(() => optionsPanel.SetActive(false));

        LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setEaseInBack();

        //ClickPunch(btnOptions);
        pausePanel.SetActive(true);
        var cg2 = pausePanel.GetComponent<CanvasGroup>();
        if (cg2 == null) cg2 = pausePanel.AddComponent<CanvasGroup>();
        cg2.alpha = 0f;
        var rt2 = pausePanel.GetComponent<RectTransform>();
        rt2.localScale = Vector3.one * 0.9f;

        LeanTween.value(pausePanel, 0f, 1f, 0.25f).setOnUpdate((float b) => cg2.alpha = b).setIgnoreTimeScale(true);
        LeanTween.scale(rt2, Vector3.one, 0.25f).setEaseOutBack().setIgnoreTimeScale(true);
    }

    public void ResumeGame()
    {
        HidePause();
    }

     // === UX: hover e click ===
    public void OnHoverEnter(RectTransform rt)
    {
        AudioController.audioController.onHover.Play();
        LeanTween.scale(rt, Vector3.one * 1.04f, 0.12f).setEaseOutQuad().setIgnoreTimeScale(true);
    }
    public void OnHoverExit(RectTransform rt)
    {
        //PlayHover();
        LeanTween.scale(rt, Vector3.one, 0.12f).setEaseOutQuad().setIgnoreTimeScale(true);
    }

    public void ClickPunch(RectTransform rt)
    {

        AudioController.audioController.onClick.Play();
        LeanTween.scale(rt, Vector3.one * 0.96f, 0.08f).setEaseOutQuad().setLoopPingPong(1);
    }

    #endregion

    public void PlayGame()
    {
        GameController.gameController.points = 0;
        //GameController.gameController.life = 100;
        //Isto é temporário, a vida será puxada do Player no jogo real!!!

        GameController.gameController.ResetGame();

        GameController.gameController.currentWave = 0;
        //Isto serve para zerar a Onda atual quando vou da tela de Menu, Vitória ou Derrota para o jogo em si!
        SceneManager.LoadScene("GameScene1");
    }

    //TEMPORÁRIO!! TENHO QUE ATUALIZAR ISTO!
    public void WinWave()
    {

        //GameController.gameController.rewardController.AssignReward();

        if (GameController.gameController.currentWave > 10)
        {
            //Invoke("WinScreen", 4f);
            //SceneManager.LoadScene("Victory");

            WinScreen();
        }

    }

    public void WinGame()
    {
        isGamePaused = true;
        GameController.gameController.isGamePaused = isGamePaused;

        winMessage.SetActive(true);
        UpdateStatus();
        var cg = winMessage.GetComponent<CanvasGroup>();
        if (cg == null) cg = winMessage.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        var rt = winMessage.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.9f;

        LeanTween.value(winMessage, 0f, 1f, 0.25f).setOnUpdate(a => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one, 0.25f).setEaseOutBack().setOnComplete(() => Time.timeScale = 0);
    }

    public void MainMenu()
    {
        HidePause();

        if (winMessage.activeSelf == true)
        {
            CanvasGroup cg = winMessage.GetComponent<CanvasGroup>();
            RectTransform rt = winMessage.GetComponent<RectTransform>();

            LeanTween.value(winMessage, 1f, 0f, 0.2f)
                .setOnUpdate(a => cg.alpha = a).setIgnoreTimeScale(true)
                .setOnComplete(() => winMessage.SetActive(false));

            LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setEaseInBack();
        }

        if (defeatMessage.activeSelf == true)
        {
            CanvasGroup cg = defeatMessage.GetComponent<CanvasGroup>();
            RectTransform rt = defeatMessage.GetComponent<RectTransform>();

            LeanTween.value(defeatMessage, 1f, 0f, 0.2f)
                .setOnUpdate(a => cg.alpha = a).setIgnoreTimeScale(true)
                .setOnComplete(() => defeatMessage.SetActive(false));

            LeanTween.scale(rt, Vector3.one * 0.9f, 0.2f).setEaseInBack();
        }

        GameController.gameController.player.SetActive(true);

        SceneManager.LoadScene("MainMenu");
    }

    public void DeathScreen()
    {
        //SceneManager.LoadScene("Defeat");
        //isGamePaused = true;
       // GameController.gameController.isGamePaused = isGamePaused;

        defeatMessage.SetActive(true);
        UpdateStatus();
        var cg = defeatMessage.GetComponent<CanvasGroup>();
        if (cg == null) cg = defeatMessage.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        var rt = defeatMessage.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * 0.9f;

        LeanTween.value(defeatMessage, 0f, 1f, 0.25f).setOnUpdate(a => cg.alpha = a);
        LeanTween.scale(rt, Vector3.one, 0.25f).setEaseOutBack().setOnComplete(() => Time.timeScale = 1);
    }

    //Isto só existe para a entreda da build
    private void WinScreen()
    {
        //SceneManager.LoadScene("Victory");
    }

    public void Defeat()
    {
        //SceneManager.LoadScene("Defeat");

    }

    public void Quit()
    {
        Debug.Log("Saí do jogo!");
        Application.Quit();
    }
}
