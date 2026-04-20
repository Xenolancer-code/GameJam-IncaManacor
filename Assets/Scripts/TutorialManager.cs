using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

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
    
    private Animator animator;
    void Start()
    {
        pageIndex = 0;
        animator = GetComponent<Animator>();
        animator.Play(pageIndex.ToString());
        maxImages = images.Count-1;   

        imageC.sprite = images[0];
        imageR.sprite = images[1];
        imageL.sprite = images[maxImages];

        CreatePointers();
    }
    public void NextSlide()
    {
        pageIndex+=1;
        if(pageIndex > maxImages)
        {
            pageIndex = 0;
        }
        UpdateImages();
        animator.Play(pageIndex.ToString());
    }

    public void PreviousSlide()
    {
        pageIndex-=1;
        if(pageIndex < 0)
        {
            pageIndex = maxImages;
        }
       UpdateImages();
       animator.Play(pageIndex.ToString());
        //TODO
        //EFECTO PARALLAX
    }

    private void UpdateImages()
    {
        imageC.sprite = images[pageIndex];
        if (pageIndex <= 0)
        {
            imageL.sprite = images[maxImages];
        }
        else
        {
            imageL.sprite = images[pageIndex - 1];
        }

        if (pageIndex >= maxImages)
        {
            imageR.sprite = images[0];
        }
        else
        {
            imageR.sprite = images[pageIndex + 1];
        }
        for (int i = 0; i < pointers.Count; i++)
        {
            pointers[i].GetComponent<Image>().sprite = imagePU;
        }
        pointers[pageIndex].GetComponent<Image>().sprite = imagePS;
    }
    private void CreatePointers() {
        float pointerX=0;
        pointerX=(Screen.width / 2)-((maxImages/2)*(imagePU.rect.width+(imagePU.rect.width/4)*maxImages/2));
    

        for (int i=0; i < images.Count; i++) {
            GameObject go = new GameObject("pointer"+i);
            go.transform.parent = transform;
            go.transform.position = Vector3.zero;
            Image img = go.AddComponent<Image>();
            img.sprite = imagePU;
            go.transform.position = new Vector3(pointerX+(i* (imagePU.rect.width) + imagePU.rect.width / 16), Screen.height / 8, 0);
            pointers.Add(go);
        }
        pointers[pageIndex].GetComponent<Image>().sprite = imagePS;
    }
}
