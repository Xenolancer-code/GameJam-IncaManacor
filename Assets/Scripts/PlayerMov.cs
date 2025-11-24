using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements.Experimental;
[RequireComponent(typeof(NavMeshModifier))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMov : MonoBehaviour
{
    [Header("Basic movment")]
    [SerializeField] private float playerSpeed = 5.0f; //Velocidad del jugador
    [SerializeField] private float jumpHeight = 1.5f; //Altura de salto(!)
    [SerializeField] private float rotationSpeed = 300; //Velocidad en la que rota
    [SerializeField] private float gravityMultiplier = 1; //Cantidad de gravedad que le afecta(!)

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10;
    [SerializeField] private float dashDuration = 1;



    private CharacterController cc;
    private Vector3 playerVerticalVelocity;
    private bool groundedPlayer;
    private float gravityValue;
    private bool dashing = false;
    private PlayerManager playerManager;
    private Vector3 movmentVector = Vector3.zero;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>();
    }
    private void Start()
    {
       playerManager = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
    }

    void Update()
    {
        gravityValue = Physics.gravity.y * gravityMultiplier; //Calcular gravedad
        groundedPlayer = cc.isGrounded; //Revisar si player esta en el suelo
        if (groundedPlayer && playerVerticalVelocity.y < 0)
        {
            playerVerticalVelocity.y = -0.5f;
        }

 

        if (movmentVector != Vector3.zero)
        {
            Rotate(movmentVector);
        }
        // Apply gravity
        playerVerticalVelocity.y += gravityValue * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = (movmentVector * playerSpeed) + (playerVerticalVelocity.y * Vector3.up);
        cc.Move(finalMove * Time.deltaTime);
    }

    private void Rotate(Vector3 moveDirection)
    {
        // Obtenim la rotació referent a una direcció de moviment. El moviment en que es mourà el personatge.
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        // Interpolem la rotació des de la rotació actual del transform cap a la rotació objectiu
        // uns quants graus cada frame. La velocitat de rotació dependrà de 'rotationSpeed'
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void SetMovmentVector(Vector2 inputVector)
    {
        movmentVector = new Vector3(inputVector.x, 0, inputVector.y);
    }

    public void TrytoJump()
    {
        if (groundedPlayer)
        {
            playerVerticalVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }
    }

    public void TryToDash()
    {
        if (dashing) return;
        if (playerManager.isDashing == false) return;
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        dashing = true;
        float originalSpeed = playerSpeed;
        playerSpeed = dashSpeed;
        float timer = 0;
        
        while (timer <= dashDuration)
        {
            timer += Time.deltaTime;
            float interpolator = timer / dashDuration;
            playerSpeed = Mathf.Lerp(dashSpeed, originalSpeed, interpolator);
            yield return null;
        }
        playerSpeed = originalSpeed;
        playerManager.initDashCooldown();
        dashing = false;
    }
}
