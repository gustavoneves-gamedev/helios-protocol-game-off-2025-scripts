using UnityEngine;
using UnityEngine.UI;

public enum BuffType
{
    None,
    LifeUp,
    ShieldUp,
    SpeedUp,
    DamageUp,
    BulletSpeedUp,
    RangeUp,
    ExplosiveShotDamageUp
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]

public class Buff
{
    public BuffType buffType;
    public Rarity rarity;
    private string buffName;
    public float increment;
    public float buffTypeCode;
    public float rarityTypeCode;
    public bool buffSelected;
    public bool isActiveSkill;
    public bool isPassiveSkill;
    public int skillCode;
    public int skillLevel;
    public bool skillSelected;
    public Sprite image;
    //public Image image; -> Não sei como seria esta variável

    private PlayerWeaponController playerWeaponController;
    private PlayerMovement playerMovement;
    private Player player;
    private PlayerActiveSkills activeSkills;
    private PlayerPassiveSkills passiveSkills;


    public Buff(Buff_Data buffData)//permite que outros scripts acessem todas as informações do buff!
    {
       
        buffType = buffData.buffType;
        rarity = buffData.rarity;
        buffName = buffData.buffName;
        increment = buffData.increment;
        buffSelected = buffData.buffSelected;
        isActiveSkill = buffData.isActiveAbility;
        isPassiveSkill = buffData.isPassiveAbility;
        skillCode = buffData.skillCode;
        skillLevel = buffData.skillLevel;
        skillSelected = buffData.skillSelected;
        image = buffData.image;
        

    }

    public void UpdateBuffType()
    {
        //ver com professor depois se tem algum jeito melhor de alimentar isto
        AssignPlayer();

        if (isActiveSkill)
        {
            ActiveSkillUpdate(skillCode, skillLevel);
            return;
        }

        if (isPassiveSkill)
        {
            PassiveSkillUpdate(skillCode, skillLevel);
            return;
        }

        if (buffType == BuffType.LifeUp)
        {
            player.UpdateMaxLife(increment);
            return;
        }
        if (buffType == BuffType.ShieldUp)
        {
            player.UpdateMaxShield(increment);
            return;
        }
        if (buffType == BuffType.SpeedUp)
        {
            playerMovement.UpdateSpeed(increment);
            return;
        }
        if (buffType == BuffType.DamageUp)
        {
            playerWeaponController.UpdateDamage(increment);
            return;
        }
        if (buffType == BuffType.BulletSpeedUp)
        {
            playerWeaponController.UpdateBulletSpeed(increment);
            return;
        }
        if (buffType == BuffType.RangeUp)
        {
            playerWeaponController.UpdateRange(increment);
            return;
        }
    }

    private void ActiveSkillUpdate(int skillCode, int skillLevel)
    {
        //BerserkerSkill
        if (skillCode == 1)
        {
            if (skillLevel == 1)
            {
                activeSkills.hasBerserkerSkill = true;
            }
        }

        //VampirismSkill
        if (skillCode == 2)
        {
            if (skillLevel == 1)
            {
                activeSkills.hasVampirismSkill = true;
            }
        }

        //LazarusSkill
        if (skillCode == 3)
        {
            if (skillLevel == 1)
            {
                activeSkills.hasLazarusSkill = true;
            }
        }

    }

    private void PassiveSkillUpdate(int skillCode, int skillLevel)
    {
        //Spinning Blades
        if (skillCode == 1)
        {
            if (skillLevel == 1)
            {
                passiveSkills.ActivateSpinningBladesSkill();
                //activeSkills.hasBerserkerSkill = true;
                passiveSkills.spinningBladeLevel++;
                
            }
            else if (skillLevel == 2)
            {
                passiveSkills.UpgradeSpinningBlades(2, 300, 40);
                //activeSkills.hasBerserkerSkill = true;
                passiveSkills.spinningBladeLevel++;
                
            }
            else if (skillLevel == 3)
            {
                passiveSkills.UpgradeSpinningBlades(2, 300, 40);
                //activeSkills.hasBerserkerSkill = true;
                passiveSkills.spinningBladeLevel++;
                
            }

            // Em vez de ++, definimos o nível e travamos no máximo 3
            passiveSkills.spinningBladeLevel = Mathf.Clamp(skillLevel, 0, 3);
            return;
        }

        //Laser Ball
        if (skillCode == 2)
        {
            if (skillLevel == 1)
            {
                passiveSkills.ActivateLaserBallSkill();
                //activeSkills.hasVampirismSkill = true;
                                
            }
            else if (skillLevel == 2)
            {
                passiveSkills.UpgradeLaserBall(30f, 30f, 4.5f, 10f);
                //activeSkills.hasBerserkerSkill = true;
                
            }
            else if (skillLevel == 3)
            {
                passiveSkills.UpgradeLaserBall(50f, 50f, 6f, 10f);
                //activeSkills.hasBerserkerSkill = true;
                
            }

            // Mesmo esquema: define o level ao invés de ++
            passiveSkills.laserBallLevel = Mathf.Clamp(skillLevel, 0, 3);
            return;
        }
    }


    private void AssignPlayer()
    {
        player = GameController.gameController.player.GetComponent<Player>(); ;
        playerWeaponController = player.GetComponent<PlayerWeaponController>();
        playerMovement = player.GetComponent<PlayerMovement>();
        activeSkills = player.GetComponent<PlayerActiveSkills>();
        passiveSkills = player.GetComponent<PlayerPassiveSkills>();
    }
}


