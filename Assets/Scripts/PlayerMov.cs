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
    [SerializeField] private float dashCooldown = 2f;



    private CharacterController cc;
    private Animator animator;
    private Vector3 playerVerticalVelocity;
    private bool groundedPlayer;
    private bool groundedPlayerPrev;
    private float gravityValue;
    private bool dashing = false;
    private bool playerIsDead = false;
    
    private Vector3 movmentVector = Vector3.zero;

    private void OnEnable()
    {
        MessageCentral.OnDiePlayer += PlayerisDead;
    }

    private void OnDisable()
    {
        MessageCentral.OnDiePlayer -= PlayerisDead;
    }
    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    
    
    void Update()
    {
        if(playerIsDead) return;
        gravityValue = Physics.gravity.y * gravityMultiplier; //Calcular gravedad
        groundedPlayer = cc.isGrounded; //Revisar si player esta en el suelo
        animator.SetBool("Grounded", groundedPlayer);
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
        animator.SetFloat("velocity", cc.velocity.magnitude);

        groundedPlayerPrev = groundedPlayer;
        
    }
    // private void Landing()
    // {
    //     if(groundedPlayerPrev == false && groundedPlayer == true)
    //     {
    //         animator.SetBool("Landing", true);
    //     }
    // }
    private void Rotate(Vector3 moveDirection)
    {
        // Obtenim la rotaci? referent a una direcci? de moviment. El moviment en que es mour? el personatge.
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        // Interpolem la rotaci? des de la rotaci? actual del transform cap a la rotaci? objectiu
        // uns quants graus cada frame. La velocitat de rotaci? dependr? de 'rotationSpeed'
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void SetMovmentVector(Vector2 inputVector)
    {
        if(playerIsDead) return;
        movmentVector = new Vector3(inputVector.x, 0, inputVector.y);
    }

    public void TrytoJump()
    {
        if (groundedPlayer)
        {
            playerVerticalVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            animator.SetTrigger("Jump");
        }
    }

    public void TryToDash()
    {
        if (dashing) return;
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        dashing = true;
        MessageCentral.DashinActivated(true);
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
        yield return new WaitForSeconds(dashCooldown);
        dashing = false;
        MessageCentral.DashinActivated(false);
    }

    private void PlayerisDead()
    {
        playerIsDead = true;
    }
}
