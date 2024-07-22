using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndMenu : MonoBehaviour {
    
    [SerializeField]    
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI gradeTitle;
    [SerializeField]
    private TextMeshProUGUI gradeText;
    [SerializeField]
    private GameObject badgePrefab;
    [SerializeField]
    private GameObject badgeContainer;


    public static LevelScoreGrade GradeFromScore(float score, float maxScore){
        float current_p = score / maxScore;
        foreach (var grade in GameManager.Instance.config.grades.passGrades.OrderByDescending(x => x.percentage))
        {

            if (current_p >= grade.percentage)
            {
                return grade;
            }
        }
        return GameManager.Instance.config.grades.failGrades;
    }

    public void ShowGrade(LevelScoreGrade grade){
        gradeTitle.color = grade.gradeColor;
        gradeText.text = grade.grade;
        gradeText.color = grade.gradeColor;
        badgeContainer.transform.DisableAllChildren();
        for (int i = 0; i < grade.spriteRepeat; i++)
        {
            GameObject badgeChild;
            if (badgeContainer.transform.childCount > i)
            {
                badgeChild = badgeContainer.transform.GetChild(i).gameObject;
            }
            else
            {
                badgeChild = Instantiate(badgePrefab, badgeContainer.transform);
            }
            badgeChild.SetActive(true);
            badgeChild.GetComponent<Image>().sprite = grade.gradeSprite;
        }
    }

    public void ShowScore(float score){
        scoreText.text = $"Score: {score}";
    }

    public void ShowEndScreen(float score, float maxScore){
        gameObject.SetActive(true);
        var grade = GradeFromScore(score, maxScore);
        ShowGrade(grade);
        ShowScore(score);
    }
    

}