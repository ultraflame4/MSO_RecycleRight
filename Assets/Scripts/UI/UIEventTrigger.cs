using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class UIEventTrigger : MonoBehaviour
{
    //Create reference variable
    public GameObject btnStart;
    public UIPointerHandler pointerHandler;


    void Start()
    {
        //Calls the event from UIPointerHandler script
        if (pointerHandler != null)
        {
            pointerHandler.PointerEnter += OnPointerEnter;
            pointerHandler.PointerExit += OnPointerExit;
            pointerHandler.PointerDown += OnPointerDown;
        }
        //UI is hidden from start
        btnStart.GetComponent<Image>().enabled = false;
    }

    //When pointer enter the UI element the UI shows up
    void OnPointerEnter()
    {
        btnStart.GetComponent<Image>().enabled = true;
        Debug.Log("Pointer enter the UI element");
    }
    // When pointer exit the UI element it hides the UI
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

    
    
    
    
    

    
    
    
    
    
    
    
    
    
