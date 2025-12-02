using UnityEngine;

public class PlayerMenu : MonoBehaviour
{
    private float startHeight;

    private float variable = 0.001f;


    void Start()
    {
        startHeight = transform.position.y;
    }

    private void FixedUpdate()
    {
        FloatingAnimation();
        //Verificar se isso pesa muito no jogo!!!
    }

    private void FloatingAnimation()
    {
        if (transform.position.y >= startHeight + .1f)
            variable = variable * -1;


        if (transform.position.y <= startHeight - .1f)
            variable = variable * -1;


        transform.position = new Vector3(transform.position.x, transform.position.y + variable, transform.position.z);
    }
}
