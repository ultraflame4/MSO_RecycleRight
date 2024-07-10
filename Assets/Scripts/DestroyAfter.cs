using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] float duration;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterLifetime());
    }

    IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
