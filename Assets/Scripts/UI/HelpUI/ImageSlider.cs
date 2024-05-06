using System;
using UnityEngine;
using UnityEngine.UI;


public class ImageSlider : MonoBehaviour
{
    public Sprite[] images; // Array to hold references to images
    private int currentIndex = 0;
    public Image imageRef;

    private void Awake()
    {
        imageRef.sprite = images[0];
    }

    public void ShowNextImage()
    {
        currentIndex = (currentIndex + 1) % images.Length;
        ShowImage(currentIndex);
    }

    public void ShowPreviousImage()
    {
        currentIndex = (currentIndex - 1 + images.Length) % images.Length;
        ShowImage(currentIndex);
    }

    private void ShowImage(int index)
    {
        
        // Assign Sprite to the Image component
        imageRef.sprite = images[index];
    }
}
