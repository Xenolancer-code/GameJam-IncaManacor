using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManagerInputReceiver : MonoBehaviour
{
    private PlayerControls playerControls;
    private GameManager gameManager;

    private bool dpadWasNeutral = true;
    private Button currentPauseButton;

    private void Awake()
    {
        playerControls = new PlayerControls();
        gameManager = GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Menu.PauseHUD.performed += OnPauseHUD;
    }

    private void OnDisable()
    {
        playerControls.Menu.PauseHUD.performed -= OnPauseHUD;
        playerControls.Disable();
    }

    private void Update()
    {
        HandlePauseNavigation();
    }

    // ── Pause / Resume ───────────────────────────────────────────────────────
    private void OnPauseHUD(InputAction.CallbackContext ctx)
    {
        if (gameManager == null) return;

        if (gameManager.paused)
            OnResume();
        else
            OnPause();
    }

    private void OnPause()
    {
        gameManager.PauseGame();

        // Seleccionar Resume por defecto al abrir
        currentPauseButton = gameManager.btnResume;
        EventSystem.current.SetSelectedGameObject(gameManager.btnResume.gameObject);
    }

    private void OnResume()
    {
        gameManager.ResumeGame();

        // Limpiar selección al cerrar
        EventSystem.current.SetSelectedGameObject(null);
        currentPauseButton = null;
    }

    // ── Navegación del pause con mando ───────────────────────────────────────
    private void HandlePauseNavigation()
    {
        if (!gameManager.paused) return;

        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // D-Pad arriba/abajo → alternar entre Resume y Exit
        float v = gamepad.dpad.y.ReadValue();
        bool dpadActive = Mathf.Abs(v) > 0.5f;

        if (dpadActive && dpadWasNeutral)
        {
            dpadWasNeutral = false;
            currentPauseButton = currentPauseButton == gameManager.btnResume
                ? gameManager.btnExit
                : gameManager.btnResume;
            EventSystem.current.SetSelectedGameObject(currentPauseButton.gameObject);
        }
        else if (!dpadActive)
        {
            dpadWasNeutral = true;
        }

        // Botón Sur (A/Cross) → confirmar botón seleccionado
        if (gamepad.buttonSouth.wasPressedThisFrame && currentPauseButton != null)
        {
            currentPauseButton.onClick.Invoke();
        }
    }
}