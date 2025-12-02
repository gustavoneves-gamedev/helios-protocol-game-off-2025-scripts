using UnityEngine;

public class BossTurrets : MonoBehaviour
{
    [Header("Status")]
    public bool isTargetInRange;
    private bool canShoot;
    private float cooldown;
    private Transform target;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float turretDamage;
    [SerializeField] private float turretRange;
    [SerializeField] private float turretSpeed;
    [SerializeField] private float fireRate;

    [Header("Shoot Info")]
    [SerializeField] private GameObject turretBulletPrefab;
    [SerializeField] private Transform gunPoint1;
    [SerializeField] private Transform gunPoint2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //target = GameController.gameController.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTargetInRange == false) return;

        RotateBossTurret();
    }

    private void RotateBossTurret()
    {

        if (target == null)
            target = GameController.gameController.player.transform;

        canShoot = true;

        if (cooldown >= 0)
        {
            cooldown -= Time.deltaTime;
            canShoot = false;
        }

        if (target != null && isTargetInRange)
        {

            Quaternion targetRotation = Quaternion.LookRotation(
                    (target.position - transform.position).normalized);

            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                targetRotation, rotationSpeed * Time.deltaTime);

            //if(laserBall.transform.forward - )

            if (canShoot == true)
                turretShoot();

        }
    }

    private void turretShoot()
    {
        GameObject newBullet1 = ObjectPool.instance.GetObject(turretBulletPrefab, gunPoint1);
        GameObject newBullet2 = ObjectPool.instance.GetObject(turretBulletPrefab, gunPoint2);


        newBullet1.transform.rotation = Quaternion.LookRotation(gunPoint1.forward);

        newBullet2.transform.rotation = Quaternion.LookRotation(gunPoint2.forward);

        Rigidbody rbNewBullet1 = newBullet1.GetComponent<Rigidbody>();
        Rigidbody rbNewBullet2 = newBullet2.GetComponent<Rigidbody>();

        EnemyBulletScript bulletScript1 = newBullet1.GetComponent<EnemyBulletScript>();
        bulletScript1.BulletSetup(turretRange, turretDamage, 0);

        EnemyBulletScript bulletScript2 = newBullet2.GetComponent<EnemyBulletScript>();
        bulletScript2.BulletSetup(turretRange, turretDamage, 0);

        rbNewBullet1.linearVelocity = newBullet1.transform.forward * turretSpeed;
        rbNewBullet2.linearVelocity = newBullet2.transform.forward * turretSpeed;


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



}
