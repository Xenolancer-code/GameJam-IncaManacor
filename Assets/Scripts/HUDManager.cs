using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI textTimer;
    [SerializeField] private TextMeshProUGUI textPoints;
    [SerializeField] private GameObject hudElements;
    [Header("Icons")]
    [SerializeField] private Image iconDash;
    [SerializeField] private Image iconDashCooldown;
    [SerializeField] private Image fillPowerBar;
    [SerializeField] private RectTransform rtfillPowerBar;
    [Header("Settings")]
    [SerializeField] private float maxWidth = 100f;
    private float barIncrement;

    private bool showHud = false;

    private void OnEnable()
    {
        MessageCentral.OnDamagedEnemy += ReSizePowerBar;
        MessageCentral.OnStart += ActivateHud;
        MessageCentral.OnDashinActivated += ControllerDashIcons;
    }

    private void OnDisable()
    {
        MessageCentral.OnDamagedEnemy -= ReSizePowerBar;
        MessageCentral.OnStart -= ActivateHud;
        MessageCentral.OnDashinActivated -= ControllerDashIcons;
    }

    private void Start()
    {
        iconDash.enabled = true;
        iconDashCooldown.enabled = false;
        barIncrement = gameManager.INCREMENTDAMAGE;
    }

    void Update()
    {
        if (showHud)
            UpdateHUD();

        textPoints.text = gameManager.enemyCounter.ToString();
       
    }
    public void UpdateHUD()
    {
        if (!hudElements.activeSelf)
        {
            hudElements.SetActive(true);
        }

        int minutes = Mathf.FloorToInt(gameManager.currentTime / 60);
        int seconds = Mathf.FloorToInt(gameManager.currentTime % 60);

        textTimer.text = minutes.ToString("00") + " : " + seconds.ToString("00");
        
       
    }
    public void ReSizePowerBar()
    {//Fer proporciones en %
        float x = rtfillPowerBar.sizeDelta.x;
        float y = rtfillPowerBar.sizeDelta.y;
        float nuevoAncho = Mathf.Min(x + barIncrement, maxWidth);
        rtfillPowerBar.sizeDelta = new Vector2(nuevoAncho, y);
        gameManager.UpdatePlayerStatus(nuevoAncho);
    }

    private void ControllerDashIcons(bool isDashing)
    {
        // Usar Message Central
        if (!isDashing)
        {
            iconDash.enabled = true;
            iconDashCooldown.enabled = false;
        }
        else
        {
            iconDash.enabled = false;
            iconDashCooldown.enabled = true;
        }
    }
    private void ActivateHud()
    {
        showHud = true;
    }
}
