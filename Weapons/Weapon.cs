using UnityEngine;

public enum WeaponType
{
    MainBasicWeapon,
    DualShotWeapon,
    SniperMainWeapon,
    ShotgunWeapon,
    ChainsawWeapon,
    LaserWeapon
}

public enum ShootType
{
    Single,
    Auto
}

[System.Serializable] //faz com que esta Classe seja visível no inspector

public class Weapon
{
    public WeaponType weaponType;
    public ShootType shootType;
    public int bulletsPerShot { get; private set; }

    private float defaultFireRate;
    public float fireRate = 1;
    private float lastShootTime;

    [Header("Burst mode info")]
    private bool burstAvailable;
    public bool burstActive;

    private int burstBulletsPerShot;
    private float burstFireRate;
    public float burstFireDelay {  get; private set; }

    public float range;
    public int bulletSpeed;
    public int damage;
    //public float cameraDistance { get; private set; } -> pensei em criar algo para regular a distância da câmera com base
    //na arma sendo utilizada

    [Header("Spread")]
    private float baseSpread = 1;
    private float maximumSpread = 3;
    private float currentSpread;

    private float spreadIncreaseRate = .15f;

    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;
    

    public Weapon(Weapon_Data weaponData)//permite que outros scripts acessem todas as informações da arma!
    {
        fireRate = weaponData.fireRate;
        this.weaponType = weaponData.weaponType;

        bulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;

        burstAvailable = weaponData.burstAvailable;
        burstActive = weaponData.burstActive;
        burstBulletsPerShot = weaponData.burstBulletsPerShot;
        burstFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;


        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;
                
        range = weaponData.range;
        bulletSpeed = weaponData.bulletSpeed;
        damage = weaponData.damage;
        //cameraDistance = weaponData.cameraDistance;


        defaultFireRate = fireRate;

    }

    //aplica imprecisão na mira conforme o jogador segure o botão de tiro.
    //PRETENDO APRIMORAR: aumentar o spread conforme movimento do jogador!!
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
        //Testar estes valores para reduzir o spread vertical

        Vector3 spread = spreadRotation * originalDirection;
        return new Vector3(spread.x, 0 , spread.z);

        //return = spreadRotation * originalDirection;

    }

    //Determina se o spread vai continuar aumentando com base no tempo entre o disparo atual e o anterior
    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            currentSpread = baseSpread;
            //Debug.Log("Aumentando o Spread");

        }
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }

    //Aumenta o spread
    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    //Configurando a escopeta para sempre dar burstshot e retorna se a arma está ou não em modo Burst
    public bool BurstActivated()
    {
        if (weaponType == WeaponType.ShotgunWeapon)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    //Verifica se arma pode ou não entrar em modo Burst e ativa/destaiva esse modo
    //APRIMORAR: Provavelmente irei tirar essa opção do Player e deixarei embutida na arma!
    public void ToggleBurst()
    {
        if (burstAvailable == false)
            return;

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = defaultFireRate;
        }
    }

    //Verfica se o jogador pode atirar com base no cooldown(fireRate) da arma equipada
    public bool CanShoot()    
    {
        
        return ReadyToFire();
    }

    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1f/fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }

        return false;
    }
}
