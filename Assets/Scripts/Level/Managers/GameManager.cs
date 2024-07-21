using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Entity.Data;

public class GameManager : MonoBehaviour
{
    [SerializeField] string prefabPath = "Prefabs/Player/Characters";
    [SerializeField] int partySize = 3;
    [SerializeField] float loadLevelDelay = 2.5f;

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
    [Tooltip("Whether this is the root game manager. if it is, it will not be destroyed on scene load.")]
    public bool is_root = false;

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

    // events
    public event Action StartedLevelLoad;

    // private fields
    Coroutine delayed_switch_scene_coroutine;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            if (is_root) DontDestroyOnLoad(gameObject);
        }

        if (_instance.is_root) { 
            Debug.LogWarning("Detected existing root game manager. This game manager will be deactived!");
            gameObject.SetActive(false);
            return;
        }
        Debug.LogWarning("Multiple game manager detected! This is not allowed!");

    }

    // Start is called before the first frame update
    void Start()
    {
        // reset coroutine to null
        delayed_switch_scene_coroutine = null;
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
    /// <summary>
    /// Load level scene based on the name of the scene
    /// </summary>
    /// <param name="name">Name of level scene to load</param>
    public void LoadLevel(string name)
    {
        if (delayed_switch_scene_coroutine != null || !LevelNames.Contains(name)) return;
        delayed_switch_scene_coroutine = StartCoroutine(DelayedSwitchScene(name, loadLevelDelay));
        StartedLevelLoad?.Invoke();
    }

    /// <summary>
    /// Load level scene based on index in Level Names array
    /// </summary>
    /// <param name="index">Index of level name in array</param>
    public void LoadLevel(int index)
    {
        if (delayed_switch_scene_coroutine != null || LevelNames == null || 
            LevelNames.Length <= 0 || index < 0 || index >= LevelNames.Length)
                return;
        delayed_switch_scene_coroutine = StartCoroutine(DelayedSwitchScene(LevelNames[index], loadLevelDelay));
        StartedLevelLoad?.Invoke();
    }

    /// <summary>
    /// Directly load the scene from the scene name
    /// </summary>
    /// <param name="name">Name of the scene</param>
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    IEnumerator DelayedSwitchScene(string scene_name, float delay)
    {
        yield return new WaitForSeconds(delay);
        delayed_switch_scene_coroutine = null;
        SceneManager.LoadScene(scene_name);
    }
    #endregion
}
