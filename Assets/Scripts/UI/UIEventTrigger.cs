using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class UIEventTrigger : MonoBehaviour
{
    public GameObject btnStart;
    public UIPointerHandler pointerHandler;


    void Start()
    {
        if (pointerHandler != null)
        {
            pointerHandler.PointerEnter += OnPointerEnter;
            pointerHandler.PointerExit += OnPointerExit;
            pointerHandler.PointerDown += OnPointerDown;
        }
        btnStart.GetComponent<Image>().enabled = false;
    }

    void OnPointerEnter()
    {
        btnStart.GetComponent<Image>().enabled = true;
        Debug.Log("Pointer enter the UI element");
    }

    void OnPointerExit()
    {
        btnStart.GetComponent<Image>().enabled = false;
        Debug.Log("Pointer exit the UI element");
    }

    void OnPointerDown()
    {
        Debug.Log("Pointer down on the UI element.");
    }
}

    
    
    
    
    

    
    
    
    
    
    
    
    
    
