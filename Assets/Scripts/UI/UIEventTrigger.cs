using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventTrigger : MonoBehaviour
{
    public GameObject btnStart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Enter()
    {
        btnStart.SetActive(true);
    }
}
