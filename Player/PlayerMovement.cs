using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    private Vector2 aimInput;    
    private CharacterController characterController;
    private Animator animator;   


    [Header("Movement Info")]    
    public float moveSpeed;    
    private Vector3 movementDirection;
    [SerializeField] private float gravityScale = 9.81f;
    private float verticalVelocity;    

    [Header("Dash Info")]
    [SerializeField] private float dashDistance = 20f;    // quantos metros ele anda no dash
    [SerializeField] private float dashDuration = 0.3f; // quanto tempo dura o dash
    [SerializeField] private float dashCooldown = 1f;  // tempo entre dashes
    public bool isDashing;
    private float dashTimer;
    private float lastDashTime;
    private Vector3 dashDirection;

    [Header("Aim Info")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    private Vector3 lookingDirection;

    [Header("Aux Weapon Info")]
    public Transform secondaryWeaponTransform;
    public float rotationSpeed;
    public Transform targetTransform;
    //O tiro da arma secundária será feito na função que a spawnar
    //Provavelmente irei transformar a arma secundária em um objeto à parte do Player

    

    private void Start()
    {
        player = GetComponent<Player>();

        characterController = GetComponent<CharacterController>();

        //Atualiza o status do SPEED
        GameController.gameController.playerSpeed = moveSpeed;

        AssignInputEvents();

    }

    private void Update()
    {
        if (GameController.gameController.isGamePaused)
            return;
        
        ApplyMovement();
        ApplyRotation();

        

    }

    private void TryDash()
    {
        // cooldown
        if (Time.time < lastDashTime + dashCooldown)
            return;

        if (isDashing)
            return;

        // Pega a direção atual do input
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);

        // Se o jogador não estiver mexendo o direcional, dash para frente
        if (inputDir.sqrMagnitude < 0.01f)
        {
            inputDir = transform.forward;
        }

        dashDirection = inputDir.normalized;

        isDashing = true;
        dashTimer = 0f;
        lastDashTime = Time.time;

        // Aqui você pode ligar VFX / som / invencibilidade etc.
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        ApplyGravity();
        // Aqui você pode adaptar se usa rotação da câmera, etc.
        // Ex: se seu jogo é top-down com world space fixo, isso já resolve.

        //if (movementDirection.magnitude > 0)
        //{
        //    characterController.Move(movementDirection * Time.deltaTime * moveSpeed);
        //}

        Vector3 finalVelocity;

        if (isDashing)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer >= dashDuration)
            {
                isDashing = false;
            }

            // velocidade constante = distância total / duração
            finalVelocity = dashDirection * (dashDistance / dashDuration);
            characterController.Move(finalVelocity * Time.deltaTime);
        }
        else
        {
            finalVelocity = movementDirection * moveSpeed;

        }

        characterController.Move(finalVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        
        if (characterController.isGrounded == false)
        {
            verticalVelocity = verticalVelocity - gravityScale * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
    }

    private void ApplyRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Verificar isso aqui, no caso o que está entre parênteses está correto?
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, aimLayerMask))
        {
            lookingDirection = hit.point - transform.position;
            lookingDirection.y = 0;
            lookingDirection.Normalize();

            transform.forward = lookingDirection;
            aim.position = new Vector3(hit.point.x, transform.position.y + 1, hit.point.z);
        }
    }

    
    //Função para atualizar a velocidade do jogador quando ele pegar um buff ou reduzir a velocidade enquanto ele estiver
    //carregando um tiro por exemplo
    public void UpdateSpeed(float speedIncrement)
    {
        //speedBuff += speedIncrement; 
        //normalSpeed += speedIncrement;
        //speed = normalSpeed;
        moveSpeed += speedIncrement;

        //Atualiza o status do SPEED
        GameController.gameController.playerSpeedIncrement = speedIncrement;

    }

    private void SecondWeaponRotation()
    {
        Vector3 direction = targetTransform.position - secondaryWeaponTransform.position;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        secondaryWeaponTransform.rotation = Quaternion.RotateTowards(secondaryWeaponTransform.rotation, targetRotation, rotationSpeed);
                
    }
           


    private void AssignInputEvents()
    {
        inputActions = player.inputActions;

        //inputActions.Player.Attack.performed += context => Shoot();
        //Movi isso para o PlayerWeaponController

        inputActions.Player.Move.performed += context => moveInput = context.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += context => moveInput = Vector2.zero;

        inputActions.Player.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        inputActions.Player.Aim.canceled += context => aimInput = Vector2.zero;

        //Não sei se vou colocar sprint no jogo, talvez um flash apenas
        inputActions.Player.Sprint.performed += context => TryDash();
        //{
        //    moveSpeed *= 2f;
        //    isDashing = true;
        //};

        //inputActions.Player.Sprint.canceled += context =>
        //{
        //    moveSpeed /= 2f;
        //    isDashing = false;
        //};
    }

    
}
