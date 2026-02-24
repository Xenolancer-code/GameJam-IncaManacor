using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEditor.VersionControl;


public class GameManager : MonoBehaviour
{
    [Header("Smoke Particles")]
    [SerializeField] private List<ParticleSystem> smokeScreens = new();
    private int currentSmokeIndex = 0;
    //Hacer lista de estos 3 objetos de forma serializada y que cada vez que se llama al metodo se elimina 1 culaquiera
    [SerializeField] private ParticleSystem portalGlow;
    [SerializeField] private GameObject protector;
    [SerializeField] private GameObject protector2;
    [Header("Drop Settings")]
    [SerializeField] private float dropRadius = 3f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float dropDuration = 0.4f;

    [Header("Score")]
    private ScoreReporter reporter;
    [SerializeField] private ScoreData scoreData;
    public int enemyCounter = 0;
    [Header("Menu Settings")]
    [SerializeField] private GameObject tutorialHUD;
    [SerializeField] private GameObject pauseHUD; 
    [SerializeField] private GameObject deadMenu;
    [SerializeField] HUDManager hudManager;
    [Header("Timer Settings")]
    public float currentTime;
    private bool isGameStarted = false;
    [SerializeField] private float timeBeforePause = 5f;
    [Header("Player Settings")]
    public int sampleAmount = 0;
    public int maxSampleAmount = 100;
    [SerializeField] GameObject player;
    [SerializeField] GameObject dropPrefab;
    
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


    
/*
 * Contador que incremente con cada item
 * y que revise que no incremente de mas al tener sampleAmount>=100
 * y al recibir daño instanciar esa cantidad en forma de drops
 * repartidos aleatoriamente alrededor del player con un Lerp
 */

    private void OnEnable()
    {
        MessageCentral.OnDieEnemy += IncrementCounter;
        MessageCentral.OnPickupSample += UpdateSample;
        MessageCentral.OnDamagedPlayer += EmptyBar;
        MessageCentral.OnDiePlayer += ObtainScoreData;
        MessageCentral.OnDiePlayer += DiePause;
        MessageCentral.OnAllSpawnersDestroyed +=ActivePortalToLight;
        MessageCentral.OnSpawnerDestroyed += SmokeOut;
        MessageCentral.OnSwapScene += SkyboxChange;
    }

    private void OnDisable()
    {
        MessageCentral.OnDieEnemy -= IncrementCounter;
        MessageCentral.OnPickupSample -= UpdateSample;
        MessageCentral.OnDamagedPlayer -= EmptyBar;
        MessageCentral.OnDiePlayer -= ObtainScoreData;
        MessageCentral.OnDiePlayer -= DiePause;
        MessageCentral.OnAllSpawnersDestroyed -=ActivePortalToLight;
        MessageCentral.OnSpawnerDestroyed -= SmokeOut;
        MessageCentral.OnSwapScene -= SkyboxChange;
    }

    private void Awake()
    {
        player.SetActive(false);
        DontDestroyOnLoad(gameObject);
        reporter=GetComponent<ScoreReporter>();
    }

    void Start()
    {
        currentTime = 0;
        tutorialCam.Priority = 1;
        playerAtk = player.GetComponent<PlayerAtk>();
       
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

    private void SkyboxChange()
    {
        
    }
    
    
    
    
    public void StartTimer()
    {
        tutorialHUD.SetActive(false); 
        isGameStarted = true;
        MessageCentral.Start();
        player.SetActive(true);
        playerCam.Priority = 2;
    }

    private void IncrementCounter()
    {
        enemyCounter++;
    }

    private void UpdateSample(int sampleQuality)
    {
       sampleAmount += sampleQuality;
        if (sampleAmount > maxSampleAmount) sampleAmount = maxSampleAmount; //if (sampleAmount >= maxSampleAmount) return;
       IncrementPlayerDamage();
       IncrementPlayerRange();
       hudManager.ReSizePowerBar();
       BarIsFull();
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
    // Metodos para manejar si la barra esta llena o vacia
    private void BarIsFull()
    {
        playerAtk.canAoe = sampleAmount >= maxSampleAmount;
    }

    private void EmptyBar(bool playerIsDamaged)
    {
        if (playerIsDamaged)
        {
            int dropAmount = sampleAmount / 20;
            sampleAmount = 0;
            hudManager.ReSizePowerBar();

            for (int i = 0; i < dropAmount; i++)
            {
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * dropRadius;
                Vector3 randomOffset = new Vector3(randomCircle.x, 0, randomCircle.y);

                Vector3 targetPos = player.transform.position + randomOffset;

                GameObject drop = Instantiate(dropPrefab, player.transform.position, Quaternion.identity);

                StartCoroutine(JumpDrop(drop.transform, targetPos));
            }

        }
    }
    private IEnumerator JumpDrop(Transform drop, Vector3 targetPos)
    {
        float timer = 0f;
        Vector3 startPos = drop.position;

        Collider col = drop.GetComponent<Collider>();
        if (col != null)
            col.enabled = false; 
        while (timer < dropDuration)
        {
            float interpolator = timer / dropDuration;

            // Movimiento horizontal normal
            Vector3 horizontalPos = Vector3.Lerp(startPos, targetPos, interpolator);

            // Curva parabólica para el salto
            float height = 4 * jumpHeight * interpolator * (1 - interpolator);

            drop.position = new Vector3(
                horizontalPos.x,
                horizontalPos.y + height,
                horizontalPos.z
            );

            timer += Time.deltaTime;
            yield return null;
        }

        drop.position = targetPos;
        yield return new WaitForSeconds(0.2f);
        if (col != null)
            col.enabled = true;
    }
    private void ActivePortalToLight()
    {
        Destroy(protector);
        Destroy(protector2);
        portalGlow.Play();
    }

    public void SmokeOut()
    {
        if (currentSmokeIndex >= smokeScreens.Count)
            return;

        ParticleSystem currentSmoke = smokeScreens[currentSmokeIndex];

        if (currentSmoke != null)
        {
            currentSmoke.Stop();
        }
        currentSmokeIndex++;
    }

    //Metodos de recopilación de datos
    private void ObtainScoreData()
    {
        //TODO: Calcular puntuación con diferentes variables
        
        reporter.SubmitScore(scoreData.name,enemyCounter,scoreData.api_token);
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
    
    public void ExitGame()
    {
        Application.Quit();
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
