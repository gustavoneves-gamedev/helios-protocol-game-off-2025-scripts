using UnityEngine;

[System.Serializable]
public class WaveInfo
{
    [Header("Configurações da Onda")]
    public int portalsToActivate = 1;

    [Tooltip("Total de inimigos que aparecerão na onda inteira")]
    public int totalEnemies = 5;

    [Header("Quais tipos de inimigos podem aparecer?")]
    public bool useDasher = true;
    public bool useRanger = true;
    public bool useFlameThrower = true;

    [Header("Limite de inimigos por portal")]
    public int enemiesPerPortal = 3;
}
