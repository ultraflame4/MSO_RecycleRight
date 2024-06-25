using UnityEngine;

public class DirectionPointer : MonoBehaviour {
    
    [field: SerializeField, Tooltip("How far away the pointer should be away from the player.")]
    public float distance { get; private set; }
    [field: SerializeField, Tooltip("The player's transform.")]
    public Transform player { get; private set; }


    private void Update() {
        Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 player_pos = player.position;
        // Ensure that comparison is done in 2D space
        player_pos.z = 0; 
        mouse_position.z = 0; 

        Vector3 direction = (mouse_position - player_pos).normalized;

        transform.position = player.position + direction * distance;
        // Rotate the pointer to face the mouse
        transform.up = direction;
    }
}