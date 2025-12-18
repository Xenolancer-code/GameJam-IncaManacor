using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private int index;
    private int maxImages;
    [Header("Lista")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private RectTransform content;
    [SerializeField] private List<Sprite> images;
    [Header("Conteiner")]
    [SerializeField] private Image imageL;
    [SerializeField] private Image imageC;
    [SerializeField] private Image imageR;
    [Header("Pointers")]
    [SerializeField] private Sprite imagePS;
    [SerializeField] private Sprite imagePU;
    [SerializeField] private List<GameObject> pointers;



    void Start()
    {
        index = 0;
        maxImages = images.Count-1;   

        imageC.sprite = images[0];
        imageR.sprite = images[1];
        imageL.sprite = images[maxImages];

        CreatePointers();
    }
    public void NextSlide()
    {
        index+=1;
        if(index > maxImages)
        {
            index = 0;
        }
       

        UpdateImages();
    }

    public void PreviousSlide()
    {
        index-=1;
        if(index < 0)
        {
            index = maxImages;
        }
        

       UpdateImages();

        //TODO
        //EFECTO PARALLAX
    }

    private void UpdateImages()
    {
        imageC.sprite = images[index];
        if (index <= 0)
        {
            imageL.sprite = images[maxImages];
        }
        else
        {
            imageL.sprite = images[index - 1];
        }

        if (index >= maxImages)
        {
            imageR.sprite = images[0];
        }
        else
        {
            imageR.sprite = images[index + 1];
        }
        for (int i = 0; i < pointers.Count; i++)
        {
            pointers[i].GetComponent<Image>().sprite = imagePU;
        }
        pointers[index].GetComponent<Image>().sprite = imagePS;

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
        pointers[index].GetComponent<Image>().sprite = imagePS;

    }
}
