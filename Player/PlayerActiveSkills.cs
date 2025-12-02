using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerActiveSkills : MonoBehaviour
{
    private Player player;
    private PlayerWeaponController weaponController;
    private PlayerMovement playerMovement;
    public Volume volume;           // arrasta o Volume do inspector
    private Vignette vignette;
    [SerializeField] private Transform IconCanvas;


    //Será representada com a tela ficando meio avermelhada e pulsando
    [Header("Beserker Skill")]
    public bool hasBerserkerSkill;
    public int berserkerLevel;
    public bool isInBerserkerMode;
    private float berserkerCooldown;
    private float berserkerDefaultCooldown = 20f;//por padrão será 120f, FALTA BALANCEAR!!
    private bool isBeserkerInCooldown = false;
    public float healthPenalty = 2f; //Depois tenho que fazer um public get, private set aqui
    private float damageBoost = 2f;
    private float speedBoost = 1.5f;
    private float berserkerDuration = 15f;
    [SerializeField] private Image beserkerIcon;
    private float berserkerPulseTime = 3f;
    [SerializeField] private Color berserkerIconDefaultColor;
    private bool hasBerserkerIconColorReset = true;

    //Será representada com um símbolo de vampiro sobre o Player
    [Header("Vampirism Skill")]
    public bool hasVampirismSkill;
    public int vampirismLevel;
    public bool isInVampirismMode;
    private float vampirismCooldown;
    private float vampirismDefaultCooldown = 20f; //por padrão será 180f, FALTA BALANCEAR!!
    private bool isVampirismInCooldown = false;
    public float vampirismAbsorb = 0.1f;
    private float vampirismDuration = 15f;
    [SerializeField] private Image vampirismIcon;
    private float vampirismPulseTime = 3f;
    [SerializeField] private Color vampirismIconDefaultColor;
    private bool hasVampirismIconColorReset = true;

    [Header("Lazarus Skill")]
    public bool hasLazarusSkill;
    public int lazarusLevel;
    public bool isInLazarusMode;
    private float lazarusCooldown;
    private float lazarusDefaultCooldown = 20f; //por padrão será 180f, FALTA BALANCEAR!!
    private bool isLazarusInCooldown;
    private bool wasJustActivated = true;
    private float resetCoeficient = 1f;
    private float lazarusBuff = 1f;
    private float lazarusDuration = 15f;
    [SerializeField] private Image lazarusIcon;
    private float lazarusPulseTime = 3f;
    [SerializeField] private Color lazarusIconDefaultColor;
    private bool hasLazarusIconColorReset = true;



    void Start()
    {
        player = GetComponent<Player>();
        weaponController = GetComponent<PlayerWeaponController>();
        playerMovement = GetComponent<PlayerMovement>();

        //actions = player.inputActions;
        AssignInputEvents();
        //BlinkAnimation(beserkerIcon);


        if (volume.profile.TryGet(out vignette))
        {
            // garante referência
        }
    }

    // Update is called once per frame
    void Update()
    {
        SkillsCooldowns();

    }

    private void SkillsCooldowns() //Depois farei um check para ver se o jogador tem ou não a skill (ou não)
    {
        if (isBeserkerInCooldown)
        {
            berserkerCooldown -= Time.deltaTime;
            beserkerIcon.fillAmount = 1 - (berserkerCooldown / berserkerDefaultCooldown);

            if (berserkerCooldown < 0)
            {
                isBeserkerInCooldown = false;
                beserkerIcon.fillAmount = 1;
                berserkerPulseTime = 3f;
            }
        }
        else if (berserkerPulseTime >= 0)
        {
            berserkerPulseTime -= Time.deltaTime;
            BlinkAnimation(beserkerIcon);
            hasBerserkerIconColorReset = false;
        }
        else if (!hasBerserkerIconColorReset)
        {
            beserkerIcon.color = berserkerIconDefaultColor;
            hasBerserkerIconColorReset = true;
        }


        if (isVampirismInCooldown)
        {
            vampirismCooldown -= Time.deltaTime;
            vampirismIcon.fillAmount = 1 - (vampirismCooldown / vampirismDefaultCooldown);

            if (vampirismCooldown < 0)
            {
                isVampirismInCooldown = false;
                vampirismIcon.fillAmount = 1;
                vampirismPulseTime = 3f;
            }
        }
        else if (vampirismPulseTime >= 0)
        {
            vampirismPulseTime -= Time.deltaTime;
            BlinkAnimation(vampirismIcon);
            hasVampirismIconColorReset = false;
        }
        else if (!hasVampirismIconColorReset)
        {
            vampirismIcon.color = vampirismIconDefaultColor;
            hasVampirismIconColorReset = true;
        }



        if (isLazarusInCooldown)
        {
            lazarusCooldown -= Time.deltaTime;
            lazarusIcon.fillAmount = 1 - (lazarusCooldown / lazarusDefaultCooldown);

            if (lazarusCooldown < 0)
            {
                isLazarusInCooldown = false;
                lazarusIcon.fillAmount = 1;
                lazarusPulseTime = 3f;
            }
        }
        else if (lazarusPulseTime >= 0)
        {
            lazarusPulseTime -= Time.deltaTime;
            BlinkAnimation(lazarusIcon);
            hasLazarusIconColorReset = false;
        }
        else if (!hasLazarusIconColorReset)
        {
            lazarusIcon.color = lazarusIconDefaultColor;
            hasLazarusIconColorReset = true;
        }


    }

    private void BlinkAnimation(Image skillIcon)
    {

        float alpha = Mathf.Lerp(0.5f, 1f, Mathf.PingPong(Time.time * 1f, 1f));
        Color c = skillIcon.color;
        c.a = alpha;
        skillIcon.color = c;
    }

    #region Berserker Skill

    private void ActivateBerserkerSkill()
    {
        if (isBeserkerInCooldown == true || isInBerserkerMode)
        {
            Debug.Log("BerserkerSkill in Cooldown!!");
            //Tocar um som de erro
            return;
        }

        isInBerserkerMode = true;
        weaponController.damage *= damageBoost;
        //Tem um problema aqui, se estiver no modo berserker e mudar para o tiro explosivo, a skill não irá afetar o dano
        //do tiro explosivo e vice-versa. Possíveis soluções: 1 - Fazer o dano do tiro explosivo ser multiplicativo (o que é
        //uma boa solução porque aí o tiro explosivo sempre será útil para o jogador). 2 - criar uma função no update ou
        //no método de troca de tiro que detecte a mudança de tiro e desative e ative o modificador de dano do Berserker
        //(poderia colocar neste script ou no da mudança de tiro mesmo)

        playerMovement.moveSpeed *= speedBoost;
        //Isso aumenta a velocidade do player enquanto estiver no modo berserker

        SetBerserkerFX(isInBerserkerMode);

        //Invoke("DeactivateBerserkerSkill", 3f); // o método de baixo é a fórmula correta, mas vou usar esta para testes
        //LEMBRAR DE TROCAR DEPOIS, SUA BESTA!!!
        Invoke("DeactivateBerserkerSkill", berserkerDuration);

    }

    public void SetBerserkerFX(bool active)
    {
        if (vignette == null) return;

        vignette.color.Override(Color.red);
        vignette.intensity.Override(active ? 0.45f : 0f);
        vignette.smoothness.Override(0.6f);
    }

    private void DeactivateBerserkerSkill()
    {
        isInBerserkerMode = false;
        weaponController.damage /= damageBoost;
        playerMovement.moveSpeed /= speedBoost;

        isBeserkerInCooldown = true;
        berserkerCooldown = berserkerDefaultCooldown;

        SetBerserkerFX(isInBerserkerMode);

    }

    public void BerserkerUpgrade(float dmg = 0, float spd = 0, float duration = 0, float cooldownReduction = 0)
    {
        damageBoost += dmg;
        speedBoost += spd;
        berserkerDuration += duration; //ou posso reduzir o cooldown -> testar depois
        berserkerDefaultCooldown -= cooldownReduction;
        healthPenalty += 0.4f;
    }

    #endregion

    #region Vampirism Skill

    private void ActivateVampirismSkill()
    {
        if (isVampirismInCooldown == true || isInVampirismMode)
        {
            Debug.Log("VampirismSkill in Cooldown!!");
            //Tocar um som de erro
            return;
        }

        isInVampirismMode = true;

        SetVampirismFX(isInVampirismMode);

        //Invoke("DeactivateVampirismSkill", 3f); // o método de baixo é a fórmula correta, mas vou usar esta para testes
        //LEMBRAR DE TROCAR DEPOIS, SUA BESTA!!!
        Invoke("DeactivateVampirismSkill", vampirismDuration);

    }

    public void SetVampirismFX(bool active)
    {
        if (vignette == null) return;

        vignette.color.Override(new Color(0.5f, 0f, 0.4f)); // roxo
        vignette.intensity.Override(active ? 0.4f : 0f);
    }

    private void DeactivateVampirismSkill()
    {
        isInVampirismMode = false;

        isVampirismInCooldown = true;
        vampirismCooldown = vampirismDefaultCooldown;

        SetVampirismFX(isInVampirismMode);

    }

    public void VampirismUpgrade(float dmg = 0, float duration = 0, float cooldownReduction = 0)
    {
        vampirismAbsorb += dmg;
        vampirismDuration += duration; //ou posso reduzir o cooldown -> testar depois
        vampirismDefaultCooldown -= cooldownReduction;
    }

    #endregion

    #region Lazarus Skill

    private void ActivateLazarusSkill()
    {
        if (isLazarusInCooldown == true || isInLazarusMode)
        {
            Debug.Log("LazarusSkill in Cooldown!!");
            //Tocar um som de erro
            return;
        }

        isInLazarusMode = true;
        SetLazarusFX(isInLazarusMode);
        LazarusUpdateDamage();


        //Invoke("DeactivateLazarusSkill", 3f); // o método de baixo é a fórmula correta, mas vou usar esta para testes
        //LEMBRAR DE TROCAR DEPOIS, SUA BESTA!!!
        Invoke("DeactivateLazarusSkill", lazarusDuration);

    }

    //Este é diferente! Será atualizado junto do cálculo de dano em vez de ser conforme a ativação!!!
    public void SetLazarusFX(bool active, float lifePercent = .5f)
    {
        if (vignette == null) return;

        // quanto menor a vida, maior a intensidade
        float intensity = Mathf.Lerp(0.1f, 0.5f, 1f - lifePercent);
        vignette.color.Override(new Color(0f, 0.9f, 0.6f)); // verde água
        vignette.intensity.Override(intensity);
        vignette.intensity.Override(active ? intensity : 0f);
    }
    

    public void LazarusUpdateDamage()
    {

        if (wasJustActivated)
        {
            resetCoeficient = 1;
        }

        weaponController.damage /= resetCoeficient;

        float lifeCalc = player.currentHealth / player.maxHealth;
        SetLazarusFX(isInLazarusMode, lifeCalc);
        float damageMultiplier = (1 - lifeCalc) * lazarusBuff;

        weaponController.damage *= (1 + damageMultiplier);

        resetCoeficient = (1 + damageMultiplier);

        wasJustActivated = false;
    }

    private void DeactivateLazarusSkill()
    {
        weaponController.damage /= resetCoeficient;
        isInLazarusMode = false;
        //weaponController.damage /= damageBoost;      

        isLazarusInCooldown = true;
        wasJustActivated = true;
        lazarusCooldown = lazarusDefaultCooldown;

        SetLazarusFX(isInLazarusMode, 1f);

    }

    public void LazarusUpgrade(float buff = 0, float duration = 0, float cooldownReduction = 0)
    {
        lazarusBuff += buff;
        lazarusDuration += duration;
        lazarusDefaultCooldown -= cooldownReduction;
    }

    #endregion

    void LateUpdate()
    {
        if (Camera.main == null) return;
        IconCanvas.forward = Camera.main.transform.forward;
    }

    private void AssignInputEvents()
    {
        InputSystem_Actions action = player.inputActions;

        action.Player.Skill1.performed += context => ActivateBerserkerSkill();
        action.Player.Skill2.performed += context => ActivateVampirismSkill();
        action.Player.Skill3.performed += context => ActivateLazarusSkill();
        //action.Player.Skill4.performed += context => ActivateBerserkerSkill();


    }

}
