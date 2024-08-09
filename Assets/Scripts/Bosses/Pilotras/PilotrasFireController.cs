using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Level;

namespace Bosses.Pilotras
{
    public class PilotrasFireController : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] int lanesToSpawn = 2;
        [SerializeField] float yOffset = 5f;
        [SerializeField] float fireDuration = 7f;
        [SerializeField] float fireSpreadSpeed = .15f;
        [SerializeField] float minFireDistance = 2.5f;

        [Header("Tile Instantiation")]
        [SerializeField] Tilemap tilemap;
        [SerializeField] GameObject fireTilePrefab;
        [SerializeField] LevelZone debug_zone;

        [HideInInspector] public bool showGizmos = true;

        LevelManager levelManager => LevelManager.Instance;
        LevelZone zone => levelManager == null ? null : levelManager.current_zone;
        Vector2 minBounds, maxBounds;

        void Start()
        {
            CalculateBounds(zone);
        }

        /// <summary>
        /// Spawns vertical lanes of fire below self at random x-positions
        /// </summary>
        public void SpawnFire()
        {
            Vector3Int tilePos;

            for (int i = 0; i < lanesToSpawn; i++)
            {
                tilePos = GetAvailableLocation();
                StartCoroutine(SpreadFire(tilePos));
            }
        }

        IEnumerator SpreadFire(Vector3Int tilePos)
        {
            int length = Mathf.RoundToInt(tilePos.y - minBounds.y);

            for (int i = 0; i <= length; i++)
            {
                LoadTile(tilePos, fireDuration + (fireSpreadSpeed * (length - i)));
                tilePos.y--;
                yield return new WaitForSeconds(fireSpreadSpeed);
            }
        }

        Vector3Int GetAvailableLocation()
        {
            float xPos = Random.Range(minBounds.x, maxBounds.x);
            Vector3Int tmPos = tilemap.WorldToCell(new Vector3(xPos, maxBounds.y, transform.position.z));
            Vector3Int originalPos = tmPos;

            // if randomly selected tiles have another tile, iterated till max and min bound to find an available tile
            while (tilemap.HasTile(tmPos) && tmPos.x < maxBounds.x)
            {
                tmPos.x++;
            }
            while (tilemap.HasTile(tmPos) && tmPos.x > minBounds.x)
            {
                tmPos.x--;
            }

            // if still cannot find tile, just override the original generated tile
            if (tilemap.HasTile(tmPos)) tmPos = originalPos;
            return tmPos;
        }

        void LoadTile(Vector3Int pos, float duration)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.gameObject = fireTilePrefab;
            var fireTile = tile.gameObject.GetComponent<FireTile>();
            fireTile.lifetime = duration;
            fireTile.associatedTilemap = tilemap;
            tilemap.SetTile(pos, tile);
        }

        void CalculateBounds(LevelZone zone)
        {
            minBounds.x = zone.center.x - (zone.size.x / 2f);
            minBounds.y = zone.center.y - (zone.size.y / 2f);
            maxBounds.x = zone.center.x + (zone.size.x / 2f);
            maxBounds.y = zone.center.y + (zone.size.y / 2f);
            maxBounds.y = maxBounds.y - (maxBounds.y - transform.position.y) - yOffset;
        }

        void OnDrawGizmosSelected() 
        {
            if (!showGizmos || debug_zone == null) return;
            CalculateBounds(debug_zone);

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(minBounds, 0.5f);
            Gizmos.DrawSphere(maxBounds, 0.5f);
        }
    }
}
