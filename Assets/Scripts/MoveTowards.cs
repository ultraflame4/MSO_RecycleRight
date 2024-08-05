using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] Vector2 moveDirection;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection.normalized * movementSpeed * Time.deltaTime);
    }
}
