using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Buff Data", menuName = "Buff System/Buff Data")]

public class Buff_Data : ScriptableObject
{
    public BuffType buffType;
    public Rarity rarity;
    public string buffName;
    public float increment;
    public float buffTypeCode;
    public float rarityTypeCode;
    public bool buffSelected;
    public bool isActiveAbility;
    public bool isPassiveAbility;
    public int skillCode;
    public int skillLevel;
    public bool skillSelected;
    public Sprite image;
}
