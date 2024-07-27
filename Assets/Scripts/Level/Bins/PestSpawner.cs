using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PestSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject pestPrefab;
    [Tooltip("Maximum number of pests allowed to be alive at any time.")]
    public int maxConcurrentPest = 10;


    private ObjectPool<PestController> pestPool; // Using ObjectPool to recycle game objects.

    /// <summary>
    /// Coroutine that spawns pests.
    /// </summary>
    private Coroutine spawnPestsCoroutine;
    [Header("Status"), SerializeField]
    private bool isActive = false;


    private void Awake()
    {
        pestPool = new ObjectPool<PestController>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, maxConcurrentPest, maxConcurrentPest);
    }

    private void OnDestroyPoolObject(PestController pest)
    {
        Destroy(pest.gameObject);
    }

    private void OnReturnedToPool(PestController pest)
    {
        pest.gameObject.SetActive(false);
    }

    private void OnTakeFromPool(PestController pest)
    {
        pest.gameObject.SetActive(true);
        // Reset position and rotation
        pest.gameObject.transform.position = transform.position;
        pest.gameObject.transform.rotation = Quaternion.identity;
        pest.gameObject.transform.parent = transform;
    }

    private PestController CreatePooledItem()
    {
        var obj = Instantiate(pestPrefab, transform.position, Quaternion.identity, transform);
        var pest = obj.GetComponent<PestController>();
        pest.OnDeath += () => pestPool.Release(pest);
        return pest;
    }


    private IEnumerator SpawnPests_Coroutine()
    {
        isActive = true;
        int rounds = 0;
        while (true)
        {
            var now = Utils.GetCurrentTime();

            // Calculate life and spawn rate based on number of rounds. These values were tested on desmos.
            var life = Mathf.Clamp(1.25f * rounds, 2, 10);
            var spawn_count = Mathf.Clamp(.2f * rounds, 1, 3);
            // Wait for 2 seconds between each round.
            yield return new WaitForSeconds(2);

            // If there are already maxConcurrentPest pests alive, skip this round.
            if (pestPool.CountActive >= maxConcurrentPest) continue;

            // Spawn pests
            for (int i = 0; i < spawn_count; i++)
            {
                // Spawn pest
                var pest = pestPool.Get(); // Get a pest from the pool and assign the new life value.
                pest.life_s = life;
            }
            // Increase round count
            rounds++;
        }
    }

    /// <summary>
    /// Start spawning pests.
    /// </summary>
    [EasyButtons.Button]
    public void StartPestSpawning()
    {
        StopPestSpawning();
        spawnPestsCoroutine = StartCoroutine(SpawnPests_Coroutine());
    }

    /// <summary>
    /// Stop spawning pests.
    /// </summary>
    [EasyButtons.Button]
    public void StopPestSpawning()
    {
        isActive = false;
        if (spawnPestsCoroutine != null)
        {
            StopCoroutine(spawnPestsCoroutine);
        }
    }

    /// <summary>
    /// Destroys all pests from this bin
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    [EasyButtons.Button]
    public void ClearPests()
    {
        foreach (var pest in GetComponentsInChildren<PestController>())
        {
            pest.KillSelf();
        }
        
    }
}