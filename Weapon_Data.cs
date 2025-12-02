using UnityEngine;



[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/Weapon Data")]

public class Weapon_Data : ScriptableObject
{
    public string weaponName;

    [Header("Regular shot")]
    public ShootType shootType;
    public int bulletsPerShot = 1;
    public float fireRate;

    [Header("Burst shot")]
    public bool burstAvailable;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay = .1f;

    [Header("Weapon spread")]
    public float baseSpread;
    public float maxSpread;
    public float spreadIncreaseRate = .15f;

    [Header("Weapon generics")]
    public WeaponType weaponType;
    //public int gunPoints;
    public float range = 50f;    
    public int bulletSpeed = 50;
    public int damage = 50;

    

}
