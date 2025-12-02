using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 originalVelocity;
    [SerializeField] private float explosionForce = 1f;




    private void OnTriggerStay(Collider other)
    {
        rb = other.gameObject.GetComponent<Rigidbody>();

        if (rb != null && other.gameObject.tag != "Bullet")
        {
            originalVelocity = rb.linearVelocity;
            //Na função de baixo, irei depois fazer uma fórmula inversamente proporcional à magnitude da diferença
            //entre os vetors3 do alvo e do ponto de origem de modo que, quanto mais próximo do centro, maior a força da
            //explosão
            rb.linearVelocity += ((other.transform.position - transform.position) * explosionForce);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (rb != null)
            rb.linearVelocity = originalVelocity;
    }


}
