using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;
    private GameObject pauseMenu;

    public bool Paused { get; private set; } = false;

    private static PauseManager _instance;
    public static PauseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new NullReferenceException("There is no pause manager instance in the scene!");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Debug.LogWarning("There are multiple PauseManager(s) in the scene! This is not allowed!");
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = transform.GetChild(0).gameObject;
        pauseMenu?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(pauseKey)) return;
        TogglePause();
    }

    // menu button methods
    public void TogglePause()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 0f : 1f;
        pauseMenu?.SetActive(Paused);
    }

    public void RestartLevel()
    {
        if (Paused) TogglePause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LeaveLevel(string scene_name)
    {
        if (Paused) TogglePause();
        SceneManager.LoadScene(scene_name);
    }
}
