using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected Vector2 moveDirection;

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Translate(moveDirection.normalized * movementSpeed * Time.deltaTime);
    }
}
