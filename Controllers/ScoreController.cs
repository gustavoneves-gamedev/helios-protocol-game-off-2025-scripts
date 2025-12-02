using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ScoreController : MonoBehaviour
{
    public int currentWaveScore;      // pontos da onda atual
    public int runTotalScore;         // somatório da run

    public float comboValue;          // acumula “estilo” (0..100)
    public float comboMultiplier = 1f;// aplicado nos pontos
    private float defaultTimeToDecay = 2f;
    private float timeToDecay = 2f;
    public float comboDecayRate = 6f; // por segundo (tuneável)
    //public float comboHitGain = 1f;   // ganho base por acerto (tuneável)
    public float comboNoHitBonus = 15f;// bônus ao finalizar onda sem tomar dano

    // Rank atual
    public enum StyleRank { D, C, B, A, S, SS }
    public StyleRank currentRank = StyleRank.D;

    private UIController uiController;

    
    void Start()
    {
        GameController.gameController.scoreController = this;

        Invoke("InitializeUIController", .1f);
        
    }

    private void InitializeUIController()
    {
        uiController = GameController.gameController.uiController;
    }

    #region Reset Score
    public void ResetScore()
    {
        currentWaveScore = 0;
        comboValue = 0f;
        comboMultiplier = 1f;
        currentRank = StyleRank.D;
        // E chama UpdateTopRightScoreWave/Combo se precisar
    }
    #endregion


    void Update()
    {
        if (uiController == null) return;

        timeToDecay -= Time.deltaTime;
        
        // Decaimento natural do combo
        if (comboValue > 0f && timeToDecay <= 0)
        {
            comboValue = Mathf.Max(0f, comboValue - comboDecayRate * Time.deltaTime);
            RecalcMultiplierAndRank();
            uiController.UpdateTopRightCombo(comboMultiplier, currentRank, comboValue);
        }
    }

    public void IncrementCombo(float basePoints = 0f)
    {

        timeToDecay = defaultTimeToDecay;

        //Debug.Log(basePoints);
        float increment = basePoints / 100f;

                
        // Cresce combo
        comboValue += increment;

        comboValue = Mathf.Min(comboValue, 100f);
        RecalcMultiplierAndRank();

        // Pontos com multiplicador da ONDA (não da run)
        int gained = Mathf.RoundToInt(basePoints * comboMultiplier);
        currentWaveScore += gained;

        uiController.UpdateTopRightScoreWave(currentWaveScore);
        uiController.UpdateTopRightCombo(comboMultiplier, currentRank, comboValue);
    }

    public void OnPlayerDamaged()
    {
        // Quebra forte de combo (ou zera, se quiser mais punitivo)
        comboValue *= 0.35f;
        RecalcMultiplierAndRank();
        uiController.UpdateTopRightCombo(comboMultiplier, currentRank, comboValue);
    }

    public void OnWaveComplete(bool tookDamage)
    {
        if (!tookDamage)
        {
            currentWaveScore += Mathf.RoundToInt(comboNoHitBonus * comboMultiplier);
        }
        runTotalScore += currentWaveScore;

        // reset p/ próxima onda
        currentWaveScore = 0;
        comboValue = 0f;
        comboMultiplier = 1f;
        currentRank = StyleRank.D;

        uiController.UpdateTopRightScoreWave(0);
        uiController.UpdateTopRightCombo(1f, StyleRank.D, 0f);
    }

    void RecalcMultiplierAndRank()
    {
        // Escalonamento suave do multiplicador (1.0 a ~4.0)
        comboMultiplier = 1f + (comboValue / 100f) * 3f;

        // Rank por thresholds
        if (comboValue >= 90f) currentRank = StyleRank.SS;
        else if (comboValue >= 70f) currentRank = StyleRank.S;
        else if (comboValue >= 50f) currentRank = StyleRank.A;
        else if (comboValue >= 35f) currentRank = StyleRank.B;
        else if (comboValue >= 20f) currentRank = StyleRank.C;
        else currentRank = StyleRank.D;
    }


}
