using System.Collections.Generic;
using UnityEngine;

public class DetectionScript : MonoBehaviour
{

    [SerializeField] private List<Transform> enemiesInRange = new List<Transform>();
    public Transform activeEnemyTransform; //Está em SerializeField apenas para testes
    //[SerializeField] private int arrayCounter;
    private float detectionRange = 50f;


    private void Update()
    {
        ActiveEnemy();
    }

    private void ActiveEnemy()
    {
        activeEnemyTransform = null;
        float minDist = detectionRange;

        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.position);


            if (dist < minDist)
            {
                minDist = dist;
                activeEnemyTransform = enemy;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Enemy"))
        {

            Transform enemyToAdd = other.transform;

            if (!enemiesInRange.Contains(enemyToAdd))
            {
                enemiesInRange.Add(enemyToAdd);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Enemy"))
        {
            UpdateEnemyArray(other.transform);

        }
    }    

    
    public void UpdateEnemyArray(Transform enemyToRemove)
    {
        
        if (enemiesInRange.Contains(enemyToRemove))
        {
            enemiesInRange.Remove(enemyToRemove);
        }

    }



}
