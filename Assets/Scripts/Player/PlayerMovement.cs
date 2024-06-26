using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField, Tooltip("The Rigidbody2D component of the player.")]
    public Rigidbody2D rb { get; private set; }
    
    [field: SerializeField, Tooltip("The speed at which the player moves.")]
    public float movement_speed { get; private set; }

    private Vector2 move_input = Vector2.zero;
    private Vector3? force_move_to = null;


    private void Update()
    {
        move_input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        if (force_move_to != null){
            var dir = force_move_to.Value - transform.position;
            var vel = dir.normalized * movement_speed * 4 * Time.deltaTime;
            rb.velocity = vel * Mathf.Clamp01( dir.sqrMagnitude);
            if (dir.magnitude < .1f)
            {
                force_move_to = null;
                rb.velocity = Vector2.zero;
            }
            return;
        }
        rb.velocity = move_input * movement_speed * Time.deltaTime;
    }

    public void ForceMoveTo(Vector3 position)
    {
        force_move_to = position;
    }
}