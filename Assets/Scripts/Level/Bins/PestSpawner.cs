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

    private ObjectPool<PestController> pestPool;

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

            var life = Mathf.Clamp(1.25f * rounds, 2, 10);
            var spawn_rate = Mathf.Clamp(.2f * rounds, 1, 3);
            yield return new WaitForSeconds(2);
            if (pestPool.CountActive >= maxConcurrentPest)
            {
                continue;
            }
            for (int i = 0; i < spawn_rate; i++)
            {
                // Spawn pest
                var pest = pestPool.Get();
                pest.life_s = life;
            }
            rounds++;
        }
    }

    public void StartPestSpawning()
    {
        StopPestSpawning();
        spawnPestsCoroutine = StartCoroutine(SpawnPests_Coroutine());
    }

    public void StopPestSpawning()
    {
        isActive = false;
        if (spawnPestsCoroutine != null)
        {
            StopCoroutine(spawnPestsCoroutine);
        }
    }
}