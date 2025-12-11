using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManagerInputReceiver : MonoBehaviour
{
    private PlayerControls playerControls;
    private GameManager gameManager;
    

    private void Awake()
    {
        playerControls = new PlayerControls();
        gameManager = GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Menu.PauseHUD.performed += PauseHUD;
    }
    private void PauseHUD(InputAction.CallbackContext ctx)
    {
        if(gameManager == null) return;
        gameManager.PauseGame();
    }

    private void OnDisable()
    {
        playerControls.Menu.PauseHUD.performed -= PauseHUD;
        playerControls.Disable();
    }
}
