using System.Collections;
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
    public LevelZone current_zone => zones[current_zone_index];
    public void Start()
    {
        zones = transform.GetComponentsInChildren<LevelZone>();
        MoveToZone(0);
    }

    public void MoveToZone(int index)
    {
        current_zone_index = index;
        camera.target_position = current_zone.transform.position;
        player.ForceMoveTo(new Vector3(current_zone.player_startpos_x, player.transform.position.y, player.transform.position.z));
        StartCoroutine(ZoneStart_corountine());
    }

    IEnumerator ZoneStart_corountine()
    {
        // Wait until player is within zone
        while (!current_zone.PositionWithinZone(player.transform.position))
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        current_zone.StardtZone();
    }

    private void OnValidate()
    {
        if (debug_move_to_current_zone)
        {
            debug_move_to_current_zone = false;
            MoveToZone(current_zone_index);
        }
    }
}