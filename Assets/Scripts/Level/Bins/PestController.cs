using System.Collections;
using Level.Bins;
using NPC;
using UnityEngine;

public class PestController : MonoBehaviour
{


    public event System.Action OnDeath;
    public float life_s = 1;
    public float sight = 8f;
    public bool is_dead = false;


    [SerializeField]
    private Navigation navigation;
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private SpriteRenderer spriteR;

    private const float current_direction_strength = .4f;
    private const float rand_direction_strength = .3f;
    private const float bin_direction_strength = .3f;
    private Vector3 current_direction;

    private Vector3 nearest_bin_dir = Vector3.zero;

    private void OnEnable()
    {
        // Because objects are recycled, we need to reset the navigation destination.
        navigation.ClearDestination();
        spriteR.color = Color.white;
        is_dead = false;
    }

    private void FixedUpdate()
    {
        var bins = Physics2D.OverlapCircleAll(transform.position, sight, LayerMask.GetMask("Bin"));
        if (bins.Length == 0) return;

        Vector3 nearest_bin = Vector3.zero;
        float nearest_bin_d = Mathf.Infinity;

        for (int i = 0; i < bins.Length; i++)
        {
            var bin = bins[i].GetComponent<RecyclingBin>();
            if (bin == null) continue;
            if (bin.binState == BinState.INFESTED || bin.binState == BinState.CLEANING) continue;

            var d = Vector3.Distance(bin.transform.position, transform.position);
            if (d < nearest_bin_d)
            {
                nearest_bin = bin.transform.position;
                nearest_bin_d = d;
            }
        }
        if (nearest_bin == Vector3.zero) return;
        nearest_bin_dir = (nearest_bin - transform.position).normalized;
    }

    private void Update()
    {
        if (is_dead) return;
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
        
        if (life_s <= 0)
        {
            KillSelf();
        }
    }

    public void OnEnteredBin()
    {
        KillSelf();
    }

    private void KillSelf()
    {
        // use is_dead to prevent multiple calls
        if (is_dead) return;
        is_dead = true;
        navigation.ClearDestination();
        rb.velocity = Vector2.zero;
        StartCoroutine(DeathEffect_Coroutine());
    }

    IEnumerator DeathEffect_Coroutine(){

        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(.01f);
            spriteR.color = Color.Lerp(Color.white, Color.clear, i / 50f);
        }
        OnDeath?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sight);
    }
}