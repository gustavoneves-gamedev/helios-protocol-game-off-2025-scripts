using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;    

    [SerializeField] private Weapon_Data defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;//Esse weapon é um script com variáveis e o ENUM weaponType
                                                  //está entre elas!!

    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet type info")]    
    private GameObject mainBulletTypePrefab;
    [SerializeField] private GameObject normalBullet;
    [SerializeField] private GameObject explosiveBullet;    
    public bool isShootingExplosiveBullet;
    [SerializeField] private float bulletImpactForce = 100f;


    [Header("Buffs")]
    public float damage;
    private float damageBuff;
    public float bulletSpeed;
    private float bulletSpeedBuff;   
    [SerializeField] private float explosiveDamage;
    private float explosiveDamageBuff;
    [SerializeField] private float range;
    private float rangeBuff;

    [SerializeField] private MainWeaponModel[] mainWeaponModels;

    //[Header("Inventory")]
    //[SerializeField] private int maxSlots = 1;
    //[SerializeField] private List<Weapon> weaponSlots;
    //Estou fazendo uma lista com slots que possuem todas as informações do Weapon

    [Header("Secondary Weapon Types - Slot 1")]
    [SerializeField] private Transform[] secondaryWeaponTransforms;

    public Transform morteiro;



    private void Start()
    {
        player = GetComponent<Player>();
        
        mainWeaponModels = GetComponentsInChildren<MainWeaponModel>(true);

        mainBulletTypePrefab = normalBullet;        

        Invoke("EquipStartingWeapon", .1f);

        

        AssignInputEvents();
    }

    private void Update()
    {
        if (GameController.gameController.isGamePaused)
            return;

        if (isShooting)
            Shoot();

        //CheckWeaponSwitch();
    }    

    public void UpdateDamage(float incrementValue)
    {
        damageBuff += incrementValue;
        GameController.gameController.playerDamageIncrement += incrementValue;
        UpdateStats();
    }

    public void UpdateBulletSpeed(float incrementValue)
    {
        bulletSpeedBuff += incrementValue;
        GameController.gameController.playerBulletSpeedIncrement += incrementValue;
        UpdateStats();
    }

    public void UpdateRange(float incrementValue)
    {
        rangeBuff += incrementValue;
        GameController.gameController.playerRangeIncrement += incrementValue;
        UpdateStats();
    }

    private void EquipStartingWeapon()
    {
        
        currentWeapon = new Weapon(defaultWeaponData);
        UpdateStats();

        //Atualizando os Status do Player DAMAGE + BULLETSPEED + RANGE -> Não é o melhor lugar, mas irá funcionar
        //para a DEMO
        GameController.gameController.playerDamage = damage;
        GameController.gameController.playerBulletSpeed = bulletSpeed;
        GameController.gameController.playerRange = range;

        SwitchOn();
        //EquipWeapon(0);
    }
    public void PickupWeapon(Weapon_Data newWeaponData)
    {
        currentWeapon = new Weapon(newWeaponData);
        UpdateStats();
        SwitchOn();        

    }

    private void UpdateStats()
    {
        bulletSpeed = currentWeapon.bulletSpeed + bulletSpeedBuff;        
        damage = currentWeapon.damage + damageBuff;        
        range = currentWeapon.range + rangeBuff;
    }

    

    public void SwitchOn()
    {
        SwitchOffGuns();
        CurrentWeaponModel().gameObject.SetActive(true);        
    }

    private void SwitchOffGuns()
    {
        
        for (int i = 0; i < mainWeaponModels.Length; i++)
        {
            mainWeaponModels[i].gameObject.SetActive(false);
        }
    }

    public void SwitchSecondaryWeaponOn(Transform secondaryWeaponTransform)
    {
        SwitchSecondaryWeaponOff();
        secondaryWeaponTransform.gameObject.SetActive(true);
        //secondaryWeaponTransform.GetComponent<MainBasicWeaponScript>().UpdateBulletInfo();
        player.currentSecondaryWeapon = secondaryWeaponTransform;
    }

    private void SwitchSecondaryWeaponOff()
    {
        for (int i = 0; i < secondaryWeaponTransforms.Length; i++)
        {
            secondaryWeaponTransforms[i].gameObject.SetActive(false);
        }
    }

    

    private void SwitchExplosiveShot()
    {
        //Tomar cuidado aqui para não dobrar o dano loucamente com algum exploit
        if (mainBulletTypePrefab == normalBullet)
        {
            mainBulletTypePrefab = explosiveBullet;
            bulletSpeed = bulletSpeed * 0.5f;
            isShootingExplosiveBullet = true;
        }
        else
        {
            mainBulletTypePrefab = normalBullet;
            bulletSpeed = bulletSpeed * 2;
            isShootingExplosiveBullet = false;      
        }                
        
    }
              

    private MainWeaponModel CurrentWeaponModel()
    {
        MainWeaponModel weaponModel = null;

        WeaponType weaponType = currentWeapon.weaponType;

        for (int i = 0; i < mainWeaponModels.Length; i++)
        {
            if (mainWeaponModels[i].weaponType == weaponType)
                weaponModel = mainWeaponModels[i];
        }

        return weaponModel;
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;    

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if (i >= currentWeapon.bulletsPerShot)
                SetWeaponReady(true);
        }
    }

    private Transform GunPoint1() => CurrentWeaponModel().gunPoint1;
    private Transform GunPoint2() => CurrentWeaponModel().gunPoint2;
    //Estou pensando em criar variáveis de GunPoint neste script e alimentá-las com 1 função só

    private void Shoot()
    {
        
        if (currentWeapon.CanShoot() == false)
        {
            return;
        }
        

        if (currentWeapon.shootType == ShootType.Single)//Isso aqui vai definir se é possível apenas segurar o botão
            isShooting = false;// para atirar ou não

        if (currentWeapon.BurstActivated() == true)
        {
            StartCoroutine(BurstFire());
            return;

        }

        FireSingleBullet();
    }

    private void FireSingleBullet()
    {
        if (CurrentWeaponModel().gunPointsQuantity == 1)
        {
            GameObject newBullet = ObjectPool.instance.GetObject(mainBulletTypePrefab, GunPoint1());

            //newBullet.transform.position = GunPoint1().position;
            newBullet.transform.rotation = Quaternion.LookRotation(GunPoint1().forward);

            Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            bulletScript.BulletSetup(range, damage, bulletImpactForce);

            Vector3 bulletsDirection = currentWeapon.ApplySpread(GunPoint1().forward);
            

            rbNewBullet.linearVelocity = bulletsDirection * bulletSpeed;
            //rbNewBullet.linearVelocity = GunPoint1().forward * bulletSpeed;
        }

        if (CurrentWeaponModel().gunPointsQuantity == 2)
        {
            GameObject newBullet1 = ObjectPool.instance.GetObject(mainBulletTypePrefab, GunPoint1());
            GameObject newBullet2 = ObjectPool.instance.GetObject(mainBulletTypePrefab, GunPoint2());

            //newBullet1.transform.position = GunPoint1().position;
            newBullet1.transform.rotation = Quaternion.LookRotation(GunPoint1().forward);

            //newBullet2.transform.position = GunPoint2().position;
            newBullet2.transform.rotation = Quaternion.LookRotation(GunPoint2().forward);

            Rigidbody rbNewBullet1 = newBullet1.GetComponent<Rigidbody>();
            Rigidbody rbNewBullet2 = newBullet2.GetComponent<Rigidbody>();

            BulletScript bulletScript1 = newBullet1.GetComponent<BulletScript>();
            bulletScript1.BulletSetup(range, damage, bulletImpactForce);

            BulletScript bulletScript2 = newBullet2.GetComponent<BulletScript>();
            bulletScript2.BulletSetup(range, damage, bulletImpactForce);

            rbNewBullet1.linearVelocity = GunPoint1().forward * bulletSpeed;
            rbNewBullet2.linearVelocity = GunPoint2().forward * bulletSpeed;
        }

    }

    private void AssignInputEvents()
    {
        InputSystem_Actions action = player.inputActions;

        action.Player.Attack.performed += context => isShooting = true;
        action.Player.Attack.canceled += context => isShooting = false;



        action.Player.SwitchBullet.performed += context => SwitchExplosiveShot();
    }

    

    //private void Fire()
    //{
    //    bool canFireExplosveShots = isShootingExplosiveBullet == true && player.explosiveShots >= mainBasicWeaponScript.gunPointsQuantity;


    //    if (player.currentWeapon != null && mainBasicWeaponScript != null)
    //    {
    //      if (isShootingExplosiveBullet == false || canFireExplosveShots)
    //        { 
    //            mainBasicWeaponScript.Shoot(mainBulletTypePrefab); 
    //        }
    //      else
    //        {
    //            Debug.Log("Insuficient Explosive Shots");
    //        }

    //    }

    //    //tocar som de erro e aparecer mensagem de "Insuficient Explosive Shots"
    //    //&& player.explosiveShots >= mainBasicWeaponScript.gunPointsQuantity

    //}
}
