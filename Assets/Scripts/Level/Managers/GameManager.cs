using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int partySize = 3;
    [SerializeField] float loadLevelDelay = 2.5f;

    public GameConfigSO config;

    // array of characters that are currently in the party
    private PlayerCharacterSO[] selected_characters;
    public PlayerCharacterSO[] selectedCharacters
    {
        get
        {
            return selected_characters;
        }
        set
        {
            selected_characters = value;
            onSelectedCharactersChanged?.Invoke();
            if (selected_characters == null || selected_characters.Length <= partySize) return;
            PlayerCharacterSO[] tempArray = (PlayerCharacterSO[])selected_characters.Clone();
            selected_characters = new PlayerCharacterSO[partySize];
            for (int i = 0; i < selected_characters.Length; i++)
            {
                selected_characters[i] = tempArray[i];
            }
        }
    }

    public event Action onSelectedCharactersChanged;
    [HideInInspector] public bool CanSetTeam = true;

    public int PartySize => partySize;

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance != this)
            {
                Debug.LogWarning("Detected existing game manager. This game manager will be deactived!");
                gameObject.SetActive(false);
                return;
            }
            Debug.LogWarning("Multiple game manager detected! This is not allowed!");
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        // reset coroutine to null
        delayed_switch_scene_coroutine = null;

    }

    #region Scene Management
    /// <summary>
    /// Load level scene based on the name of the scene
    /// </summary>
    /// <param name="name">Name of level scene to load</param>
    public void LoadLevel(string name)
    {
        if (delayed_switch_scene_coroutine != null || config.levels.Any(x => x.levelInfo.name == name)) return;
        delayed_switch_scene_coroutine = StartCoroutine(DelayedSwitchScene(name, loadLevelDelay));
        StartedLevelLoad?.Invoke();
    }

    /// <summary>
    /// Load level scene based on index in Level Names array
    /// </summary>
    /// <param name="index">Index of level name in array</param>
    public void LoadLevel(int index)
    {
        if (delayed_switch_scene_coroutine != null || config.levels.Length == 0 ||
            index < 0 || index >= config.levels.Length)
            return;
        delayed_switch_scene_coroutine = StartCoroutine(DelayedSwitchScene(config.levels[index].scene.Name, loadLevelDelay));
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

    /// <summary>
    /// Opens the Level Selection Scene.
    /// </summary>
    public void OpenScene_LevelSelection()
    {
        LoadScene("Level-Selection");
    }

    /// <summary>
    /// Opens the main menu scene
    /// </summary>
    public void OpenScene_MainMenu()
    {
        LoadScene("MainMenu");
    }

    /// <summary>
    /// Restarts the current scene
    /// </summary>
    /// <returns></returns>
    public void RestartScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator DelayedSwitchScene(string scene_name, float delay)
    {
        yield return new WaitForSeconds(delay);
        delayed_switch_scene_coroutine = null;
        SceneManager.LoadScene(scene_name);
    }
    #endregion
}
