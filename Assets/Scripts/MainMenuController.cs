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
                if (hit.collider.gameObject.name == "tapa")
                {
                    SceneManager.LoadScene("GameScene");
                }
                if (hit.collider.gameObject.name == "gramo")
                {
                    camMenu.Priority = inactiveCam;
                    camSettings.Priority = activeCam;
                }

                if (hit.collider.gameObject.name == "Cuadro")
                {
                    camMenu.Priority = inactiveCam;
                    camAbout.Priority = activeCam;
                    splineAbout.CameraPosition=0.5f;
                }

                if (hit.collider.gameObject.name == "Cube")
                {
                    StartCoroutine(ReturnCamMenu());
                }
            }
        }
    }
    private IEnumerator ReturnCamMenu()
    {
        splinePlay.CameraPosition=1f;
        splineSettings.CameraPosition=1f;
        splineExit.CameraPosition=1f;
        splineAbout.CameraPosition=1f;
        yield return new WaitForSeconds(timeSpline);
        camMenu.Priority = activeCam;
        camPlay.Priority = inactiveCam;
        camSettings.Priority = inactiveCam;
        camExit.Priority = inactiveCam;
        camAbout.Priority = inactiveCam;
    } 
}
