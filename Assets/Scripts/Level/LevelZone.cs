using UnityEngine;

public class LevelZone : MonoBehaviour
{
    [field: SerializeField]
    public Vector2 size { get; private set; } = new Vector2(20, 10);
    [field: SerializeField]
    public float buffer_zone_start { get; private set; } = 10f;


    [field: SerializeField]
    public float player_start_offset { get; private set; } = 1;
    public float player_startpos_x => transform.position.x - size.x / 2 + player_start_offset;

    public EnemyController[] enemies;

    private void Start()
    {
        enemies = GetComponentsInChildren<EnemyController>();
    }

    public void StartZone()
    {
        Debug.Log("Zone started");
        foreach (var enemy in enemies)
        {
            enemy.TriggerActive();
        }
    }
    public bool PositionWithinZone(Vector3 position)
    {
        bool x = position.x >= transform.position.x - size.x / 2 && position.x <= transform.position.x + size.x / 2;
        bool y = position.y >= transform.position.y - size.y / 2 && position.y <= transform.position.y + size.y / 2;
        return x && y;
    }

    public float DistanceFromEdgeX(Vector3 position)
    {
        float leftEdge = transform.position.x - size.x / 2;
        float rightEdge = transform.position.x + size.x / 2;
        return Mathf.Min(position.x - leftEdge, rightEdge - position.x);
    }
    public float DistanceFromEdgeY(Vector3 position)
    {
        float bottomEdge = transform.position.y - size.y / 2;
        float topEdge = transform.position.y + size.y / 2;
        return Mathf.Min(position.y - bottomEdge, topEdge - position.y);
    }

    public float DistanceFromEdge(Vector3 position)
    {
        return Mathf.Min(DistanceFromEdgeX(position), DistanceFromEdgeY(position));
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, size);
        Gizmos.color = Color.cyan * .1f;
        Gizmos.DrawCube(transform.position, size);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(player_startpos_x, transform.position.y - size.y / 2), new Vector2(player_startpos_x, transform.position.y + size.y / 2));
    }



}