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
    public static int INITDAMAGE= 10;
    public int INCREMENTDAMAGE = 5; //Cada 10%
    private const int INITRANGE = 1;
    private const int INCREMENTRANGE = 1; //Cada 30%
    private int damageTier = 10;
    private int rangeTier = 30;

    public float barWidth;


    private void OnEnable()
    {
        MessageCentral.OnDieEnemy += IncrementCounter;
        MessageCentral.OnIncrementPlayerDamage += IncrementPlayerDamage;
        MessageCentral.OnIncrementPlayerRange += IncrementPlayerRange;
    }

    private void OnDisable()
    {
        MessageCentral.OnDieEnemy -= IncrementCounter;
        MessageCentral.OnIncrementPlayerDamage -= IncrementPlayerDamage;
        MessageCentral.OnIncrementPlayerRange -= IncrementPlayerRange;
    }

    private void Awake()
    {
        playerPrefab.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerPrefab.GetComponent<PlayerAtk>().finalDamage = INITDAMAGE;
        
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
   
    public void IncrementPlayerDamage()
    {
        if (barWidth % damageTier == 0)
        {
            int fdmg = playerPrefab.GetComponent<PlayerAtk>().finalDamage;
            playerPrefab.GetComponent<PlayerAtk>().finalDamage = fdmg + INCREMENTDAMAGE;
        }
    }
    public void IncrementPlayerRange()
    {
        if (barWidth % rangeTier == 0)
        {
            int frg = playerPrefab.GetComponent <PlayerAtk>().finalRange;
            playerPrefab.GetComponent<PlayerAtk>().finalRange = frg + INCREMENTRANGE;
        }
    }



    public void ResetPlayerStatus()
    {
        playerPrefab.GetComponent<PlayerAtk>().finalDamage = INITDAMAGE;
        playerPrefab.GetComponent<PlayerAtk>().finalRange = INITRANGE;
    }


    //Esto ira en otro script(?
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
