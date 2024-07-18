using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Entity.Data;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] 
    public string[] LevelNames { get; private set; }
    public GameObject[] Characters { get; private set; }
    public PlayerCharacter[] CharacterData { get; private set; }

    private static GameManager _instance;
    public static GameManager Instance 
    { 
        get 
        { 
            if (_instance == null)
                throw new NullReferenceException("There is no Game Manager in the scene!");
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("There are multiple GameManagers in the scene! This is not allowed!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel(string name)
    {
        if (!LevelNames.Contains(name)) return;
        SceneManager.LoadScene(name);
    }

    public void LoadLevel(int index)
    {
        if (LevelNames == null || LevelNames.Length <= 0 || index < 0 || index >= LevelNames.Length)
            return;
        SceneManager.LoadScene(LevelNames[index]);
    }
}
