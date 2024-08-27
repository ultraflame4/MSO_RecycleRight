using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player.Behaviours
{
    public class BaseRangedAttack : Behaviour
    {
        [Header("Ranged Attack")]
        [Tooltip("Damage of projectile upon hitting target")]
        [SerializeField] protected float damage = 15f;
        [Tooltip("Movement speed of projectile")]
        [SerializeField] protected float movementSpeed = 15f;
        [Tooltip("Whether or not projectile should be rotated to face move direction")]
        [SerializeField] protected bool rotateProjectile = true;
        [Tooltip("Prefab of projectile to fire (Must contain projectile script)")]
        [SerializeField] protected Projectile projectilePrefab;

        // use object pooling
        protected List<Projectile> projectilePool = new List<Projectile>();

        /// <summary>
        /// Event to be triggered when a projectile is launched
        /// </summary>
        public event Action<Projectile> OnLaunch;

        // perform default ranged attack
        public override void TriggerAttack()
        {
            Vector2 direction = (character.pointer.position - character.transform.position).normalized;
            Projectile projectile = InstantiateProjectile();
            projectile.transform.up = rotateProjectile ? direction : projectile.transform.up;
            projectile._moveDirection = rotateProjectile ? Vector3.up : direction;
            projectile._movementSpeed = movementSpeed;
            projectile.damage = damage;
            projectile.UpdateValues();
            OnLaunch?.Invoke(projectile);
        }

        Projectile InstantiateProjectile()
        {
            projectilePool = projectilePool.Where(x => x != null).ToList();
            List<Projectile> availableObjects = projectilePool.Where(x => !x.gameObject.activeInHierarchy).ToList();
            Projectile projectile;

            // check if there are available objects, otherwise create a new one
            if (availableObjects != null && availableObjects.Count > 0)
            {
                projectile = availableObjects[0];
                projectile.gameObject.SetActive(true);
            }
            else
            {
                GameObject newObject = Instantiate(projectilePrefab.gameObject);
                projectile = newObject.GetComponent<Projectile>();
                projectilePool.Add(projectile);
            }
            
            // reset projectile position and rotation before returning
            projectile.transform.position = character.transform.position;
            projectile.transform.rotation = Quaternion.identity;
            return projectile;
        }
    }
}
