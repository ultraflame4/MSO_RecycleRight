using Unity.VisualScripting;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField, Tooltip("The Rigidbody2D component of the player.")]
    public Rigidbody2D rb { get; private set; }
    
    [field: SerializeField, Tooltip("The speed at which the player moves.")]
    public float movement_speed { get; private set; }

    private Vector2 move_input = Vector2.zero;


    private void Update()
    {
        move_input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        rb.velocity = move_input * movement_speed * Time.deltaTime;
    }
}