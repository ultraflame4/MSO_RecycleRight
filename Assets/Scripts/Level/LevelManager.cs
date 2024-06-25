using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [field: SerializeField, Tooltip("The zones that the player can switch between. This is automatically retrieved at runtime.")]
    public LevelZone[] zones { get; private set; }

    [field: SerializeField, Tooltip("The index of the current zone.")]
    public int current_zone_index { get; private set; } = 0;

    [field: SerializeField, Tooltip("The player's movement script."), Header("References")]
    public PlayerMovement player { get; private set; }
    [field: SerializeField, Tooltip("The level camera")]
    public LevelCamera camera { get; private set; }

    public bool debug_move_to_current_zone = false;
    public void Start()
    {
        zones = transform.GetComponentsInChildren<LevelZone>();
        MoveToZone(0);
    }

    public void MoveToZone(int index)
    {
        current_zone_index = index;
        camera.target_position = zones[index].transform.position;
        player.ForceMoveTo(new Vector3(zones[index].player_startpos_x, player.transform.position.y, player.transform.position.z));
    }

    private void OnValidate() {
        if (debug_move_to_current_zone)
        {
            debug_move_to_current_zone = false;
            MoveToZone(current_zone_index);
        }
    }
}