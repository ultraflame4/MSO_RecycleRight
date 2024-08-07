using UnityEngine;

public class LevelSelectHall : MonoBehaviour
{
    [SerializeField]
    private Animator trainDoorsAnim;

    public void Activate(LevelChoice choice)
    {
        gameObject.SetActive(true);
        trainDoorsAnim.ResetTrigger("openDoors");
        trainDoorsAnim.ResetTrigger("closeDoors");
        trainDoorsAnim.SetTrigger("closeDoors");
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }


}