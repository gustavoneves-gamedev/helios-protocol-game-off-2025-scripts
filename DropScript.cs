using UnityEngine;
using UnityEngine.Audio;


public class DropScript : MonoBehaviour
{
    public int dropType;

    [Header("VFX info")]
    [SerializeField] private ParticleSystem burstVFX;
    [SerializeField] private ParticleSystem followVFX;
    [SerializeField] private AudioSource audioSource;

    private float startHeight;
    private float variable = 0.02f;


    void Start()
    {
        startHeight = transform.position.y;
        //PlayVFX();
        Invoke("ReturnToPool", 15f);
    }

    private void FixedUpdate()
    {
        FloatingAnimation();

    }

    private void FloatingAnimation()
    {
        if (transform.position.y >= startHeight + 1f)
            variable = variable * -1;


        if (transform.position.y <= startHeight - 1f)
            variable = variable * -1;


        transform.position = new Vector3(transform.position.x, transform.position.y + variable, transform.position.z);
    }

    private void PlayVFX()
    {
        if (burstVFX == null || followVFX == null || audioSource == null)
            return;

        burstVFX.transform.parent = null;
        followVFX.transform.parent = null;
        //audioSource.transform.parent = null;

        burstVFX.Play();
        followVFX.Play();
        audioSource.Play();
    }


    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null && dropType == 1)
        {

            player.UpdateLifeOverTime(player.maxHealth / 10);
            //player.UpdateLife(player.maxHealth/10);
            PlayVFX();

            //gameObject.SetActive(false);
            //ObjectPool.instance.ReturnObject(gameObject);
        }
        if (player != null && dropType == 2)
        {

            player.UpdateShieldOverTime(player.maxShield / 10);
            //player.UpdateShield(player.maxShield/10);
            //GameController.gameController.AtualizarStatus(0);
            PlayVFX();

            //gameObject.SetActive(false);

            //ObjectPool.instance.ReturnObject(gameObject);
        }
        if (player != null && dropType == 3)
        {

            player.UpdateExplosiveShots(10);

            //gameObject.SetActive(false);

            //ObjectPool.instance.ReturnObject(gameObject);
        }

        //gameObject.SetActive(false);
        if (player != null)
        {
            Invoke("ReturnToPool", 3f);
            gameObject.SetActive(false);

        }
    }

    private void ReturnToPool()
    {
        burstVFX.transform.parent = gameObject.transform;
        followVFX.transform.parent = gameObject.transform;
        //audioSource.transform.parent = gameObject.transform;

        ObjectPool.instance.ReturnObject(gameObject);
    }
}
