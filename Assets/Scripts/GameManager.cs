using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    [Header("Menu Settings")]
    public int enemyCounter = 0;
    [SerializeField] private GameObject menuHUD;
    [Header("Timer Settings")]
    public float currentTime;
    private bool isGameStarted = false;
    [Header("Player Settings")] 
    [SerializeField] GameObject playerPrefab;
    private const int INITDAMAGE= 10;
    public int INCREMENTDAMAGE = 5; //Cada 10%
    private int finalDamage = INITDAMAGE;
    private const int INITRANGE = 1;
    private const int INCREMENTRANGE = 1; //Cada 30%
    private int finalRange=INITRANGE;
    




    private void OnEnable()
    {
        MessageCentral.OnDieEnemy += IncrementCounter;
    }

    private void OnDisable()
    {
        MessageCentral.OnDieEnemy -= IncrementCounter;
    }

    private void Awake()
    {
        playerPrefab.SetActive(false);
    }

    void Start()
    {
        
        if (isGameStarted == false)
        {
            //Juego pausado
        }
        else
        {
            //Juego empieza
        }
    }

    
    void Update()
    {
        if(isGameStarted == true)
        {
            currentTime += Time.deltaTime;
            menuHUD.SetActive(false);  
        }
    }
    public void StartTimer()
    {
        isGameStarted = true;
        MessageCentral.Start();
        playerPrefab.SetActive(true);
    }

    private void IncrementCounter()
    {
        enemyCounter++;
    }

    public void UpdatePlayerStatus(float powerbar)
    {   
       if(powerbar % 10 == 0)
        {
            finalDamage = finalDamage + INCREMENTDAMAGE;
        }
       if(powerbar % 30 == 0)
        {
            finalRange = finalRange + INCREMENTRANGE;
        }
    }

    public void ResetPlayerStatus()
    {
        finalDamage = INITDAMAGE;
        finalRange = INITRANGE;
    }
}
