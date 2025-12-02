using UnityEngine;

public class PlayerPassiveSkills : MonoBehaviour
{
    [SerializeField] private Transform passiveSkillParent;

    [Header("Spinning Blade")]
    public bool hasSpinningBladeSkill;
    public int spinningBladeLevel;
    public float spinningBladeDamage;
    [SerializeField] private GameObject spinningBlade;
    private bool isSpinningBladeActive;
    public float rotationSpeed;
    [SerializeField] private GameObject[] spinningBlades;
    [SerializeField] private int activeBlades = 0; //Está em SerializeField apenas para testes, depois será privada mesmo

    [Header("Laser Ball")]
    public bool hasLaserBallSkill;
    public int laserBallLevel;
    [SerializeField] private GameObject laserBallBase;
    [SerializeField] private GameObject laserBall;
    [SerializeField] private Transform gunPoint1;
    [SerializeField] private Transform gunPoint2;
    private int gunPointsQuantity = 1;
    private bool isLaserBallActive;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float laserBallRotationSpeed;
    [SerializeField] private GameObject laserBulletPrefab;
    [SerializeField] private float laserSpeed = 50f; //Está em SerializeField apenas para testes, depois será privada
    public DetectionScript laserDetection { get; private set; }
    //private Transform activeEnemyTransform;
    private float cooldown = 1f;
    [SerializeField] private float fireRate = 0.5f; //Está em SerializeField apenas para testes, depois será privada
    private bool canShoot;
    private float laserRange = 30f;
    [SerializeField] private float laserDamage = 20f;


    void Start()
    {
        //ActivateSpinningBladesSkill();
        //ActivateLaserBallSkill();
    }


    void Update()
    {
        RotateSpinningBlades();

        MoveLaserBall();
        
    }

    #region SpinningBlade

    public void ActivateSpinningBladesSkill()
    {

        //spinningBladeLevel++;

        spinningBlade.gameObject.SetActive(true);

        AddBlade();


        spinningBlade.transform.parent = null;
        isSpinningBladeActive = true; //Quando eu criar uma animação para ativar as lâminas, essa bool só será true depois
                                      //que a animação terminar

    }

    //Acho que vou usar isso para parar a skill e tocar a animação de guardar as lâminas
    private void DisableSpinningBladesSkill()
    {
        isSpinningBladeActive = false;
        spinningBlade.transform.parent = passiveSkillParent;
        //playStopAnimation;
    }

    private void AddBlade()
    {
        HideBlades();

        for (int i = 0; i < activeBlades; i++)
        {
            spinningBlades[i].SetActive(true);
        }
    }

    private void HideBlades()
    {
        for (int i = 0; i < spinningBlades.Length; i++)
        {
            spinningBlades[i].SetActive(false);
        }
    }

    private void RotateSpinningBlades()
    {
        if (isSpinningBladeActive == true)
        {
            spinningBlade.transform.position = transform.position;
            spinningBlade.transform.Rotate(0, (rotationSpeed * Time.deltaTime), 0);

        }
    }

    public void UpgradeSpinningBlades(int bladesToAdd = 0, float speedToIncrease = 0, float damageToIncrease = 0)
    {

        activeBlades += bladesToAdd;

        rotationSpeed += speedToIncrease;
        spinningBladeDamage += damageToIncrease;

        if (bladesToAdd > 0 && activeBlades < 6)
        {
            ActivateSpinningBladesSkill();
        }

    }


    #endregion

    #region LaserBall

    public void ActivateLaserBallSkill()
    {
        //laserBallLevel++;

        laserBallBase.gameObject.SetActive(true);
        laserDetection = laserBallBase.gameObject.GetComponent<DetectionScript>();


        laserBallBase.transform.parent = null;
        isLaserBallActive = true; //Quando eu criar uma animação para ativar as laserBall, essa bool só será true depois
                                  //que a animação terminar

    }

    private void MoveLaserBall()
    {
        if (isLaserBallActive == true)
        {
            Transform target = laserDetection.activeEnemyTransform;

            laserBallBase.transform.position = transform.position;
            laserBallBase.transform.Rotate(0, (moveSpeed * Time.deltaTime), 0);

            canShoot = true;


            if (cooldown >= 0)
            {
                cooldown -= Time.deltaTime;
                canShoot = false;
            }



            if (laserDetection.activeEnemyTransform != null)
            {

                Quaternion targetRotation = Quaternion.LookRotation(
                        (target.position - laserBall.transform.position).normalized);

                laserBall.transform.rotation = Quaternion.RotateTowards(laserBall.transform.rotation,
                    targetRotation, rotationSpeed * Time.deltaTime);

                //if(laserBall.transform.forward - )

                if (canShoot == true)
                    LaserShoot();

                
                //laserBall.transform.LookAt(laserDetection.activeEnemyTransform.position);

            }

        }
    }

    private void LaserShoot()
    {
        if (gunPointsQuantity == 1)
        {
            GameObject newBullet = ObjectPool.instance.GetObject(laserBulletPrefab, gunPoint1);

            //newBullet.transform.position = GunPoint1().position;
            newBullet.transform.rotation = Quaternion.LookRotation(gunPoint1.forward);

            Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            //bulletScript.BulletSetup(range, damage, bulletImpactForce);
            bulletScript.BulletSetup(laserRange, laserDamage, 0);

            rbNewBullet.linearVelocity = newBullet.transform.forward * laserSpeed;
            


        }

        //Não criei a possibilidade de atirar 2 balas de uma vez ainda, mas o código abaixo servirá para isso
        if (gunPointsQuantity == 2)
        {
            GameObject newBullet1 = ObjectPool.instance.GetObject(laserBulletPrefab, gunPoint1);
            GameObject newBullet2 = ObjectPool.instance.GetObject(laserBulletPrefab, gunPoint2);

           
            newBullet1.transform.rotation = Quaternion.LookRotation(gunPoint1.forward);
                        
            newBullet2.transform.rotation = Quaternion.LookRotation(gunPoint2.forward);

            Rigidbody rbNewBullet1 = newBullet1.GetComponent<Rigidbody>();
            Rigidbody rbNewBullet2 = newBullet2.GetComponent<Rigidbody>();

            BulletScript bulletScript1 = newBullet1.GetComponent<BulletScript>();
            bulletScript1.BulletSetup(laserRange, laserDamage, 0);

            BulletScript bulletScript2 = newBullet2.GetComponent<BulletScript>();
            bulletScript2.BulletSetup(laserRange, laserDamage, 0);

            rbNewBullet1.linearVelocity = newBullet1.transform.forward * laserSpeed;
            rbNewBullet2.linearVelocity = newBullet2.transform.forward * laserSpeed;


        }

        canShoot = false;

        if (fireRate > 0)
        {
            cooldown = (1 / fireRate);
        }
        else
        {
            cooldown = 1;
        }
    }

    public void UpgradeLaserBall(float speedToIncrease = 0, float damageToIncrease = 0, float fireRateToIncrease = 0, 
        float rangeToIncrease = 0)
    {

        laserSpeed += speedToIncrease;
        fireRate += fireRateToIncrease;
        laserRange += rangeToIncrease;
        laserDamage += damageToIncrease;

        //ActivateLaserBallSkill();
        //laserBallLevel++;
    }
    
    #endregion

}
