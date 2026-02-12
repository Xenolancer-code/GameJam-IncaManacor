using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class MainMenuController : MonoBehaviour
{
    public Camera cam;
    public float maxDistance = 100f;
    public LayerMask layerMask;
    private int activeCam = 1;
    private int inactiveCam = 0;
    [SerializeField] private float timeSpline = 2.5f;
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
                        camMenu.Priority = inactiveCam;
                        camPlay.Priority = activeCam;
                        StartCoroutine((MoveCamWithSpline(splinePlay,1f, timeSpline)));
                        break;
                    case "gramo":
                        camMenu.Priority = inactiveCam;
                        camSettings.Priority = activeCam;
                        StartCoroutine((MoveCamWithSpline(splineSettings,1f, timeSpline)));
                        break;
                    case "key":
                        camMenu.Priority = inactiveCam;
                        camExit.Priority = activeCam;
                        StartCoroutine((MoveCamWithSpline(splineExit,1f, timeSpline)));
                        break;
                    case "Cuadro":
                        camMenu.Priority = inactiveCam;
                        camAbout.Priority = activeCam;
                        StartCoroutine((MoveCamWithSpline(splineAbout,1f, timeSpline)));
                        break;
                    case "Cube":
                        StartCoroutine(ReturnCamMenu());
                        break;
                }
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
    }
    private IEnumerator ReturnCamMenu()
    {
        yield return MoveCamWithSpline(splineAbout,0f, timeSpline);
        camMenu.Priority = activeCam;
        camPlay.Priority = inactiveCam;
        camSettings.Priority = inactiveCam;
        camExit.Priority = inactiveCam;
        camAbout.Priority = inactiveCam;
    } 
}
