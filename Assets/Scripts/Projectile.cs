using System;
using UnityEngine;

public class Projectile : MoveTowards
{
    [Tooltip("Leave as -1 for infinite duration")]
    [SerializeField] protected float maxActiveDuration = 5f;
    [Tooltip("Whether or not object should be destroyed on hit, or set inactive")]
    [SerializeField] protected bool destroyOnHit = true;

    [HideInInspector] public float? _movementSpeed = null;
    [HideInInspector] public Vector2? _moveDirection = null;

    /// <summary>
    /// Delegate that checks if object should be hit
    /// </summary>
    /// <param name="other">Object that was hit</param>
    /// <returns>Boolean representing if hit should be detected</returns>
    public delegate bool HitCondition(Collider2D other);
    public HitCondition hitCondition = null;

    private float timeElasped = 0f;

    /// <summary>
    /// Event to be triggered when an enemy is hit
    /// </summary>
    public event Action<Projectile, Collider2D> OnHit;

    public void UpdateValues()
    {
        // check if override values are set
        movementSpeed = _movementSpeed == null ? movementSpeed : (float) _movementSpeed;
        moveDirection = _moveDirection == null ? moveDirection : (Vector2) _moveDirection;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        // do not calculate duration if object has infinite lifetime
        if (maxActiveDuration < 0f) return;
        timeElasped += Time.deltaTime;
        // end lifetime when exceed max active duration
        if (timeElasped <= maxActiveDuration) return;
        EndLifetime();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        if (hitCondition != null && !hitCondition.Invoke(other)) return;
        // invoke on hit event
        OnHit?.Invoke(this, other);
        // end object lifetime on hit
        EndLifetime();
    }

    void EndLifetime()
    {
        // reset time elasped
        timeElasped = 0f;
        // check if need to be destroyed
        if (destroyOnHit)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
