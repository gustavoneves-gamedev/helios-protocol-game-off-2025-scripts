using UnityEngine;

public class Player : MonoBehaviour
{
    public InputSystem_Actions inputActions;
    private PlayerActiveSkills activeSkills;

    [SerializeField] private GameObject bigExplosion;

    [Header("Status")]
    public float currentHealth;
    public float maxHealth;
    private float amountToHeal;
    public float currentShield;
    public float maxShield;
    private float amountToRestore;
    public bool isInvulnerable = false;

    [Header("Resources Info")]
    public int explosiveShots;

    [Header("Secondary Weapon Info")]
    public Transform currentSecondaryWeapon;


    private void Start()
    {
        GameController.gameController.player = gameObject;
        currentHealth = maxHealth;
        currentShield = maxShield;
        isInvulnerable = false;
        activeSkills = GetComponent<PlayerActiveSkills>();

        //Atualiza os Status do Player VIDA e ESCUDO
        GameController.gameController.playerMaxHealth = maxHealth;
        GameController.gameController.playerMaxShield = maxShield;

        InvokeRepeating("InitializeStatus", .1f, .1f);


    }

    private void InitializeStatus()
    {
        if (GameController.gameController.uiController == null)
        {

            return;
        }

        CancelInvoke("InitializeStatus");

        GameController.gameController.AtualizarStatus();
    }    

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

    }

    #region Life Updates
    public void UpdateLifeOverTime(float totalHeal)
    {
        amountToHeal += totalHeal;
        InvokeRepeating("HealingApplyOverTime", 1.5f, .15f);
        
    }

    private void HealingApplyOverTime()
    {
        //float amountHealed = 0;
        UpdateLife(1f);
        amountToHeal--;

        if (amountToHeal <= 0)
        {
            CancelInvoke("HealingApplyOverTime");
        }
    }

    //Atualiza a vida do jogador ao pegar uma poção de cura (drop), também será chamada quando o jogador sofrer dano
    public void UpdateLife(float life = 0f, float vampirismCure = 0f)
    {
        
        
        if (life < 0f && currentShield > 0f)
        {
            UpdateShield(life/2);
            return;
        }
        
        
        //Isso afeta o buff de vida recebido caso o jogador esteja em modo berserker!!
        if (activeSkills.isInBerserkerMode)
        {
            if (life > 0) // Reduz a cura recebida
            {
                life /= activeSkills.healthPenalty;
            }
            else // Aumenta o dano sofrido
            {
                life *= activeSkills.healthPenalty;
            }
        }

        if (activeSkills.isInVampirismMode)
        {
            currentHealth += (vampirismCure *= activeSkills.vampirismAbsorb);
        }

        if (isInvulnerable == false)
            currentHealth += life;

        if (currentHealth < 0f)
        {
            currentHealth = 0f;
            isInvulnerable = true;
            OnDeathEvent();
        }

        if (currentHealth > maxHealth) 
            currentHealth = maxHealth;

        if (activeSkills.isInLazarusMode)
        {
            activeSkills.LazarusUpdateDamage();
        }

        if (life < 0)
            GameController.gameController.AtualizarStatus(-life);
        else
            GameController.gameController.AtualizarStatus(0);

    }

    private void OnDeathEvent()
    {
        bigExplosion.SetActive(true);
        gameObject.SetActive(false);
        

        Invoke("Death", 1f);
    }

    private void Death()
    {
        GameController.gameController.uiController.DeathScreen();
    }

    //Atualiza a vida do jogador ao pegar um Buff de Vida
    public void UpdateMaxLife(float life)
    {
        maxHealth += life;
        currentHealth += life;
        GameController.gameController.playerMaxHealthIncrement += life;
        GameController.gameController.AtualizarStatus(0);
    }

    #endregion


    #region Shield Updates
    public void UpdateShieldOverTime(float totalHeal)
    {
        amountToRestore += totalHeal;
        InvokeRepeating("RestoringApplyOverTime", 1.5f, .15f);

    }

    private void RestoringApplyOverTime()
    {
        //float amountHealed = 0;
        UpdateShield(1f);
        amountToRestore--;

        if (amountToRestore <= 0)
        {
            CancelInvoke("RestoringApplyOverTime");
        }
    }

    //Atualiza o shield do jogador ao pegar um shield (drop), também será chamada quando o jogador sofrer dano
    public void UpdateShield(float shield)
    {
        currentShield += shield;

        if (currentShield <= 0)
        {
            UpdateLife(currentShield);
            currentShield = 0;
        }

        if(currentShield > maxShield)
            currentShield = maxShield;

        GameController.gameController.AtualizarStatus(0);
    }

    //Atualiza o shield do jogador ao pegar um Buff de Shield
    public void UpdateMaxShield(float shield)
    {
        maxShield += shield;
        currentShield += shield;
        GameController.gameController.playerMaxShieldIncrement += shield;
        GameController.gameController.AtualizarStatus(0);
    }

    #endregion

    public void UpdateExplosiveShots(int explosive)
    {
        explosiveShots += explosive;
        GameController.gameController.AtualizarStatus(0);
    }


    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


}
