using System.Collections;
using Level.Bins;
using NPC;
using UnityEngine;

public class PestController : MonoBehaviour, IBinTrashItem
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
        // Reset the sprite color
        spriteR.color = Color.white;
        // Reset flags
        is_dead = false;
    }

    private void FixedUpdate()
    {
        // Check for nearby bins to infest.
        // Use LayerMask.GetMask("Bin") to only get bins
        var bins = Physics2D.OverlapCircleAll(transform.position, sight, LayerMask.GetMask("Bin"));
        // Skip if there are no bins
        if (bins.Length == 0) return;


        Vector3 nearest_bin = Vector3.zero;
        float nearest_bin_d = Mathf.Infinity;

        // Find the nearest bin that is not already infested or is being cleaned by the player
        for (int i = 0; i < bins.Length; i++)
        {
            var bin = bins[i].GetComponent<RecyclingBin>();
            // if component not found, skip
            if (bin == null) continue;

            // Skip if the bin is infested or is being cleaned
            if (bin.binState == BinState.INFESTED || bin.binState == BinState.CLEANING) continue;

            // compare the distance to the nearest bin
            var d = Vector3.Distance(bin.transform.position, transform.position);
            if (d < nearest_bin_d)
            {
                nearest_bin = bin.transform.position;
                nearest_bin_d = d;
            }
        }
        // If no bin was found, skip
        if (nearest_bin == Vector3.zero) return;
        // Set the direction to the nearest bin
        nearest_bin_dir = (nearest_bin - transform.position).normalized;
    }

    private void Update()
    {
        // Skip if the pest is dead
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

        // If the pest has run out of life, kill this pest
        if (life_s <= 0)
        {
            KillSelf();
        }
    }



    /// <summary>
    /// Called when the pest enters a bin. Will kill the pest.
    /// </summary>
    public void OnEnterBin(RecyclingBin bin)
    {
        // Skip if the bin is already infested or is being cleaned
        if (bin.binState == BinState.INFESTED) return;
        if (bin.binState == BinState.CLEANING) return;
        // Skip if the pest is already dead
        if (is_dead) return;
        
        Debug.Log($"Pest {this} entered bin {bin} of type {bin.recyclableType}");
        bin.StartInfestation();
        KillSelf();
    }

    public void KillSelf()
    {
        // Check if alraedy dead, if yes skip to prevent multiple calls
        if (is_dead) return;
        // Set the pest as dead
        is_dead = true;

        // Clear the destination and stop the pest
        navigation.ClearDestination();
        rb.velocity = Vector2.zero;

        // Start the death effect
        StartCoroutine(DeathEffect_Coroutine());
    }

    IEnumerator DeathEffect_Coroutine()
    {
        // Slowly fade out the sprite
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(.01f);
            spriteR.color = Color.Lerp(Color.white, Color.clear, i / 50f);
        }
        // When finished fading out, call the OnDeath event
        gameObject.SetActive(false);
        OnDeath?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sight);
    }

}