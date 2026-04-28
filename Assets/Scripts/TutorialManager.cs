using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    private int pageIndex;
    private int maxImages;

    [Header("Lista")]
    [SerializeField] private List<Sprite> images;

    [Header("Conteiner")]
    [SerializeField] private Image imageL;
    [SerializeField] private Image imageC;
    [SerializeField] private Image imageR;

    [Header("Pointers")]
    [SerializeField] private Sprite imagePS;
    [SerializeField] private Sprite imagePU;
    [SerializeField] private List<GameObject> pointers;

    [Header("Gamepad")]
    [SerializeField] private Button btnClose;       // Botón de cerrar tutorial
    [SerializeField] private float stickThreshold = 0.5f;

    private Animator animator;
    private bool stickWasNeutral = true;

    void Start()
    {
        pageIndex = 0;
        animator = GetComponent<Animator>();
        animator.Play(pageIndex.ToString());
        maxImages = images.Count - 1;

        imageC.sprite = images[0];
        imageR.sprite = images[1];
        imageL.sprite = images[maxImages];

        CreatePointers();
    }

    void Update()
    {
        HandleGamepadInput();
    }

    // ── Gamepad ──────────────────────────────────────────────────────────────
    private void HandleGamepadInput()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // Botón Sur (A/Cross) → pulsar el botón de cerrar
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            btnClose.onClick.Invoke();
            return;
        }

        // Left stick izquierda/derecha → cambiar slide
        float h = gamepad.leftStick.x.ReadValue();
        bool stickActive = Mathf.Abs(h) > stickThreshold;

        if (stickActive && stickWasNeutral)
        {
            stickWasNeutral = false;
            if (h > 0)
                NextSlide();
            else
                PreviousSlide();
        }
        else if (!stickActive)
        {
            stickWasNeutral = true;
        }
    }

    // ── Slides ───────────────────────────────────────────────────────────────
    public void NextSlide()
    {
        pageIndex += 1;
        if (pageIndex > maxImages)
            pageIndex = 0;

        UpdateImages();
        animator.Play(pageIndex.ToString());
    }

    public void PreviousSlide()
    {
        pageIndex -= 1;
        if (pageIndex < 0)
            pageIndex = maxImages;

        UpdateImages();
        animator.Play(pageIndex.ToString());
    }

    private void UpdateImages()
    {
        imageC.sprite = images[pageIndex];

        imageL.sprite = pageIndex <= 0
            ? images[maxImages]
            : images[pageIndex - 1];

        imageR.sprite = pageIndex >= maxImages
            ? images[0]
            : images[pageIndex + 1];

        for (int i = 0; i < pointers.Count; i++)
            pointers[i].GetComponent<Image>().sprite = imagePU;

        pointers[pageIndex].GetComponent<Image>().sprite = imagePS;
    }

    private void CreatePointers()
    {
        float scaledWidth = imagePU.rect.width * 0.7f; // Ancho real tras el escalado

        float pointerX = (Screen.width / 2) - ((maxImages / 2) * (scaledWidth + (scaledWidth / 4) * maxImages / 2));

        for (int i = 0; i < images.Count; i++)
        {
            GameObject go = new GameObject("pointer" + i);
            go.transform.parent = transform;
            go.transform.position = Vector3.zero;

            Image img = go.AddComponent<Image>();
            img.sprite = imagePU;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 1f); // Escala a la mitad

            go.transform.position = new Vector3(
                pointerX + (i * scaledWidth + scaledWidth / 16),
                Screen.height / 8,
                0
            );
            pointers.Add(go);
        }
        pointers[pageIndex].GetComponent<Image>().sprite = imagePS;
    }
}