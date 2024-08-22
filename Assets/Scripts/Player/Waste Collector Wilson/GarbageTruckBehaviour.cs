using UnityEngine;
using Level;

public class GarbageTruckBehaviour : MoveTowards
{
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] DestroyAfter destroyManager;
    [SerializeField, Range(-2f, 2f)] float zoneSpace = 0.9f;
    LevelManager levelManager => LevelManager.Instance;
    LevelZone zone => levelManager.current_zone;

    void Start()
    {
        if (levelManager == null || zone == null) return;
        // scale box collider size depending on height of zone
        Vector2 size = boxCollider.size;
        size.y = zone.size.y * 1.5f;
        boxCollider.size = size;
        // scale truck positions depending on size of zone
        ManageTruckPositions();
        // remove destroy after script if there is a level manager, manually handle destroying self
        destroyManager.enabled = false;
    }

    protected override void Update()
    {
        base.Update();

        // ensure level manager and zone is not null, if so, enable destroy manager to manage destroying self
        if (levelManager == null || zone == null)
        {
            destroyManager.enabled = true;
            return;
        }
        else
        {
            destroyManager.enabled = false;
        }

        // check if left zone
        if (transform.position.x + boxCollider.offset.x - (boxCollider.size.x * 0.5f) <= 
            zone.center.x + (zone.size.x * 0.5f))
                return;
        Destroy(gameObject);
    }

    // manage destroying NPCs through collisions
    void OnTriggerEnter2D(Collider2D other) 
    {
        Destroy(other.gameObject);
    }

    void ManageTruckPositions()
    {
        Transform[] trucks = transform.GetComponentsInChildren<Transform>();
        float spaceBetweenTruck = (zone.size.y * zoneSpace) / (trucks.Length - 1f);

        for (int i = 0; i < trucks.Length; i++)
        {
            Transform truck = trucks[i];
            if (truck == transform) continue;
            float yPos = zone.center.y + (zone.size.y * zoneSpace * 0.5f);
            yPos -= i * spaceBetweenTruck;
            truck.position = new Vector2(truck.position.x, yPos);
        }
    }
}
