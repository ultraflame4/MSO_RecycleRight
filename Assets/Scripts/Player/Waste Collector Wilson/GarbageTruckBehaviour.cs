using UnityEngine;

public class GarbageTruckBehaviour : MoveTowards
{
    void OnTriggerEnter2D(Collider2D other) 
    {
        Destroy(other.gameObject);
    }
}
