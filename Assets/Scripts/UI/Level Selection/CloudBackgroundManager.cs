using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.LevelSelection
{
    public class CloudBackgroundManager : MonoBehaviour
    {
        [SerializeField] RectTransform canvas;
        [SerializeField] Cloud[] clouds;

        [Serializable]
        public struct Cloud
        {
            public Vector2 ySpawnRange, spawnCooldownRange;
            public RectTransform prefab;
        }

        List<RectTransform> cloudPrefabs;
        List<GameObject>[] cloudObjectPool;

        float xBoundary;

        // Start is called before the first frame update
        void Start()
        {
            xBoundary = canvas.sizeDelta.x / 2f;
            cloudPrefabs = new List<RectTransform>();
            cloudObjectPool = new List<GameObject>[clouds.Length];
            StartClouds();
        }

        // Update is called once per frame
        void Update()
        {
            if (cloudPrefabs.Count <= 0) StartClouds();

            for (int i = 0; i < cloudPrefabs.Count; i++)
            {
                RectTransform cloud = cloudPrefabs[i];
                if (cloud.localPosition.x >= -xBoundary - (cloud.sizeDelta.x / 2f)) continue;
                cloudPrefabs.Remove(cloud);
                cloud.gameObject.SetActive(false);
                cloudObjectPool[cloud.GetComponent<CloudMovement>().index].Add(cloud.gameObject);
            }
        }

        void StartClouds()
        {
            StopAllCoroutines();

            foreach (Cloud cloud in clouds)
            {
                StartCoroutine(SpawnClouds(cloud));
            }
        }

        IEnumerator SpawnClouds(Cloud cloud)
        {
            // create cloud prefab
            // get index of cloud
            int index = Array.IndexOf(clouds, cloud);
            // get cloud prefab, check if there are any objects pooled
            if (cloudObjectPool[index] == null) cloudObjectPool[index] = new List<GameObject>();
            GameObject gameObject = cloudObjectPool[index].Count <= 0 ? 
                CreateObject(cloud.prefab.gameObject, index) : GetFromPool(cloud.prefab.gameObject, index);
            gameObject.transform.localPosition = new Vector2(xBoundary + (cloud.prefab.sizeDelta.x / 2f), 
                Random.Range(cloud.ySpawnRange.x, cloud.ySpawnRange.y));
            cloudPrefabs.Add(gameObject.GetComponent<RectTransform>());
            // set sorting index
            cloudPrefabs[^1].transform.SetSiblingIndex(index);
            // wait for cooldown before spawning another cloud
            yield return new WaitForSeconds(Random.Range(cloud.spawnCooldownRange.x, cloud.spawnCooldownRange.y));
            // start a new coroutine to repeat cloud spawning
            StartCoroutine(SpawnClouds(cloud));
        }
        
        #region Object Pooling
        public class PooledItem : MonoBehaviour
        {
            public int index;
        }

        GameObject CreateObject(GameObject prefab, int index)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.GetComponent<CloudMovement>().index = index;
            cloudObjectPool[index].Add(obj);
            return obj;
        }

        GameObject GetFromPool(GameObject prefab, int index)
        {
            GameObject obj;
            // search for available object in pool
            for (int i = 0; i < cloudObjectPool[index].Count; i++)
            {
                obj = cloudObjectPool[index][i];
                if (obj.activeSelf) continue;
                cloudObjectPool[index].RemoveAt(i);
                obj.SetActive(true);
                return obj;
            }
            // if there are no objects left, create new object
            return CreateObject(prefab, index);
        }
        #endregion
    }
}
