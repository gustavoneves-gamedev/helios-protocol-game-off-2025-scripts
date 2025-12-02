using UnityEngine;

public class BossTurretDetection : MonoBehaviour
{
    [SerializeField] private BossTurrets turretScript;

    private void Start()
    {
        turretScript = GetComponentInChildren<BossTurrets>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            turretScript.isTargetInRange = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            turretScript.isTargetInRange = false;
        }
    }
}
