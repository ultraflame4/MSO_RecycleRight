using UnityEngine;

public class LevelSelectHall : MonoBehaviour
{
    [SerializeField]
    private Animator trainDoorsAnim;
    [SerializeField]
    private Animator animator;

    public void Activate(LevelChoice choice)
    {
        gameObject.SetActive(true);

        trainDoorsAnim.SetTrigger("closeDoors");
        animator.SetBool("Active", true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        animator.SetBool("Active", false);
    }


}