using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private float yPosition = 22f;
    [SerializeField] private float zPosition = 20f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Player.position.x, Player.position.y + yPosition, Player.position.z - zPosition);
    }
}
