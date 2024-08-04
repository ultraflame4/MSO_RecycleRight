using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagerment;

public class TransitionController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        public SpriteRenderer sr;
        public float transitionSpeed = 1f;

        private float transitionSpeed = 0f;
        private bool transitionStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transitionStarted)
        {
            transitionProgress += Time.deltaTime + tansitionSpeed;
            sr.material.SetFloat("_CutOff", transitionProgress);
            if (transitionProgress >= 1f)
            {
                transitionStarted = false;
                SceneManager.LoadScene("Level-Selection");
            }
        }
    }
    void transitionStart()
    {
        transitionStarted = true;
    }
}
