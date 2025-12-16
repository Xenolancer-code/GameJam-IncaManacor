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
    [SerializeField] private Image iconShield;
    [SerializeField] private Image iconBrokenShield;
    [Header("Settings")]
    [SerializeField] private float maxWidth = 100f;
    private float incWidthBar=5;
    

    private bool showHud = false;

    private void OnEnable()
    {
        MessageCentral.OnStart += ActivateHud;
        MessageCentral.OnDashinActivated += ControllerDashIcons;
        MessageCentral.OnDamagedPlayer += ControllerHPIcons;
    }

    private void OnDisable()
    {   
        MessageCentral.OnStart -= ActivateHud;
        MessageCentral.OnDashinActivated -= ControllerDashIcons;
        MessageCentral.OnDamagedPlayer -= ControllerHPIcons;
    }

    private void Start()
    {
        iconDash.enabled = true;
        iconDashCooldown.enabled = false;
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
        float y = rtfillPowerBar.sizeDelta.y;
        float nuevoAncho = gameManager.sampleAmount;
        rtfillPowerBar.sizeDelta = new Vector2(nuevoAncho, y);
        
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

    private void ControllerHPIcons(bool playerIsDamaged)
    {
        if(!playerIsDamaged)
        {
            iconShield.enabled = true;
            iconBrokenShield.enabled = false;
        }else
        { 
            iconShield.enabled = false;
            iconBrokenShield.enabled = true;
        }
        
    }
    private void ActivateHud()
    {
        showHud = true;
    }
}
