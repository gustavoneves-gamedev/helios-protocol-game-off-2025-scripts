//using UnityEditor.Build;
using UnityEngine;

public class BuffApply : MonoBehaviour
{
    
    //public int powerUpClass;
    public int powerUpType;
    public int powerUpRarity;

    private GameObject playerGameObject;

    private Player player;
    private PlayerWeaponController playerWeaponController;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerGameObject = GameController.gameController.player;  
        player = gameObject.GetComponent<Player>();
        playerWeaponController = player.GetComponent<PlayerWeaponController>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void UpdatePowerupType(int powerUpType, int powerUpRarity)
    {
                
        if (powerUpType == 1)
        {
            UpdateMaxLife(powerUpRarity);
            return;
        }
        if (powerUpType == 2)
        {
            UpdateMaxShield(powerUpRarity);
            return;
        }
        if (powerUpType == 3)
        {
            UpdateSpeed(powerUpRarity);
            return;
        }
        if (powerUpType == 4)
        {
            UpdateDamage(powerUpRarity);
            return;
        }
        if (powerUpType == 5)
        {
            UpdateBulletSpeed(powerUpRarity);
            return;
        }
        if (powerUpType == 6)
        {
            UpdateRange(powerUpRarity);
            return;
        }
    }

    private void UpdateMaxLife(int powerUpRarity)
    {
        if (powerUpRarity == 1)
        {
            player.UpdateMaxLife(10);
        }
        if (powerUpRarity == 2)
        {
            player.UpdateMaxLife(15);
        }
        if (powerUpRarity == 3)
        {
            player.UpdateMaxLife(30);
        }
        if (powerUpRarity == 4)
        {
            player.UpdateMaxLife(50);
        }
        if (powerUpRarity == 5)
        {
            player.UpdateMaxLife(80);
        }
    }

    private void UpdateMaxShield(int powerUpRarity)
    {
        if (powerUpRarity == 1)
        {
            player.UpdateMaxShield(10);
        }
        if (powerUpRarity == 2)
        {
            player.UpdateMaxShield(15);
        }
        if (powerUpRarity == 3)
        {
            player.UpdateMaxShield(30);
        }
        if (powerUpRarity == 4)
        {
            player.UpdateMaxShield(50);
        }
        if (powerUpRarity == 5)
        {
            player.UpdateMaxShield(80);
        }
    }

    private void UpdateSpeed(int powerUpRarity)
    {
        if (powerUpRarity == 1)
        {
            playerMovement.UpdateSpeed(.5f);
        }
        if (powerUpRarity == 2)
        {
            playerMovement.UpdateSpeed(1);
        }
        if (powerUpRarity == 3)
        {
            playerMovement.UpdateSpeed(2);
        }
        if (powerUpRarity == 4)
        {
            playerMovement.UpdateSpeed(3);
        }
        if (powerUpRarity == 5)
        {
            playerMovement.UpdateSpeed(5);
        }
    }

    private void UpdateDamage(int powerUpRarity)
    {
        if (powerUpRarity == 1)
        {
            playerWeaponController.UpdateDamage(5);
        }
        if (powerUpRarity == 2)
        {
            playerWeaponController.UpdateDamage(8);
        }
        if (powerUpRarity == 3)
        {
            playerWeaponController.UpdateDamage(12);
        }
        if (powerUpRarity == 4)
        {
            playerWeaponController.UpdateDamage(18);
        }
        if (powerUpRarity == 5)
        {
            playerWeaponController.UpdateDamage(25);
        }
    }

    private void UpdateBulletSpeed(int powerUpRarity)
    {
        if (powerUpRarity == 1)
        {
            playerWeaponController.UpdateBulletSpeed(2);
        }
        if (powerUpRarity == 2)
        {
            playerWeaponController.UpdateBulletSpeed(4);
        }
        if (powerUpRarity == 3)
        {
            playerWeaponController.UpdateBulletSpeed(7);
        }
        if (powerUpRarity == 4)
        {
            playerWeaponController.UpdateBulletSpeed(10);
        }
        if (powerUpRarity == 5)
        {
            playerWeaponController.UpdateBulletSpeed(15);
        }
    }

    private void UpdateRange(int powerUpRarity)
    {
        if (powerUpRarity == 1)
        {
            playerWeaponController.UpdateRange(2);
        }
        if (powerUpRarity == 2)
        {
            playerWeaponController.UpdateRange(4);
        }
        if (powerUpRarity == 3)
        {
            playerWeaponController.UpdateRange(7);
        }
        if (powerUpRarity == 4)
        {
            playerWeaponController.UpdateRange(10);
        }
        if (powerUpRarity == 5)
        {
            playerWeaponController.UpdateRange(15);
        }
    }

    //Estou usando apenas para teste, na realidade ele nem vai aparecer na tela...
    private void OnTriggerEnter(Collider other)
    {
        //player = GameController.gameController.player;
        //playerWeaponController = player.GetComponent<PlayerWeaponController>();
        //playerMovement = player.GetComponent<PlayerMovement>();


        if (player != null)
        {
            UpdatePowerupType(6,5);
            Debug.Log("Estou trigando");
            //gameObject.SetActive(false);
        }

    }
}
