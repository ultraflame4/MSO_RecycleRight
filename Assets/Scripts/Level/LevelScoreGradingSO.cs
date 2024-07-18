using System;
using UnityEngine;

[Serializable]
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
