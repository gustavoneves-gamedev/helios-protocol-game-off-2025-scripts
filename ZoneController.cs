using UnityEngine;

public class ZoneController : MonoBehaviour
{
    [Header("Centro e raio da arena")]
    public Transform center;      // arraste o ArenaCenter aqui
    public float radius = 30f;    // raio máximo permitido

    [Header("Referência ao Player")]
    public Transform player;      // arraste o Player aqui

    void LateUpdate()
    {
        if (player == null || center == null)
            return;

        Vector3 pos = player.position;
        Vector3 centerPos = center.position;

        // Considera só o plano XZ (top-down)
        Vector3 offset = new Vector3(pos.x - centerPos.x, 0f, pos.z - centerPos.z);
        float dist = offset.magnitude;

        if (dist > radius)
        {
            // Projeta o player de volta para o círculo
            Vector3 clampedOffset = offset.normalized * radius;
            Vector3 clampedPos = new Vector3(
                centerPos.x + clampedOffset.x,
                pos.y, // mantém a altura
                centerPos.z + clampedOffset.z
            );

            // Se usa CharacterController, prefira mover por Move()
            var controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                Vector3 delta = clampedPos - pos;
                controller.Move(delta);
            }
            else
            {
                player.position = clampedPos;
            }
        }
    }
}
