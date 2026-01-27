using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public float sampleAmount = 0;
    public float maxSampleAmount = 100;
    [Header("Score")]
    private ScoreReporter reporter;
    [SerializeField] private ScoreData scoreData;
    public int enemyCounter = 0;
    [Header("Menu Settings")]
    [SerializeField] private GameObject tutorialHUD;
    [SerializeField] private GameObject pauseHUD; 
    [SerializeField] private GameObject deadMenu;
    [Header("Timer Settings")]
    public float currentTime;
    private bool isGameStarted = false;
    [SerializeField] private float timeBeforePause = 10f;
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
    [Header("Camera Settings")]
    [SerializeField] private CinemachineCamera tutorialCam;
    [SerializeField] private CinemachineCamera playerCam;


    


    private void OnEnable()
    {
        MessageCentral.OnDieEnemy += IncrementCounter;
        MessageCentral.OnPickupSample += UpdateSample;
        MessageCentral.OnDiePlayer += ObtainScoreData;
        MessageCentral.OnDiePlayer += DiePause;
    }

    private void OnDisable()
    {
        MessageCentral.OnDieEnemy -= IncrementCounter;
        MessageCentral.OnPickupSample -= UpdateSample;
        MessageCentral.OnDiePlayer -= ObtainScoreData;
        MessageCentral.OnDiePlayer -= DiePause;
    }

    private void Awake()
    {
        playerPrefab.SetActive(false);
        DontDestroyOnLoad(gameObject);
        reporter=GetComponent<ScoreReporter>();
    }

    void Start()
    {
        currentTime = 0;
        tutorialCam.Priority = 1;
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
             
        }
    }
    public void StartTimer()
    {
        tutorialHUD.SetActive(false); 
        isGameStarted = true;
        MessageCentral.Start();
        playerPrefab.SetActive(true);
        playerCam.Priority = 2;
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
    //Metodos para calcular el escalado
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

    //Metodos de recopilaci?n de datos
    private void ObtainScoreData()
    {
        scoreData.kills = enemyCounter;
        scoreData.time = currentTime;
    }

    //Metodos sobre el menu de Pausa
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
    public void DiePause()
    {
        StartCoroutine(PlayerDeath());
    }

    private IEnumerator PlayerDeath()
    {
        
        yield return new WaitForSeconds(timeBeforePause);
        Time.timeScale = 0;
        deadMenu.SetActive(true);
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
