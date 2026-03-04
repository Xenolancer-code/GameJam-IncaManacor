using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Camera cam;
    public float maxDistance = 100f;
    public LayerMask layerMask;
    private int activeCam = 1;
    private int inactiveCam = 0;
    [SerializeField] private float timeSpline = 2f;
    private bool cameraReachedEnd= false;

    [Header("Referencias SoundCanvas")] 
    [SerializeField] private Button btnMusic;
    [SerializeField] private Button btnFX;
    [SerializeField] private Slider volumeMusic;
    [SerializeField] private Slider volumeFX;
    [Header("Referencia all Cameras")]
    [SerializeField] private CinemachineCamera camMenu;
    [SerializeField] private CinemachineCamera camPlay;
    [SerializeField] private CinemachineCamera camExit;
    [SerializeField] private CinemachineCamera camAbout;
    [SerializeField] private CinemachineCamera camSettings;
    [Header("Referencia all Spline")]
    [SerializeField] private CinemachineSplineDolly splinePlay;
    [SerializeField] private CinemachineSplineDolly splineExit;
    [SerializeField] private CinemachineSplineDolly splineAbout;
    [SerializeField] private CinemachineSplineDolly splineSettings;
    

    public void Exit()
    {
        Application.Quit();
    }

    private void Start()
    {
        btnMusic.interactable = false;
        btnFX.interactable = false;
        volumeMusic.interactable = false;
        volumeFX.interactable = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                string objectClickedName = hit.collider.gameObject.name;

                switch (objectClickedName)      
                {
                    case "tapa":
                        //SceneManager.LoadScene("GameScene");
                        if (cameraReachedEnd)
                        {
                            SceneManager.LoadScene("GameScene");
                        }
                        else
                        {
                            camMenu.Priority = inactiveCam;
                            camPlay.Priority = activeCam;
                            StartCoroutine((MoveCamWithSpline(splinePlay,1f, timeSpline)));
                        }
                        break;  
                    case "gramo":
                        if (cameraReachedEnd)
                        {
                            btnMusic.interactable = true;
                            btnFX.interactable = true;
                            volumeMusic.interactable = true;
                            volumeFX.interactable = true;
                        }
                        else
                        {
                            camMenu.Priority = inactiveCam;
                            camSettings.Priority = activeCam;
                            StartCoroutine((MoveCamWithSpline(splineSettings,1f, timeSpline)));    
                        }
                        break;
                    case "key":
                        if (cameraReachedEnd)
                        {
                            Application.Quit();
                        }
                        else
                        {
                            camMenu.Priority = inactiveCam;
                            camExit.Priority = activeCam;
                            StartCoroutine((MoveCamWithSpline(splineExit,1f, timeSpline)));
                        }
                        
                        break;
                    case "Cuadro":
                        camMenu.Priority = inactiveCam;
                        camAbout.Priority = activeCam;
                        StartCoroutine((MoveCamWithSpline(splineAbout,1f, timeSpline)));
                        break;
                }
            }
            else
            {
                cameraReachedEnd = false;
                StartCoroutine(ReturnToMenu(
                    camMenu,
                    new CinemachineCamera[] { camPlay, camSettings, camExit, camAbout },
                    new CinemachineSplineDolly[] { splinePlay, splineSettings, splineExit, splineAbout },
                    timeSpline
                ));
            }
        }
    }

    private IEnumerator MoveCamWithSpline(CinemachineSplineDolly spline,float target, float duration)
    {
        float start = spline.CameraPosition;;
        //float end = 1f;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float interpolator = (timer / duration);

            spline.CameraPosition = Mathf.Lerp(start, target, interpolator);

            yield return null;
        }

        spline.CameraPosition = target;
        cameraReachedEnd = target >= 0.9;
    }
    private IEnumerator ReturnToMenu(
        CinemachineCamera menuCam,
        CinemachineCamera[] otherCams,
        CinemachineSplineDolly[] splines,
        float duration)
    {
        menuCam.Priority = activeCam;
        btnMusic.interactable = false;
        btnFX.interactable = false;
        volumeMusic.interactable = false;
        volumeFX.interactable = false;
        
        foreach (var cam in otherCams)
        {
            cam.Priority = inactiveCam;
        }
        
        foreach (var spline in splines)
        {
            yield return MoveCamWithSpline(spline, 0f, duration);
        }
    }

}
