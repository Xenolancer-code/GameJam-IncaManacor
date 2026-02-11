using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Camera cam;
    public float maxDistance = 100f;
    public LayerMask layerMask;
    private int activeCam = 1;
    private int inactiveCam = 0;
    [Header("Referencia all cameras")]
    [SerializeField] private CinemachineCamera camMenu;
    [SerializeField] private CinemachineCamera camPlay;
    [SerializeField] private CinemachineCamera camExit;
    [SerializeField] private CinemachineCamera camAbout;
    [SerializeField] private CinemachineCamera camSettings;
    

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
                }

                if (hit.collider.gameObject.name == "Cube")
                {
                    camMenu.Priority = activeCam;
                    camAbout.Priority = inactiveCam;
                }
            }
        }
    }
}
