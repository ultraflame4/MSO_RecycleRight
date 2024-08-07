using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectHall : MonoBehaviour
{

    [SerializeField]
    private Animator trainDoorsAnim;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Image levelImage;
    [SerializeField]
    private TextMeshProUGUI levelTitle;
    [SerializeField]
    private TextMeshProUGUI levelDesc;

    void Start()
    {
        // Deactivate();
    }

    public void Activate(LevelInfoSO levelInfo)
    {
        gameObject.SetActive(true);

        trainDoorsAnim.SetBool("zoom_out", true);
        trainDoorsAnim.ResetTrigger("openDoors");
        trainDoorsAnim.SetTrigger("closeDoors");
        animator.SetBool("Active", true);

        if (levelInfo.data.levelImage != null)
        {
            levelImage.sprite = levelInfo.data.levelImage;
        }
        levelTitle.text = levelInfo.data.levelName;
        levelDesc.text = levelInfo.data.levelDescription;

    }

    public void Deactivate()
    {

        trainDoorsAnim.SetBool("zoom_out", false);
        Debug.Log("Deactivating level hall");
        // gameObject.SetActive(false);
        trainDoorsAnim.SetTrigger("openDoors");
        animator.SetBool("Active", false);
    }

    public void TriggerLaunchAnimations()
    {

        trainDoorsAnim.SetBool("zoom_out", false);
        Debug.Log("Deactivating level hall");
        // gameObject.SetActive(false);
        trainDoorsAnim.SetTrigger("openDoors");
        animator.SetTrigger("launchScene");
    }
}