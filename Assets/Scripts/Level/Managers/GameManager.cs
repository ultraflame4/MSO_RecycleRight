using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Entity.Data;

public class GameManager : MonoBehaviour
{
    [SerializeField] string prefabPath = "Prefabs/Player/Characters";
    [SerializeField] int partySize = 3;

    [field: SerializeField] 
    public string[] LevelNames { get; private set; }

    // characters
    public Character[] Characters { get; private set; }
    public struct Character
    {
        public GameObject prefab;
        public PlayerCharacter data;
        public bool selected;

        public Character(GameObject prefab, PlayerCharacter data)
        {
            this.prefab = prefab;
            this.data = data;
            selected = false;
        }
    }

    public LevelScoreGradingSO levelScoreGradingSO;

    // singleton instance
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
        // get reference to player character prefabs
        GameObject[] prefabs = Resources.LoadAll<GameObject>(prefabPath);
        Characters = new Character[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            Characters[i] = new Character(prefabs[i], prefabs[i].GetComponent<PlayerCharacter>());
            Characters[i].data.SetData();
        }
    }

    #region Level Selection
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
    #endregion
}
