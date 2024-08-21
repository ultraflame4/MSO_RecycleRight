using System;
using UnityEngine;

[Serializable]
public class LevelScoreGrade
{
    public string grade;

    [Tooltip("The color to display for this grade. Used for text.")]
    public Color gradeColor = Color.white;
    [Tooltip("The description to display for this grade.")]
    public string gradeDescription;
    [Range(0, 1)]
    public float percentage;
    [Tooltip("The sprite to display for this grade.")]
    public Sprite gradeSprite;
    
    [Tooltip("The sprites (animation if multiple) to display for this grade")]
    public Sprite[] gradeSpritesFrames;

    [Tooltip("The number of times to repeat the sprite (horizontal).")]
    public int spriteRepeat;

    
}

[CreateAssetMenu(fileName = "LevelGradingConfig", menuName = "ScriptableObjects/Game/LevelScoreGradingSO", order = 0)]
public class LevelScoreGradingSO : ScriptableObject
{
    public LevelScoreGrade failGrades;
    public LevelScoreGrade[] passGrades;

    [EasyButtons.Button]
    public void AutoSetPercentage()
    {
        float interval = 1f / (passGrades.Length-1);
        for (int i = 0; i < passGrades.Length; i++)
        {
            passGrades[i].percentage = interval * i;
        }
    }
}
