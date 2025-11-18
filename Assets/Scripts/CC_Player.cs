using Unity.AI.Navigation;
using UnityEngine;
[RequireComponent(typeof(NavMeshModifier))]
[RequireComponent(typeof(CharacterController))]
public class CC_Player : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5.0f; //Velocidad del jugador
    [SerializeField] private float jumpHeight = 1.5f; //Altura de salto(!)
    [SerializeField] private float rotationSpeed = 300; //Velocidad en la que rota
    [SerializeField] private float gravityMultiplier = 1; //Cantidad de gravedad que le afecta(!)

    private CharacterController cc;
    private Vector3 playerVerticalVelocity;
    private bool groundedPlayer;
    private float gravityValue;

    private void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        gravityValue = Physics.gravity.y * gravityMultiplier; //Calcular gravedad
        groundedPlayer = cc.isGrounded; //Revisar si player esta en el suelo
        if (groundedPlayer && playerVerticalVelocity.y < 0)
        {
            playerVerticalVelocity.y = -0.5f;
        }

        // Read input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(moveX, 0, moveZ);
        move = Vector3.ClampMagnitude(move, 1f);

        if (move != Vector3.zero)
        {
            Rotate(move);
        }

        bool jumpAction = Input.GetButtonDown("Jump");
        // Jump
        if (jumpAction && groundedPlayer)
        {
            playerVerticalVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        // Apply gravity
        playerVerticalVelocity.y += gravityValue * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = (move * playerSpeed) + (playerVerticalVelocity.y * Vector3.up);
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
}
