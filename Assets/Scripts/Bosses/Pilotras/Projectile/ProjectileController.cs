using UnityEngine;
using Interfaces;

namespace Bosses.Pilotras.Projectile
{
    public class ProjectileController : MoveTowards
    {
        [SerializeField] float rotationSpeed = 60f;
        public SpriteRenderer spriteRenderer;

        [HideInInspector] public float damage = 25f;
        [HideInInspector] public float maxX = 0f;
        [HideInInspector] public GameObject npcPrefab;

        protected override void Update()
        {
            // move and rotate projectile
            transform.position = (Vector2) transform.position + (moveDirection.normalized * movementSpeed * Time.deltaTime);
            transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);

            // destroy self after moving past max X position
            if (transform.position.x <= maxX) return;
            Destroy(gameObject);
        }

        void OnTriggerEnter(Collider other) 
        {
            // when hit something, damage it and spawn a NPC in its place
            if (other.TryGetComponent<IDamagable>(out IDamagable damagable))
                damagable.Damage(damage);
            Instantiate(npcPrefab, transform.position, Quaternion.identity, transform.parent);
            Destroy(gameObject);
        }
    }
}
