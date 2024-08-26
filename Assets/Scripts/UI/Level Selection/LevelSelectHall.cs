using System.Collections;
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
    [SerializeField]
    private AudioClip trainHallNoiseSFX;
    AudioSource sfxSource;

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

        SoundManager.Instance?.Play(trainHallNoiseSFX, out sfxSource, true);
    }


    public void Deactivate()
    {
        trainDoorsAnim.SetBool("zoom_out", false);
        // gameObject.SetActive(false);
        trainDoorsAnim.SetTrigger("openDoors");
        animator.SetBool("Active", false);
        if (sfxSource != null) SoundManager.Instance?.Stop(sfxSource);
    }


    public IEnumerator LaunchAnimation(){
        animator.SetTrigger("launchScene");
        if (sfxSource != null) SoundManager.Instance?.Stop(sfxSource);
        yield return new WaitForSeconds(0.25f);
        trainDoorsAnim.SetBool("zoom_out", false);
        // gameObject.SetActive(false);
        trainDoorsAnim.SetTrigger("openDoors");
        yield return new WaitForSeconds(0.75f);
    }
}