using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SecondaryWeapon : MonoBehaviour
{
    [Header("Aux Weapon Info")]
    public Transform secondaryWeaponTransform;
    public float rotationSpeed;
    public Transform targetTransform;
    public float cooldown = 1f;
    private float currentCooldown;
    [SerializeField] private bool isExplosive;

    [Header("Gun Info")]
    [SerializeField] private Transform gunPoint1;
    [SerializeField] private Transform gunPoint2;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject mainBulletPrefab;
    [SerializeField] private GameObject explosiveBulletPrefab;
    [SerializeField] private float gunPointsQuantity;
    public float bulletDamage = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        cooldown = 1f;
        currentCooldown = cooldown;        

    }

    // Update is called once per frame
    void Update()
    {
        currentCooldown -= 1 * Time.deltaTime;
        if (currentCooldown <= 0)
        {
            Shoot();
            currentCooldown = cooldown;
        }
    }

    private void FixedUpdate()
    {
        SecondWeaponRotation();
        
        
    }

    private void SecondWeaponRotation()
    {
        Vector3 direction = targetTransform.position - secondaryWeaponTransform.position;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        secondaryWeaponTransform.rotation = Quaternion.RotateTowards(secondaryWeaponTransform.rotation, targetRotation, rotationSpeed);

    }

    private void Shoot()
    {
        if (gunPointsQuantity == 1 && isExplosive == false)
        {
            GameObject mainBullet = Instantiate(mainBulletPrefab, gunPoint1.position, gunPoint1.rotation);

            BulletScript bulletScript = mainBullet.GetComponent<BulletScript>();
            bulletScript.damage = bulletDamage;
            bulletScript.range = 10f;

            mainBullet.GetComponent<Rigidbody>().linearVelocity = gunPoint1.forward * bulletSpeed;

            Destroy(mainBullet, 5);
        }

        if (gunPointsQuantity == 1 && isExplosive == true)
        {
            GameObject explosiveBullet = Instantiate(explosiveBulletPrefab, gunPoint1.position, gunPoint1.rotation);

            BulletScript bulletScript = explosiveBullet.GetComponent<BulletScript>();
            bulletScript.damage = bulletDamage;
            bulletScript.range = 20f;
            //Este BulletDamage não está vinculado ao dano do jogador por enquanto


            explosiveBullet.GetComponent<Rigidbody>().linearVelocity = gunPoint1.forward * bulletSpeed;

            Destroy(explosiveBullet, 5);

        }

    }

    


}
