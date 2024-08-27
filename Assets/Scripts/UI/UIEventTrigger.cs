using UnityEngine;
using UnityEngine.UI;
using UI;

public class UIEventTrigger : MonoBehaviour
{
    //Create reference variable
    public GameObject btnStart;
    public UIPointerHandler pointerHandler;
    Image btnStartImg;

    void Start()
    {
        //Calls the event from UIPointerHandler script
        if (pointerHandler != null)
        {
            pointerHandler.PointerEnter += OnPointerEnter;
            pointerHandler.PointerExit += OnPointerExit;
        }
        // get start button image
        btnStartImg = btnStart.GetComponent<Image>();
        //UI is hidden from start
        btnStartImg.enabled = false;
    }

    //When pointer enter the UI element the UI shows up
    void OnPointerEnter()
    {
        btnStartImg.enabled = true;
    }
    // When pointer exit the UI element it hides the UI
    void OnPointerExit()
    {
        btnStartImg.enabled = false;
    }
}

    
    
    
    
    

    
    
    
    
    
    
    
    
    
