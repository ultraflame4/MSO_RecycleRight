using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [SerializeField] int clipIndex = 0;
    SoundManager soundManager => SoundManager.Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (soundManager == null)
        {
            Debug.LogWarning("Sound Manager instance could not be found, background music could not be played! (BackgroundMusicManager.cs)");
            return;
        }

        soundManager.PlayBackgroundMusic(clipIndex);
    }
}
