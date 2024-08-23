using System;
using UnityEngine;
using Interfaces;

public class Projectile : MoveTowards
{
    [Tooltip("Damage done to damagable object on hit")]
    public float damage = 15f;
    [Tooltip("Leave as -1 for infinite duration")]
    [SerializeField] protected float maxActiveDuration = 5f;
    [Tooltip("Whether or not object should be destroyed on hit, or set inactive")]
    [SerializeField] protected bool destroyOnHit = true;

    [HideInInspector] public float? _movementSpeed = null;
    [HideInInspector] public Vector2? _moveDirection = null;
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
        if (!other.TryGetComponent<IDamagable>(out IDamagable damagable)) return;
        // apply damage
        damagable?.Damage(damage);
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
