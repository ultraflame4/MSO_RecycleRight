using UnityEngine;
using TMPro;
using Level;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    LevelManager levelManager => LevelManager.Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (scoreText != null) return;
        scoreText = GetComponent<TextMeshProUGUI>();
        if (scoreText != null) return;
        Debug.LogWarning("Score UI Text could not be found. (ScoreUI.cs)");
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreText == null || levelManager == null) return;
        scoreText.text = $"Score: {levelManager.GetCurrentScore()}";
    }
}
