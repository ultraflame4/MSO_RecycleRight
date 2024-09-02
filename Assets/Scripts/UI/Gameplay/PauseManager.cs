using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Level;

namespace UI
{
    public class PauseManager : MonoBehaviour
    {
        [Header("Pause Menu")]
        [SerializeField] KeyCode pauseKey = KeyCode.Escape;
        [SerializeField] AudioClip pauseSFX;

        [Header("Character Details")]
        [SerializeField] KeyCode detailsKey = KeyCode.C;
        [SerializeField] PauseDetailsMenu detailsMenu;

        private GameObject pauseMenu;

        public bool Paused { get; private set; } = false;
        private bool canPause = true;

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
            canPause = true;
        }

        // Update is called once per frame
        void Update()
        {
            CheckEnded();

            // detect showing details menu
            if (Input.GetKeyDown(detailsKey))
            {
                if (!Paused) TogglePause();
                detailsMenu?.Activate();
                return;
            }

            // detect pausing game
            if (!canPause || !Input.GetKeyDown(pauseKey)) return;
            TogglePause();
        }

        // menu button methods
        public void TogglePause()
        {
            Paused = !Paused;
            SoundManager.Instance?.PlayOneShot(pauseSFX);
            Time.timeScale = Paused ? 0f : 1f;
            pauseMenu?.SetActive(Paused);
            detailsMenu?.Deactivate();
        }

        public void RestartLevel()
        {
            if (Paused) TogglePause();
            SoundManager.Instance?.RestartBackgroundMusic();
            LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LeaveLevel(string scene_name)
        {
            if (Paused) TogglePause();
            LoadScene(scene_name);
        }

        void LoadScene(string scene_name)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("Game Manager instance could not be found, manually switching scenes instead. (PauseManager.cs)");
                SceneManager.LoadScene(scene_name);
                return;
            }

            GameManager.Instance.LoadScene(scene_name);
        }

        void CheckEnded()
        {
            if (LevelManager.Instance == null || !LevelManager.Instance.LevelEnded) return;
            canPause = false;
            if (!Paused) return;
            TogglePause();
        }
    }
}

