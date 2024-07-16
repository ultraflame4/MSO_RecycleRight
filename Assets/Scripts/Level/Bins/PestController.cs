using NPC;
using UnityEngine;

public class PestController : MonoBehaviour
{
    [SerializeField]
    private Navigation navigation;
    [SerializeField]
    private Rigidbody2D rb;

    private const float current_direction_strength = .6f;
    public float rand_direction_strength = .4f;
    Vector3 current_direction;
    public event System.Action OnDeath;
    public float life_s = 1;

    private void OnEnable() {
        navigation.ClearDestination();
    }

    private void Update()
    {
        if (navigation.reachedDestination)
        {
            Vector3 randDir = new Vector3(Random.value - .5f, Random.value - .5f, 0).normalized;
            current_direction = (randDir * rand_direction_strength) + (current_direction * current_direction_strength);
            current_direction.Normalize();
            navigation.SetDestination(transform.position + current_direction * .5f);
        }
        transform.up = rb.velocity.normalized;
        life_s -= Time.deltaTime;
        if (life_s <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}