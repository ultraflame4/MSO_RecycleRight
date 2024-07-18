using UnityEngine;


public struct LevelScoreGrade
{
    public string grade;
    [Range(0, 1)]
    public float percentage;
    [Tooltip("The sprite to display for this grade.")]
    public Sprite gradeSprite;
    [Tooltip("The number of times to repeat the sprite (horizontal).")]
    public int spriteRepeat;

}

[CreateAssetMenu(fileName = "LevelScoreGradingSO", menuName = "ScriptableObjects/Level/LevelScoreGradingSO", order = 0)]
public class LevelScoreGradingSO : ScriptableObject {
    public LevelScoreGrade[] grades;
}
