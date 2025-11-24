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
    private PlayerManager playerManager;
    private GameObject playerInstance;
    [SerializeField] GameObject playerPrefab;


    private void Awake()
    {
        Vector3 playerPos = new Vector3(-1.79f, 0.046f, -12.81f);
        playerInstance = Instantiate(playerPrefab, playerPos, Quaternion.identity);
        playerManager = playerInstance.GetComponent<PlayerManager>();
        playerInstance.transform.GetChild(0).gameObject.SetActive(false);
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
        playerInstance.transform.GetChild(0).gameObject.SetActive(true);

    }
}
