using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Menu Settings")]
    public int enemyCounter = 0;
    [SerializeField] private GameObject menuHUD;
    [SerializeField] private GameObject pauseHUD;
    [Header("Timer Settings")]
    public float currentTime;
    private bool isGameStarted = false;
    [Header("Player Settings")] 
    [SerializeField] GameObject playerPrefab;
    [SerializeField] HUDManager hudManager;
    public static int INITDAMAGE= 10;
    public int INCREMENTDAMAGE = 5; //Cada 10%
    private const int INITRANGE = 1;
    private const int INCREMENTRANGE = 1; //Cada 30%
    private int damageTier = 10;
    private int rangeTier = 30;
    private PlayerAtk playerAtk;


    public float sampleAmount = 0;
    public float maxSampleAmount = 100;


    private void OnEnable()
    {
        MessageCentral.OnDieEnemy += IncrementCounter;
        MessageCentral.OnPickupSample += UpdateSample;
    }

    private void OnDisable()
    {
        MessageCentral.OnDieEnemy -= IncrementCounter;
        MessageCentral.OnPickupSample -= UpdateSample;
    }

    private void Awake()
    {
        playerPrefab.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
        playerAtk = playerPrefab.GetComponent<PlayerAtk>();
       
        playerAtk.finalDamage = INITDAMAGE;
        
        if (isGameStarted == false)
        {
            //Juego pausado
        }
        else
        {
            //Juego empieza
        }

        hudManager.ReSizePowerBar();
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


    private void UpdateSample(int sampleQuality)
    {
       sampleAmount += sampleQuality;
        if (sampleAmount > maxSampleAmount) sampleAmount = maxSampleAmount;
       IncrementPlayerDamage();
       IncrementPlayerRange();
       hudManager.ReSizePowerBar();

    }
    private void IncrementPlayerDamage()
    {
        int incrementMultiplier = (int)(sampleAmount / damageTier);
        if (incrementMultiplier > 0)
            playerAtk.finalDamage = INITDAMAGE + INCREMENTDAMAGE * incrementMultiplier;
        //if (sampleAmount % damageTier == 0)
        //{
        //   // int fdmg =playerAtk.finalDamage;
        //   //playerAtk.finalDamage = fdmg + INCREMENTDAMAGE;
        //    playerAtk.finalDamage += INCREMENTDAMAGE;
        //}
    }
    private void IncrementPlayerRange()
    {
        int incrementMultiplier = (int)(sampleAmount / rangeTier);
        if (incrementMultiplier > 0)
            playerAtk.finalRange = INITRANGE + INCREMENTRANGE * incrementMultiplier;
        //if (sampleAmount % rangeTier == 0)
        //{
        //    int frg = playerAtk.finalRange;
        //   playerAtk.finalRange = frg + INCREMENTRANGE;
        //}
    }



    public void ResetPlayerStatus()
    {
       playerAtk.finalDamage = INITDAMAGE;
       playerAtk.finalRange = INITRANGE;
    }

    public void ReturnMenu(int index)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseHUD.SetActive(true);
    }
    public void ResumeGame()
    {
     //Revisar como hacer para que no ataque al poner Resume   
        Time.timeScale = 1;
        pauseHUD.SetActive(false);
    }
}
