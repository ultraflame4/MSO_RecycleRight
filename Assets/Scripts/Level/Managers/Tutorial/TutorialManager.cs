using System.Collections;
using System.Collections.Generic;
using Level;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    TutorialTask[][] tasks;
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = LevelManager.Instance;
        tasks = new TutorialTask[levelManager.zones.Length][];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
