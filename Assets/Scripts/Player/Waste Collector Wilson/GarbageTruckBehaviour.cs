using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageTruckBehaviour : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] Vector2 moveDirection;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection.normalized * movementSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        Destroy(other.gameObject);
    }
}
