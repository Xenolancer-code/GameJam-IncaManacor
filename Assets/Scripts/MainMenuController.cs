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
    [SerializeField] private CinemachineCamera menucam;
    [SerializeField] private CinemachineCamera settingscam;
    [SerializeField] private Animator animatorGramo;

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
                    settingscam.Priority = inactiveCam;
                    menucam.Priority = activeCam;
                    animatorGramo.SetBool("Settings",false);
                }
            }
        }
    }
}
