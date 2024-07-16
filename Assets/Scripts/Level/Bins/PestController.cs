using NPC;
using UnityEngine;

public class PestController : MonoBehaviour
{

    
    public event System.Action OnDeath;
    public float life_s = 1;
    public float sight=8f;
    public bool is_dead=false;


    [SerializeField]
    private Navigation navigation;
    [SerializeField]
    private Rigidbody2D rb;

    private const float current_direction_strength = .4f;
    private const float rand_direction_strength = .3f;
    private const float bin_direction_strength = .3f;
    private Vector3 current_direction;

    private Vector3 nearest_bin_dir = Vector3.zero;

    private void OnEnable() {
        // Because objects are recycled, we need to reset the navigation destination.
        navigation.ClearDestination();
    }

    private void FixedUpdate() {
        var bins = Physics2D.OverlapCircleAll(transform.position, sight, LayerMask.GetMask("Bin"));
        if (bins.Length == 0) return;
        Vector3 nearest_bin = bins[0].transform.position;
        float nearest_bin_d = Vector3.Distance(nearest_bin, transform.position);
        for (int i = 0; i < bins.Length; i++)
        {
            if (Vector3.Distance(bins[i].transform.position, transform.position) < nearest_bin_d)
            {
                nearest_bin = bins[i].transform.position;
            }
        }
        nearest_bin_dir = (nearest_bin - transform.position).normalized;
    }

    private void Update()
    {

        // If the pest has reached its destination, set a new random direction.
        if (navigation.reachedDestination)
        {
            Vector3 randDir = new Vector3(Random.value - .5f, Random.value - .5f, 0).normalized;
            // Apply weights to the new direction and the current direction. so that the resulting vector is more similar to the current direction
            current_direction = (randDir * rand_direction_strength) + (current_direction * current_direction_strength) + (nearest_bin_dir * bin_direction_strength);
            current_direction.Normalize();
            // Set the new destination to the new position
            navigation.SetDestination(transform.position + current_direction * .5f);
        }
        // Make the pest face the direction it is moving
        transform.up = rb.velocity.normalized;
        // Decrease the life of the pest
        life_s -= Time.deltaTime;
        // If the pest has run out of life, call the OnDeath event
        // use is_dead to prevent multiple calls to OnDeath
        if (life_s <= 0 && !is_dead)
        {
            is_dead = true;
            OnDeath?.Invoke();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sight);
    }
}