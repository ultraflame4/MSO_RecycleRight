using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UITransition : MonoBehaviour
{
    public SpriteRenderer sr;
    public float transitionSpeed = 1f;
    
    private float transitionProgress = 0f;
    private bool transitionStarted = false;

    //Onlick function for the button to start transition
    public void TransitionStart()
    {
         transitionStarted = true ;
         Debug.Log("Transition Started");
    }
    //Start is called before the first frame update
    void Start()
    {
        TransitionStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (transitionStarted)
        {
            transitionProgress += Time.deltaTime * transitionSpeed;
            sr.material.SetFloat ("_CutOff", transitionProgress);
            if (transitionProgress >= 1f)
            {
                Debug.Log("Transition Done");
                transitionStarted = false;
            }
        }
        
    }
}
