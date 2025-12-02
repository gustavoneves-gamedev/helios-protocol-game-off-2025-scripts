using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    [SerializeField] private Weapon_Data weaponData;
    //public int weaponType;

    
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerWeaponController>()?.PickupWeapon(weaponData);
        
    }

}
