using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReceiver : MonoBehaviour
{
    private PlayerMov playerMov;
    private PlayerAtk playerAtk;
    private PlayerControls playerControls;
    

    private void Awake()
    {
        playerMov = GetComponent<PlayerMov>();
        playerAtk = GetComponent<PlayerAtk>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player_CC.Move.performed += Move;
        playerControls.Player_CC.Move.canceled += Move;
        playerControls.Player_CC.Jump.performed += Jump;
        playerControls.Player_CC.BasicAtk.performed += BasicAtk;
        playerControls.Player_CC.AoEAtk.performed += AoEAtk;
        playerControls.Player_CC.Dash.performed += Dash;
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (playerMov == null) return;
        playerMov.TrytoJump();
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        if (playerMov == null) return;
        Vector2 inputVector = ctx.ReadValue<Vector2>();
        playerMov.SetMovmentVector(inputVector);
    }

    private void BasicAtk(InputAction.CallbackContext ctx)
    {
        if(playerAtk == null) return;
        playerAtk.BasicAtk();
    }

    private void AoEAtk(InputAction.CallbackContext ctx)
    {
        if (playerAtk == null) return;
        playerAtk.AoEAtk();
    }

    private void Dash(InputAction.CallbackContext ctx)
    {
        if(playerMov == null) return;
        playerMov.TryToDash();
    }

    private void OnDisable()
    {
        playerControls.Player_CC.Move.performed -= Move;
        playerControls.Player_CC.Move.canceled -= Move;
        playerControls.Player_CC.Jump.performed -= Jump;
        playerControls.Player_CC.BasicAtk.performed -= BasicAtk;
        playerControls.Player_CC.AoEAtk.performed -= AoEAtk;
        playerControls.Player_CC.Dash.performed -= Dash;
        playerControls.Disable();
    }
}
