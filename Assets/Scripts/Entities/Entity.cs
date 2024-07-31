using UnityEngine;

namespace Entity.Data
{
    public class Entity : MonoBehaviour 
    {
        [HideInInspector] public string characterName;
        [HideInInspector] public string characterDesc;
        [HideInInspector] public Sprite characterSprite;

        // public properties
        public new Collider2D collider { get; private set; }
        public new SpriteRenderer renderer { get; private set; }
        public bool Enabled { get; private set; } = true;

        protected virtual void Awake()
        {
            // get reference to components
            collider = GetComponent<Collider2D>();
            renderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void SetSpawn(bool active)
        {
            if (renderer != null)
                renderer.enabled = active;
            if (collider != null)
                collider.enabled = active;
            Enabled = active;
        }
    }
}
