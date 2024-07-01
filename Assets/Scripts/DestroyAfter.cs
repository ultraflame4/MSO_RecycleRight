using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] float duration;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestryAfterLifetime());
    }

    IEnumerator DestryAfterLifetime()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
