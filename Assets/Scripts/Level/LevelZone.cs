using UnityEngine;

public class LevelZone : MonoBehaviour {
    [field: SerializeField]
    public Vector2 size { get; private set; } = new Vector2(20,10);
    [field: SerializeField]
    public float player_start_offset { get; private set; } = 1;
    public float player_startpos_x => transform.position.x - size.x / 2 + player_start_offset;

    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, size);
        Gizmos.color = Color.cyan * .1f;
        Gizmos.DrawCube(transform.position, size);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(player_startpos_x, transform.position.y - size.y / 2), new Vector2(player_startpos_x, transform.position.y + size.y / 2));
    }
}