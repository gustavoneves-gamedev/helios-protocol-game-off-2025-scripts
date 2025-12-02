using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class RewardController : MonoBehaviour
{
    public static RewardController rewardController;

    //CLASSIFICAÇÕES: S - 6, A - 5, B - 4, C - 3, D - 2, E - 1
    private int waveResult;
    private float currentWave;
    public bool rowAbilitySelected;
    public bool rowBoostSelected;
    private int skillToSelect = 1;
    private int skillsSelected;
    private int buffToSelect = 1;
    private int buffsSelected;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject abilitiesChoice;
    [SerializeField] private GameObject buffsChoice;

    [Header("UI – Reward Buttons")]
    [SerializeField] private List<RectTransform> rewardButtons;

    //public Canvas rewardScreen;

    [Header("Passive Skill 1")]
    public Image image1;
    public TMP_Text rarity1;


    [Header("Passive Skill 2")]
    public Image image2;
    public TMP_Text rarity2;


    [Header("Slot1")]
    public TMP_Text buffTypeA;
    public TMP_Text incrementA;
    public TMP_Text rarityA;
    public Image imageA;

    [Header("Slot2")]
    public TMP_Text buffTypeB;
    public TMP_Text incrementB;
    public TMP_Text rarityB;
    public Image imageB;

    [Header("Slot3")]
    public TMP_Text buffTypeC;
    public TMP_Text incrementC;
    public TMP_Text rarityC;
    public Image imageC;

    [Header("Slot4")]
    public TMP_Text buffTypeD;
    public TMP_Text incrementD;
    public TMP_Text rarityD;
    public Image imageD;

    [Header("Slot5")]
    public TMP_Text buffTypeE;
    public TMP_Text incrementE;
    public TMP_Text rarityE;
    public Image imageE;

    [SerializeField] private Buff_Data[] legendary;
    [SerializeField] private Buff_Data[] epic;
    [SerializeField] private Buff_Data[] rare;
    [SerializeField] private Buff_Data[] uncommon;
    [SerializeField] private Buff_Data[] common;

    [Header("Berserker Skills Data")]
    [SerializeField] private Buff_Data spinningBladesLevel1;
    [SerializeField] private Buff_Data spinningBladesLevel2;
    [SerializeField] private Buff_Data spinningBladesLevel3;

    [Header("Drone Skills Data")]
    [SerializeField] private Buff_Data droneLevel1;
    [SerializeField] private Buff_Data droneLevel2;
    [SerializeField] private Buff_Data droneLevel3;

    //[SerializeField] private Buff_Data test1;
    //[SerializeField] private Buff_Data test2;
    //[SerializeField] private Buff_Data test3;

    [SerializeField] private List<Buff> slots = new List<Buff>();
    [SerializeField] private List<Buff> skillSlots = new List<Buff>();

    private int slotsToAssign = 3;
    private int skillSlotsAssign = 2;

    [Header("Buff Info")]
    public Buff slot1;
    [SerializeField] private bool isSlot1Assigned;
    public Buff slot2;
    [SerializeField] private bool isSlot2Assigned;
    public Buff slot3;
    [SerializeField] private bool isSlot3Assigned;
    public Buff slot4;
    [SerializeField] private bool isSlot4Assigned;
    public Buff slot5;
    [SerializeField] private bool isSlot5Assigned;

    [SerializeField] private Buff slotToAssign;

    [Header("Skill Info")]
    public Buff passiveSkillSlot1;
    private bool isPassiveSkillSlot1Assigned;
    public Buff passiveSkillSlot2;
    private bool isPassiveSkillSlot2Assigned;

    [SerializeField] private Buff skillToAssign;

    [SerializeField] private int legendaryRewardCount;
    [SerializeField] private int epicRewardCount;
    [SerializeField] private int rareRewardCount;
    [SerializeField] private int uncommonRewardCount;
    [SerializeField] private int commonRewardCount;

    private PlayerPassiveSkills passiveSkills;

    private void Start()
    {
        GameController.gameController.rewardController = this;

        //passiveSkills = GameController.gameController.player.GetComponent<PlayerPassiveSkills>();
        
        //APENAS PARA TESTES!!!
        passiveSkillSlot1 = new Buff(spinningBladesLevel1);
        passiveSkillSlot2 = new Buff(droneLevel1);

        skillSlots.Add(passiveSkillSlot1);
        skillSlots.Add(passiveSkillSlot2);


    }

    public void AssignReward()
    {
        passiveSkills = GameController.gameController.player.GetComponent<PlayerPassiveSkills>();

        ResetSlots();
        ResetRewardVisuals();

        

        //waveResult = GameController.gameController.waveController.waveResult;
        waveResult = 6;
        currentWave = GameController.gameController.currentWave;

        if (currentWave == 1)
        {
            GameController.gameController.uiController.SecondTutorial();
        }

        if (waveResult == 6)
        {
            for (int i = 0; i < slotsToAssign; i++)
            {
                //Posso fazer com que o método RankS retorne um slot e jogar diretamente dentro do EmptySlot
                //Perguntar ao prof se é pertinente
                RankS();
                EmptySlot(slotToAssign);
               
            }
            
            if (passiveSkills.spinningBladeLevel == 1)
            {
                passiveSkillSlot1 = new Buff(spinningBladesLevel2);
                skillSlots.Clear();
            }
            else if (passiveSkills.spinningBladeLevel == 2)
            {
                passiveSkillSlot1 = new Buff(spinningBladesLevel3);
                skillSlots.Clear();
            }

            if (passiveSkills.laserBallLevel == 1)
            {
                passiveSkillSlot2 = new Buff(droneLevel2);
                skillSlots.Clear();
            }
            else if (passiveSkills.laserBallLevel == 2)
            {
                passiveSkillSlot2 = new Buff(droneLevel3);
                skillSlots.Clear();
            }

            skillSlots.Add(passiveSkillSlot1);
            skillSlots.Add(passiveSkillSlot2);

            UpdateRewardUI();
            return;

        }

        if (waveResult == 5)
        {
            for (int i = 0; i < slotsToAssign; i++)
            {
                //Posso fazer com que o método RankS retorne um slot e jogar diretamente dentro do EmptySlot
                //Perguntar ao prof se é pertinente
                RankA();
                EmptySlot(slotToAssign);
            }

            UpdateRewardUI();
            return;
        }

        if (waveResult == 4)
        {
            for (int i = 0; i < slotsToAssign; i++)
            {
                //Posso fazer com que o método RankS retorne um slot e jogar diretamente dentro do EmptySlot
                //Perguntar ao prof se é pertinente
                RankB();
                EmptySlot(slotToAssign);
            }

            UpdateRewardUI();
            return;
        }

        if (waveResult == 3)
        {
            for (int i = 0; i < slotsToAssign; i++)
            {
                //Posso fazer com que o método RankS retorne um slot e jogar diretamente dentro do EmptySlot
                //Perguntar ao prof se é pertinente
                RankC();
                EmptySlot(slotToAssign);
            }

            UpdateRewardUI();
            return;
        }

        if (waveResult == 2)
        {
            for (int i = 0; i < slotsToAssign; i++)
            {
                //Posso fazer com que o método RankS retorne um slot e jogar diretamente dentro do EmptySlot
                //Perguntar ao prof se é pertinente
                RankD();
                EmptySlot(slotToAssign);
            }

            UpdateRewardUI();
            return;
        }

        if (waveResult == 1)
        {
            for (int i = 0; i < slotsToAssign; i++)
            {
                //Posso fazer com que o método RankS retorne um slot e jogar diretamente dentro do EmptySlot
                //Perguntar ao prof se é pertinente
                RankE();
                EmptySlot(slotToAssign);
            }

            UpdateRewardUI();
            return;
        }


    }

        

    private void RankS()
    {
        float roll = Random.Range(0, 101);
        //Debug.Log("Rolei " +  roll + " para recompensa");

        if (roll > ((500f / currentWave) + (legendaryRewardCount * 30f)))
        {
            slotToAssign = new Buff(legendary[Random.Range(0, legendary.Length)]);
            legendaryRewardCount++;
            return;
        }

        if (roll > ((300f / currentWave) + (epicRewardCount * 20f)))
        {
            slotToAssign = new Buff(epic[Random.Range(0, epic.Length)]);
            epicRewardCount++;
            return;
        }

        if (roll > (rareRewardCount * (80f / currentWave)))
        {
            slotToAssign = new Buff(rare[Random.Range(0, rare.Length)]);
            rareRewardCount++;
            return;
        }

        if (roll > (uncommonRewardCount * (20f / currentWave)))
        {
            slotToAssign = new Buff(uncommon[Random.Range(0, uncommon.Length)]);
            uncommonRewardCount++;
            return;
        }

        if (roll >= 0)
        {
            slotToAssign = new Buff(common[Random.Range(0, common.Length)]);
            commonRewardCount++;
            return;
        }


    }

    private void RankA()
    {
        float roll = Random.Range(0, 101);
        Debug.Log("Rolei " + roll + " para recompensa");

        if (roll > ((800f / currentWave) + (legendaryRewardCount * 40f)))
        {
            slotToAssign = new Buff(legendary[Random.Range(0, legendary.Length)]);
            legendaryRewardCount++;
            return;
        }

        if (roll > ((400f / currentWave) + (epicRewardCount * 30f)))
        {
            slotToAssign = new Buff(epic[Random.Range(0, epic.Length)]);
            epicRewardCount++;
            return;
        }

        if (roll > (rareRewardCount * (160f / currentWave)))
        {
            slotToAssign = new Buff(rare[Random.Range(0, rare.Length)]);
            rareRewardCount++;
            return;
        }

        if (roll > (uncommonRewardCount * (50f / currentWave)))
        {
            slotToAssign = new Buff(uncommon[Random.Range(0, uncommon.Length)]);
            uncommonRewardCount++;
            return;
        }

        if (roll >= 0)
        {
            slotToAssign = new Buff(common[Random.Range(0, common.Length)]);
            commonRewardCount++;
            return;
        }


    }

    private void RankB()
    {
        float roll = Random.Range(0, 101);
        Debug.Log("Rolei " + roll + " para recompensa");

        if (roll > ((1000f / currentWave) + (legendaryRewardCount * 50f)))
        {
            slotToAssign = new Buff(legendary[Random.Range(0, legendary.Length)]);
            legendaryRewardCount++;
            return;
        }

        if (roll > ((700f / currentWave) + (epicRewardCount * 30f)))
        {
            slotToAssign = new Buff(epic[Random.Range(0, epic.Length)]);
            epicRewardCount++;
            return;
        }

        if (roll > ((160f / currentWave) + (rareRewardCount * 50f)))
        {
            slotToAssign = new Buff(rare[Random.Range(0, rare.Length)]);
            rareRewardCount++;
            return;
        }

        if (roll > (uncommonRewardCount * (50f / currentWave)))
        {
            slotToAssign = new Buff(uncommon[Random.Range(0, uncommon.Length)]);
            uncommonRewardCount++;
            return;
        }

        if (roll >= 0)
        {
            slotToAssign = new Buff(common[Random.Range(0, common.Length)]);
            commonRewardCount++;
            return;
        }

    }

    private void RankC()
    {
        float roll = Random.Range(0, 101);
        Debug.Log("Rolei " + roll + " para recompensa");

        if (roll > ((2000f / currentWave) + (legendaryRewardCount * 50f)))
        {
            slotToAssign = new Buff(legendary[Random.Range(0, legendary.Length)]);
            legendaryRewardCount++;
            return;
        }

        if (roll > ((1200f / currentWave) + (epicRewardCount * 40f)))
        {
            slotToAssign = new Buff(epic[Random.Range(0, epic.Length)]);
            epicRewardCount++;
            return;
        }

        if (roll > ((500f / currentWave) + (rareRewardCount * 40f)))
        {
            slotToAssign = new Buff(rare[Random.Range(0, rare.Length)]);
            rareRewardCount++;
            return;
        }

        if (roll > (uncommonRewardCount * (400f / currentWave)))
        {
            slotToAssign = new Buff(uncommon[Random.Range(0, uncommon.Length)]);
            uncommonRewardCount++;
            return;
        }

        if (roll >= 0)
        {
            slotToAssign = new Buff(common[Random.Range(0, common.Length)]);
            commonRewardCount++;
            return;
        }


    }

    private void RankD()
    {
        float roll = Random.Range(0, 101);
        Debug.Log("Rolei " + roll + " para recompensa");

        if (roll > ((2700f / currentWave) + (legendaryRewardCount * 50f)))
        {
            slotToAssign = new Buff(legendary[Random.Range(0, legendary.Length)]);
            legendaryRewardCount++;
            return;
        }

        if (roll > ((2000f / currentWave) + (epicRewardCount * 30f)))
        {
            slotToAssign = new Buff(epic[Random.Range(0, epic.Length)]);
            epicRewardCount++;
            return;
        }

        if (roll > ((1200f / currentWave) + (rareRewardCount * 50f)))
        {
            slotToAssign = new Buff(rare[Random.Range(0, rare.Length)]);
            rareRewardCount++;
            return;
        }

        if (roll > (uncommonRewardCount * (800f / currentWave)))
        {
            slotToAssign = new Buff(uncommon[Random.Range(0, uncommon.Length)]);
            uncommonRewardCount++;
            return;
        }

        if (roll >= 0)
        {
            slotToAssign = new Buff(common[Random.Range(0, common.Length)]);
            commonRewardCount++;
            return;
        }

    }

    private void RankE()
    {
        float roll = Random.Range(0, 101);
        Debug.Log("Rolei " + roll + " para recompensa");

        if (roll > ((3000f / currentWave) + (legendaryRewardCount * 50f)))
        {
            slotToAssign = new Buff(legendary[Random.Range(0, legendary.Length)]);
            legendaryRewardCount++;
            return;
        }

        if (roll > ((2700f / currentWave) + (epicRewardCount * 30f)))
        {
            slotToAssign = new Buff(epic[Random.Range(0, epic.Length)]);
            epicRewardCount++;
            return;
        }

        if (roll > ((2000f / currentWave) + (rareRewardCount * 50f)))
        {
            slotToAssign = new Buff(rare[Random.Range(0, rare.Length)]);
            rareRewardCount++;
            return;
        }

        if (roll > ((400f / currentWave) + (rareRewardCount * 40f)))
        {
            slotToAssign = new Buff(uncommon[Random.Range(0, uncommon.Length)]);
            uncommonRewardCount++;
            return;
        }

        if (roll >= 0)
        {
            slotToAssign = new Buff(common[Random.Range(0, common.Length)]);
            commonRewardCount++;
            return;
        }

    }

    public void UpdateRewardUI()
    {
        //APENAS PARA TESTES!! DEPOIS DEVO PENSAR NO BALANCEAMENTO
        image1.sprite = passiveSkillSlot1.image;
        rarity1.text = passiveSkillSlot1.rarity.ToString();

        image2.sprite = passiveSkillSlot2.image;
        rarity2.text = passiveSkillSlot2.rarity.ToString();




        buffTypeA.text = slot1.buffType.ToString();
        incrementA.text = "Increment: " + slot1.increment.ToString();
        rarityA.text = slot1.rarity.ToString();
        imageA.sprite = slot1.image;

        buffTypeB.text = slot2.buffType.ToString();
        incrementB.text = "Increment: " + slot2.increment.ToString();
        rarityB.text = slot2.rarity.ToString();
        imageB.sprite = slot2.image;

        buffTypeC.text = slot3.buffType.ToString();
        incrementC.text = "Increment: " + slot3.increment.ToString();
        rarityC.text = slot3.rarity.ToString();
        imageC.sprite = slot3.image;

        buffTypeD.text = slot4.buffType.ToString();
        incrementD.text = "Increment: " + slot4.increment.ToString();
        rarityD.text = slot4.rarity.ToString();
        imageD.sprite = slot4.image;

        buffTypeE.text = slot5.buffType.ToString();
        incrementE.text = "Increment: " + slot5.increment.ToString();
        rarityE.text = slot5.rarity.ToString();
        imageE.sprite = slot5.image;



        //rewardScreen.gameObject.SetActive(true);  
        GameController.gameController.uiController.rewardPanel.SetActive(true);

        if (GameController.gameController.currentWave % 3 == 0)
            abilitiesChoice.SetActive(true);
        else
            buffsChoice.SetActive(true);

    }

    #region Selecting Reward

    public void SkillSelected1()
    {
        SkillsSelected(passiveSkillSlot1);
        EnableConfirmButton();
    }

    public void SkillSelected2()
    {
        SkillsSelected(passiveSkillSlot2);
        EnableConfirmButton();
    }


    public void RewardSelected1()
    {
        SlotsSelected(slot1);
        EnableConfirmButton();
    }

    public void RewardSelected2()
    {
        SlotsSelected(slot2);
        EnableConfirmButton();
    }

    public void RewardSelected3()
    {
        SlotsSelected(slot3);
        EnableConfirmButton();
    }

    public void RewardSelected4()
    {
        SlotsSelected(slot4);
        EnableConfirmButton();
    }

    public void RewardSelected5()
    {
        SlotsSelected(slot5);
        EnableConfirmButton();
    }

    private void SkillsSelected(Buff slotToCheck)
    {

        slotToCheck.skillSelected = !slotToCheck.skillSelected;

        if (slotToCheck.skillSelected == true)
        {
            skillsSelected++;
        }
        else if (slotToCheck.skillSelected == false)
        {
            skillsSelected--;
        }

        if (skillsSelected >= skillToSelect)
        {
            foreach (Buff slot in slots)
            {
                if (slot.skillSelected == true && slot != slotToCheck)
                {
                    slot.buffSelected = false;
                    skillsSelected--;
                }
            }
        }

        //Se for colocar animação para ele fechar ou abrir, será aqui!!
        abilitiesChoice.SetActive(false);
        buffsChoice.SetActive(true);

    }


    private void SlotsSelected(Buff slotToCheck)
    {

        slotToCheck.buffSelected = !slotToCheck.buffSelected;

        if (slotToCheck.buffSelected == true)
        {
            buffsSelected++;
        }
        else if (slotToCheck.buffSelected == false)
        {
            buffsSelected--;
        }

        if (buffsSelected >= buffToSelect)
        {
            foreach (Buff slot in slots)
            {
                if (slot.buffSelected == true && slot != slotToCheck)
                {
                    slot.buffSelected = false;
                    buffsSelected--;
                }
            }
        }

        //Temporário! Isto causará erro quando o jogador puder escolher mais de uma recompensa
        buffsChoice.SetActive(false);
        ApplyRewards();

    }

    private void EnableConfirmButton()
    {


        if (buffsSelected == buffToSelect && skillsSelected == skillToSelect)
        {
            confirmButton.interactable = true;
        }
        else
            confirmButton.interactable = false;
    }


    #endregion

    public void ApplyRewards()
    {

        foreach (Buff slot in slots)
        {
            if (slot.buffSelected == true)
            {
                slot.UpdateBuffType();
            }
        }

        foreach (Buff slot in skillSlots)
        {
            if (slot.skillSelected == true)
            {
                slot.UpdateBuffType();
            }
        }

        //rewardScreen.gameObject.SetActive(false);
        GameController.gameController.uiController.rewardPanel.SetActive(false);
        GameController.gameController.PseudoStart();
    }

    private Buff EmptySkillSlot(Buff buff)
    {
        if (isPassiveSkillSlot1Assigned == false)
        {
            //Debug.Log("Atribuí o SLOT 1");
            passiveSkillSlot1 = buff;
            isPassiveSkillSlot1Assigned = true;
            skillSlots.Add(passiveSkillSlot1);
            return passiveSkillSlot1;
        }

        if (isPassiveSkillSlot2Assigned == false)
        {
            //Debug.Log("Atribuí o SLOT 2");
            passiveSkillSlot2 = buff;
            isPassiveSkillSlot2Assigned = true;
            skillSlots.Add(passiveSkillSlot2);
            return passiveSkillSlot2;
        }

        //if (isSlotCAssigned == false)
        //{
        //    //Debug.Log("Atribuí o SLOT 3");
        //    slot3 = buff;
        //    isSlot3Assigned = true;
        //    slots.Add(slot3);
        //    return slot3;
        //}        

        return null;
    }

    private Buff EmptySlot(Buff buff)
    {
        if (isSlot1Assigned == false)
        {
            //Debug.Log("Atribuí o SLOT 1");
            slot1 = buff;
            isSlot1Assigned = true;
            slots.Add(slot1);
            return slot1;
        }

        if (isSlot2Assigned == false)
        {
            //Debug.Log("Atribuí o SLOT 2");
            slot2 = buff;
            isSlot2Assigned = true;
            slots.Add(slot2);
            return slot2;
        }

        if (isSlot3Assigned == false)
        {
            //Debug.Log("Atribuí o SLOT 3");
            slot3 = buff;
            isSlot3Assigned = true;
            slots.Add(slot3);
            return slot3;
        }

        if (isSlot4Assigned == false)
        {
            //Debug.Log("Atribuí o SLOT 4");
            slot4 = buff;
            isSlot4Assigned = true;
            slots.Add(slot4);
            return slot4;
        }

        if (isSlot5Assigned == false)
        {
            //Debug.Log("Atribuí o SLOT 5");
            slot5 = buff;
            isSlot5Assigned = true;
            slots.Add(slot5);
            return slot5;
        }

        return null;
    }

    private void ResetSlots()
    {
        isSlot1Assigned = false;
        isSlot2Assigned = false;
        isSlot3Assigned = false;
        isSlot4Assigned = false;
        isSlot5Assigned = false;

        legendaryRewardCount = 0;
        epicRewardCount = 0;
        rareRewardCount = 0;
        uncommonRewardCount = 0;
        commonRewardCount = 0;

    }


    // === UX: hover e click ===
    private void ResetRewardVisuals()
    {
        if (rewardButtons == null) return;

        foreach (var rt in rewardButtons)
        {
            if (rt == null) continue;

            // Cancela qualquer tween que ainda esteja rodando
            LeanTween.cancel(rt.gameObject);

            // Volta para o tamanho padrão
            rt.localScale = Vector3.one;
        }
    }
    public void OnHoverEnter(RectTransform rt)
    {
        AudioController.audioController.onHover.Play();
        LeanTween.scale(rt, Vector3.one * 1.1f, 0.12f).setEaseOutQuad().setIgnoreTimeScale(true);
    }
    public void OnHoverExit(RectTransform rt)
    {
        //PlayHover();
        LeanTween.scale(rt, Vector3.one, 0.12f).setEaseOutQuad().setIgnoreTimeScale(true);
    }

}
