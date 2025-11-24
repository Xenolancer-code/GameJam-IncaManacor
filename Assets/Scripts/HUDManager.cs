using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI textTimer;
    [SerializeField] private TextMeshProUGUI textPoints;
    [SerializeField] private GameObject hudElements;
    [Header("Icons")]
    [SerializeField] private Image iconDash;
    [SerializeField] private Image iconDashCooldown;
    private GameManager gameManager;
    private PlayerManager playerManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerManager = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
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
}
