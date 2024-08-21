using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Bosses.Pilotras
{
    public class PilotrasFireController : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] int lanesToSpawn = 2;
        [SerializeField] float fireDuration = 7f;
        [SerializeField] float fireSpreadSpeed = .15f;
        [SerializeField] float minFireDistance = 2.5f;

        [Header("Tile Instantiation")]
        [SerializeField] Tilemap tilemap;
        [SerializeField] GameObject fireTilePrefab;

        [Header("Sound Effect")]
        [SerializeField] AudioClip fireSpawnSound;
        [SerializeField] AudioClip fireSound;

        [HideInInspector] public Vector2 minBounds, maxBounds;

        /// <summary>
        /// Spawns 2 vertical lanes of fire below self at random x-positions
        /// </summary>
        public void SpawnFire()
        {
            Vector3Int tilePos;

            for (int i = 0; i < lanesToSpawn; i++)
            {
                tilePos = GetAvailableLocation();
                StartCoroutine(SpreadFire(tilePos));
                tilePos.x++;
                StartCoroutine(SpreadFire(tilePos));
            }

            if (SoundManager.Instance == null) return;
            SoundManager.Instance.PlayOneShot(fireSpawnSound);
            StartCoroutine(PlayFireSound());
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

        IEnumerator PlayFireSound()
        {
            if (SoundManager.Instance.Play(fireSound, out AudioSource source, true))
            {
                yield return new WaitForSeconds(fireDuration + fireSpreadSpeed * (maxBounds.y - minBounds.y));
                SoundManager.Instance.Stop(source);
            }
        }

        Vector3Int GetAvailableLocation()
        {
            float xPos = Random.Range(minBounds.x, maxBounds.x);
            Vector3Int tmPos = tilemap.WorldToCell(new Vector3(xPos, maxBounds.y, transform.position.z));
            Vector3Int originalPos = tmPos;
            Vector3Int tmPos2 = tmPos;
            tmPos2.x++;

            // if randomly selected tiles have another tile, iterated till max and min bound to find an available tile
            while ((tilemap.HasTile(tmPos) || tilemap.HasTile(tmPos2)) && tmPos.x < maxBounds.x)
            {
                tmPos.x++;
                tmPos2.x++;
            }
            while ((tilemap.HasTile(tmPos) || tilemap.HasTile(tmPos2)) && tmPos.x > minBounds.x)
            {
                tmPos.x--;
                tmPos2.x--;
            }

            // if still cannot find tile, just override the original generated tile
            if (tilemap.HasTile(tmPos)) 
            {
                tmPos = originalPos;
                tmPos2 = originalPos;
                tmPos2.x++;
            }

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
    }
}
