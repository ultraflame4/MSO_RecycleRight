using System.Collections.Generic;
using UnityEngine;

namespace Bosses
{
    public class IndicatorManager : MonoBehaviour
    {
        [SerializeField] GameObject[] indicators;
        [SerializeField] Transform indicatorParent;
        List<GameObject>[] indicatorPool;

        void Awake()
        {
            // load indicator pool
            indicatorPool = new List<GameObject>[indicators.Length];
            for (int i = 0; i < indicatorPool.Length; i++)
            {
                indicatorPool[i] = new List<GameObject>();
            }
        }

        /// <summary>
        /// Returns an indicator game object using object pooling
        /// </summary>
        /// <param name="index">The index of the indicator</param>
        /// <param name="position">Position to instantiate object</param>
        /// <returns>Game object of instantiated indicator</returns>
        public GameObject Instantiate(int index, Vector3 position)
        {
            if (indicatorPool == null || index < 0 || index >= indicatorPool.Length || indicatorPool[index] == null)
            {
                Debug.LogWarning($"Index ({index}) is out of range ({indicatorPool.Length})! (IndicatorManager.cs)");
                return null;
            }

            for (int i = 0; i < indicatorPool[index].Count; i++)
            {
                if (indicatorPool[index][i].activeSelf) continue;
                indicatorPool[index][i].transform.position = position;
                indicatorPool[index][i].transform.rotation = Quaternion.identity;
                indicatorPool[index][i].transform.parent = indicatorParent;
                indicatorPool[index][i].SetActive(true);
                return indicatorPool[index][i];
            }

            indicatorPool[index].Add(Instantiate(indicators[index], position, Quaternion.identity, indicatorParent));
            return indicatorPool[index][^1];
        }
    }
}
