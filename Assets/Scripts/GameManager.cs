using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    [Header("Menu Settings")]
    public int enemyCounter = 0;
    [SerializeField] private GameObject menuHUD;
    [Header("Timer Settings")]
    public float startTime;
    public float currentTime;
    public bool isRunningTime = false;
    [Header("Player Settings")] 
    [SerializeField] GameObject playerPrefab;
    public float barIncrement =10f;


    private void Awake()
    {
        playerPrefab.SetActive(false);
    }

    void Start()
    {
        
        if (isRunningTime == false)
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
        if(isRunningTime == true)
        {
            currentTime = Time.time - startTime;
            menuHUD.SetActive(false);
           
        }
    }
    public void StartTimer()
    {
        isRunningTime = true;
        startTime = Time.time;
        playerPrefab.SetActive(true);
        Debug.Log(playerPrefab.activeSelf);

    }
}
