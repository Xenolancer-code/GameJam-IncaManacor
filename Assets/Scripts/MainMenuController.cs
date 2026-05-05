using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour, PlayerControls.IUIActions
{
    public Camera cam;
    public float maxDistance = 100f;
    public LayerMask layerMask;
    private int activeCam = 1;
    private int inactiveCam = 0;
    [SerializeField] private float timeSpline = 2f;
    private bool cameraReachedEnd = false;

    [Header("Referencias SoundCanvas")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private Button btnMusic;
    [SerializeField] private Button btnFX;
    [SerializeField] private Slider volumeMusic;
    [SerializeField] private Slider volumeFX;

    [Header("Referencia all Cameras")]
    [SerializeField] private CinemachineCamera camMenu;
    [SerializeField] private CinemachineCamera camPlay;
    [SerializeField] private CinemachineCamera camExit;
    [SerializeField] private CinemachineCamera camAbout;
    [SerializeField] private CinemachineCamera camSettings;

    [Header("Referencia all Spline")]
    [SerializeField] private CinemachineSplineDolly splinePlay;
    [SerializeField] private CinemachineSplineDolly splineExit;
    [SerializeField] private CinemachineSplineDolly splineAbout;
    [SerializeField] private CinemachineSplineDolly splineSettings;

    [Header("Gamepad Navigation")]
    [SerializeField] private float stickThreshold = 0.5f;

    [Header("Outline y Animators")]
    [SerializeField] private Outline outlineBook;
    [SerializeField] private Outline outlineGramofono;
    [SerializeField] private Animator animatorBook;
    [SerializeField] private Animator animatorGramofono;
    [SerializeField] private float outlineWidthSelected = 10f;
    [SerializeField] private float outlineWidthDefault = 3f;

    [Header("Settings Navigation")]
    [SerializeField] private float sliderStep = 0.05f;
    [SerializeField] private Image highlightMusic;
    [SerializeField] private Image highlightFX;
    [SerializeField] private Button[] settingsButtons;

    [Header("Score")] 
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private TMPro.TMP_Text scoreText;
    private ScoreReporter scoreReporter;
    

    [Header("Player Name")]
    [SerializeField] private LetterSlot[] letterSlots; // 5 casillas en el Inspector
    private int[] letterIndices = new int[5];           // índice de letra actual por casilla
    private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ #$€%?!&()·~¬-_<>*";
    private string playerName = "";
    [System.Serializable]
    public struct LetterSlot
    {
        public TMPro.TMP_Text textUp;     // letra anterior
        public TMPro.TMP_Text textCenter; // letra actual
        public TMPro.TMP_Text textDown;   // letra posterior
    }
    
    // Ciclo: izquierda → -1 (Play→About→Settings→Exit)
    //        derecha   → +1 (Play→Exit→Settings→About)
    private enum MenuOption { Play, About, Settings, Exit }
    private readonly MenuOption[] cycleOrder =
        { MenuOption.Play, MenuOption.About, MenuOption.Settings, MenuOption.Exit };

    private enum SettingsOption { Music, FX, Buttons }
    private SettingsOption currentSettingsOption = SettingsOption.Music;
    private bool isInSettingsMode = false;
    private int currentButtonIndex = 0;
    private const int totalButtons = 5;

    private int currentIndex = -1;
    private bool isNavigating = false;
    private bool stickWasNeutral = true;
    private bool dpadWasNeutral = true;
    private bool submitCooldown = false;

    private PlayerControls controls;

    // ── Ciclo de vida ────────────────────────────────────────────────────────
    private void Awake()
    {
        controls = new PlayerControls();
        controls.UI.SetCallbacks(this);
        scoreReporter=GetComponent<ScoreReporter>();
    }

    private void OnEnable()
    {
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Disable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void Start()
    {
        btnMusic.interactable    = false;
        btnFX.interactable       = false;
        volumeMusic.interactable = false;
        volumeFX.interactable    = false;
        
        // Inicializar casillas de letras
        for (int i = 0; i < letterSlots.Length; i++)
        {
            letterIndices[i] = PlayerPrefs.GetInt($"LetterIndex_{i}", 0);  // empieza en 'A'
            UpdateSlotTexts(i);
        }
        UpdatePlayerName();

        FetchClassification(1);
    }

    // ── IUIActions ───────────────────────────────────────────────────────────
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            stickWasNeutral = true;
            return;
        }

        if (!context.performed) return;
        if (isInSettingsMode) return;
        if (isNavigating) return;
        if (!stickWasNeutral) return;

        Vector2 input = context.ReadValue<Vector2>();
        if (Mathf.Abs(input.x) <= stickThreshold) return;

        stickWasNeutral = false;

        if (currentIndex == -1)
        {
            ActivateOption(0);
            return;
        }

        int direction = input.x < 0 ? 1 : -1;
        int nextIndex = (currentIndex + direction + cycleOrder.Length) % cycleOrder.Length;
        ActivateOption(nextIndex);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (submitCooldown) return;

        // Si estamos en botones → invocar el botón seleccionado
        if (isInSettingsMode && currentSettingsOption == SettingsOption.Buttons)
        {
            if (settingsButtons.Length > 0)
                settingsButtons[currentButtonIndex].onClick.Invoke();
            return;
        }

        if (isInSettingsMode) return;
        if (!cameraReachedEnd) return;

        ConfirmSelection();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!isInSettingsMode) return;

        ExitSettingsMode();
    }

    public void OnVolumenControl(InputAction.CallbackContext context)
{
    if (!isInSettingsMode) return;

    if (context.canceled)
    {
        dpadWasNeutral = true;
        return;
    }

    if (!context.performed) return;
    if (!dpadWasNeutral) return;

    Vector2 input = context.ReadValue<Vector2>();
    dpadWasNeutral = false;

    // ── Movimiento vertical: navegar entre Music → FX → Buttons ─────────
    if (Mathf.Abs(input.y) > Mathf.Abs(input.x) && Mathf.Abs(input.y) > stickThreshold)
    {
        if (input.y > 0) // Arriba
        {
            switch (currentSettingsOption)
            {
                case SettingsOption.FX:
                    currentSettingsOption = SettingsOption.Music;
                    break;
                case SettingsOption.Buttons:
                    SwapLetterUp();
                    break;
            }
        }
        else // Abajo
        {
            switch (currentSettingsOption)
            {
                case SettingsOption.Music:
                    currentSettingsOption = SettingsOption.FX;
                    break;
                case SettingsOption.FX:
                    currentSettingsOption = SettingsOption.Buttons;
                    currentButtonIndex = 0;
                    break;
                case SettingsOption.Buttons:
                    SwapLetterDown();
                    break;
            }
        }
        UpdateSettingsHighlight();
        return;
    }

    // ── Movimiento horizontal ────────────────────────────────────────────
    if (Mathf.Abs(input.x) > stickThreshold)
    {
        float delta = input.x > 0 ? sliderStep : -sliderStep;

        switch (currentSettingsOption)
        {
            // Sliders: izquierda/derecha modifica el valor
            case SettingsOption.Music:
                volumeMusic.value = Mathf.Clamp01(volumeMusic.value + delta);
                break;
            case SettingsOption.FX:
                volumeFX.value = Mathf.Clamp01(volumeFX.value + delta);
                break;
            // Botones: izquierda/derecha navega entre ellos
            case SettingsOption.Buttons:
                int direction = input.x > 0 ? 1 : -1;
                currentButtonIndex = Mathf.Clamp(currentButtonIndex + direction, 0, totalButtons - 1);
                UpdateSettingsHighlight();
                break;
        }
    }
}

    // Métodos de IUIActions que no usamos — implementación vacía obligatoria
    public void OnPoint(InputAction.CallbackContext context) { }
    public void OnClick(InputAction.CallbackContext context) { }
    public void OnScrollWheel(InputAction.CallbackContext context) { }
    public void OnMiddleClick(InputAction.CallbackContext context) { }
    public void OnRightClick(InputAction.CallbackContext context) { }
    public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }

    // ── Input de ratón ───────────────────────────────────────────────────────
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            int clickedIndex = hit.collider.gameObject.name switch
            {
                "tapa"   => 0, // Play
                "Cuadro" => 1, // About
                "gramo"  => 2, // Settings
                "key"    => 3, // Exit
                _        => -1
            };

            if (clickedIndex == -1) return;

            if (clickedIndex == currentIndex && cameraReachedEnd)
                ConfirmSelection();
            else
                ActivateOption(clickedIndex);
        }
        else
        {
            GoToMainMenu();
        }
    }

    // ── Settings mode ────────────────────────────────────────────────────────
    private void EnterSettingsMode()
    {
        isInSettingsMode = true;
        currentSettingsOption = SettingsOption.Music;
        currentButtonIndex = 0;
        dpadWasNeutral = true;
        submitCooldown = true;
        StartCoroutine(ClearSubmitCooldown());

        btnMusic.interactable    = true;
        btnFX.interactable       = true;
        volumeMusic.interactable = true;
        volumeFX.interactable    = true;

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        UpdateSettingsHighlight();
    }
    private IEnumerator ClearSubmitCooldown()
    {
        yield return null; // esperar un frame
        submitCooldown = false;
    }

    private void ExitSettingsMode()
    {
        isInSettingsMode = false;

        btnMusic.interactable    = false;
        btnFX.interactable       = false;
        volumeMusic.interactable = false;
        volumeFX.interactable    = false;

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        
        if (highlightMusic != null) highlightMusic.enabled = false;
        if (highlightFX != null)    highlightFX.enabled    = false;
    }

    private void UpdateSettingsHighlight()
    {
        if (highlightMusic != null)
            highlightMusic.enabled = currentSettingsOption == SettingsOption.Music;
        if (highlightFX != null)
            highlightFX.enabled = currentSettingsOption == SettingsOption.FX;

        // Highlight de botones — usa el Selected color del EventSystem
        if (currentSettingsOption == SettingsOption.Buttons && settingsButtons.Length > 0)
            UnityEngine.EventSystems.EventSystem.current
                .SetSelectedGameObject(settingsButtons[currentButtonIndex].gameObject);
        else
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    // ── Outline y Animators ──────────────────────────────────────────────────
    private void ApplySelectedVisuals(MenuOption option)
    {
        ClearAllVisuals();

        switch (option)
        {
            case MenuOption.Play:
                if (outlineBook != null)  outlineBook.OutlineWidth = outlineWidthSelected;
                if (animatorBook != null) animatorBook.SetBool("Close2", false);
                soundManager.PlayMenuOptionsMusic(0,true);
                break;
            case MenuOption.Settings:
                if (outlineGramofono != null)  outlineGramofono.OutlineWidth = outlineWidthSelected;
                if (animatorGramofono != null) animatorGramofono.SetBool("Settings", true);
                soundManager.PlayMenuOptionsMusic(1,true);
                break;
            case MenuOption.About:
                soundManager.PlayMenuOptionsMusic(2,true);
                break;
            case MenuOption.Exit:
                soundManager.PlayMenuOptionsMusic(3,true);
                break;
        }
    }

    private void ClearAllVisuals()
    {
        soundManager.StopMenuOptionsMusic();
        if (outlineBook != null)
        {
            outlineBook.OutlineWidth = outlineWidthDefault;
            animatorBook.SetBool("Close2", true);
        }
        if (outlineGramofono != null)
        {
            outlineGramofono.OutlineWidth = outlineWidthDefault;
            animatorGramofono.SetBool("Settings", false);
        }
    }

    // ── Activar opción por índice ────────────────────────────────────────────
    private void ActivateOption(int index)
    {
        if (isNavigating) return;

        if (isInSettingsMode) ExitSettingsMode();

        currentIndex = index;
        ResetAllCameraPriorities();
        ApplySelectedVisuals(cycleOrder[index]);

        switch (cycleOrder[index])
        {
            case MenuOption.Play:
                camPlay.Priority = activeCam;
                StartCoroutine(MoveCamWithSpline(splinePlay, 1f, timeSpline));
                break;
            case MenuOption.About:
                camAbout.Priority = activeCam;
                StartCoroutine(MoveCamWithSpline(splineAbout, 1f, timeSpline));
                break;
            case MenuOption.Settings:
                camSettings.Priority = activeCam;
                StartCoroutine(MoveCamWithSpline(splineSettings, 1f, timeSpline));
                break;
            case MenuOption.Exit:
                camExit.Priority = activeCam;
                StartCoroutine(MoveCamWithSpline(splineExit, 1f, timeSpline));
                break;
        }
    }

    // ── Confirmar la opción actual ───────────────────────────────────────────
    private void ConfirmSelection()
    {
        if (currentIndex == -1) return;

        switch (cycleOrder[currentIndex])
        {
            case MenuOption.Play:
                SceneManager.LoadScene("GameScene");
                break;
            case MenuOption.Settings:
                EnterSettingsMode();
                break;
            case MenuOption.Exit:
                Application.Quit();
                break;
        }
    }

    // ── Volver al menú principal ─────────────────────────────────────────────
    private void GoToMainMenu()
    {
        if (isInSettingsMode) ExitSettingsMode();

        currentIndex = -1;
        cameraReachedEnd = false;
        ClearAllVisuals();
        StartCoroutine(ReturnToMenu(
            camMenu,
            new CinemachineCamera[]      { camPlay, camSettings, camExit, camAbout },
            new CinemachineSplineDolly[] { splinePlay, splineSettings, splineExit, splineAbout },
            timeSpline
        ));
    }

    private void ResetAllCameraPriorities()
    {
        camMenu.Priority     = inactiveCam;
        camPlay.Priority     = inactiveCam;
        camSettings.Priority = inactiveCam;
        camExit.Priority     = inactiveCam;
        camAbout.Priority    = inactiveCam;
        cameraReachedEnd     = false;
    }
    // ── Nombre del jugador ───────────────────────────────────────────────────────
    private void SwapLetterUp()
    {
        // Subir → letra anterior en el alfabeto (A→Z)
        letterIndices[currentButtonIndex] = 
            (letterIndices[currentButtonIndex] - 1 + ALPHABET.Length) % ALPHABET.Length;

        UpdateSlotTexts(currentButtonIndex);
        UpdatePlayerName();
    }
    public void SwapLetterUpCLick(int i)
    {
        // Subir → letra anterior en el alfabeto (A→Z)
        letterIndices[i] = 
            (letterIndices[i] - 1 + ALPHABET.Length) % ALPHABET.Length;

        UpdateSlotTexts(i);
        UpdatePlayerName();
    }

    private void SwapLetterDown()
    {
        // Bajar → letra siguiente en el alfabeto (Z→A)
        letterIndices[currentButtonIndex] = 
            (letterIndices[currentButtonIndex] + 1) % ALPHABET.Length;

        UpdateSlotTexts(currentButtonIndex);
        UpdatePlayerName();
    }
    public void SwapLetterDownClick(int i)
    {
        // Bajar → letra siguiente en el alfabeto (Z→A)
        letterIndices[i] = 
            (letterIndices[i] + 1) % ALPHABET.Length;

        UpdateSlotTexts(i);
        UpdatePlayerName();
    }

    private void UpdateSlotTexts(int slotIndex)
    {
        int current  = letterIndices[slotIndex];
        int previous = (current - 1 + ALPHABET.Length) % ALPHABET.Length;
        int next     = (current + 1) % ALPHABET.Length;

        letterSlots[slotIndex].textUp.text     = ALPHABET[previous].ToString();
        letterSlots[slotIndex].textCenter.text = ALPHABET[current].ToString();
        letterSlots[slotIndex].textDown.text   = ALPHABET[next].ToString();
    }

    private void UpdatePlayerName()
    {
        playerName = "";
        for (int i = 0; i < letterSlots.Length; i++)
        {
            playerName += ALPHABET[letterIndices[i]];
            PlayerPrefs.SetInt($"LetterIndex_{i}", letterIndices[i]);
        }
        PlayerPrefs.Save();
        scoreData.name = playerName;
        Debug.Log($"Player name: {playerName}");
    }
    // ── Corrutinas ───────────────────────────────────────────────────────────
    private IEnumerator MoveCamWithSpline(CinemachineSplineDolly spline, float target, float duration)
    {
        isNavigating = true;
        float start = spline.CameraPosition;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            spline.CameraPosition = Mathf.Lerp(start, target, timer / duration);
            yield return null;
        }

        spline.CameraPosition = target;
        cameraReachedEnd = target >= 0.9f;
        isNavigating = false;
        stickWasNeutral = true;
    }

    private IEnumerator ReturnToMenu(
        CinemachineCamera menuCam,
        CinemachineCamera[] otherCams,
        CinemachineSplineDolly[] splines,
        float duration)
    {
        isNavigating = true;
        menuCam.Priority = activeCam;

        btnMusic.interactable    = false;
        btnFX.interactable       = false;
        volumeMusic.interactable = false;
        volumeFX.interactable    = false;

        foreach (var c in otherCams) c.Priority = inactiveCam;
        foreach (var s in splines)   StartCoroutine(MoveCamWithSpline(s, 0f, duration));

        yield return new WaitForSeconds(duration);

        isNavigating = false;
    }
    //-SCORE AUXILIAR

    private void FetchClassification(int top)
    {
        scoreReporter.GetClassification(scoreData.api_token,top,OnClassificationReceived);
    }

    private void OnClassificationReceived(ScoreReporter.ScoreEntry[] entries)
    {
        if (entries == null || entries.Length == 0)
        {
            scoreText.text = "Sin puntuaciones";
            return;
        }
        var entry = entries[0];
        int minutos  = entry.puntuacion / 60;
        int segundos = entry.puntuacion % 60;

        scoreText.text = $"{entry.name}  {minutos:00}:{segundos:00}";
    }
}