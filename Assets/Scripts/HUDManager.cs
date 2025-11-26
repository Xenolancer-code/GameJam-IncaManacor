using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private PlayerManager playerManager;
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
    [SerializeField] private float maxWidth = 97f;
    [SerializeField] private float aumentoMedidor = 10f;

    private void OnEnable()
    {
        MessageCentral.OnDieEnemy += ReSizePowerBar;
    }

    private void OnDisable()
    {
        MessageCentral.OnDieEnemy -= ReSizePowerBar;
    }


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //playerManager = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
    }

    
    void Update()
    {
        if (gameManager.isRunningTime == true)
        {

            if(!hudElements.activeSelf)
            {
                hudElements.SetActive(true);
            }

            int minutes = Mathf.FloorToInt(gameManager.currentTime / 60);
            int seconds = Mathf.FloorToInt(gameManager.currentTime % 60);

            textTimer.text = minutes.ToString("00")+ " : " + seconds.ToString("00");
            if (playerManager.isDashing == true)
            {
                iconDash.GetComponent<Image>().enabled = true;
                iconDashCooldown.GetComponent<Image>().enabled = false;
            }
            else
            {
                iconDash.GetComponent<Image>().enabled = false;
                iconDashCooldown.GetComponent<Image>().enabled = true;
            }
        }

        textPoints.text = gameManager.enemyCounter.ToString();
       
    }
    public void ReSizePowerBar()
    {//Fer proporciones en %
        float x = rtfillPowerBar.sizeDelta.x;
        float y = rtfillPowerBar.sizeDelta.y;
        float nuevoAncho = Mathf.Max(x + aumentoMedidor, maxWidth);
        //rtfillPowerBar.sizeDelta = new Vector2(x + (10), y);
        rtfillPowerBar.sizeDelta = new Vector2(nuevoAncho, y);
    }
}
