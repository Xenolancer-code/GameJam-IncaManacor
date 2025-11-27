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
}
