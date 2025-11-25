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
    {
        float y = fillPowerBar.GetComponent<RectTransform>().sizeDelta.y;
        float x = fillPowerBar.GetComponent<RectTransform>().sizeDelta.x;
        RectTransform rt = fillPowerBar.GetComponent<RectTransform>();
        rt.sizeDelta= new Vector2(x+(10), y);
    }
}
