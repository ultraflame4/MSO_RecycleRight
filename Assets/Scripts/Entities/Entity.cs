using UnityEngine;

namespace Entity.Data
{
    public class Entity : MonoBehaviour 
    {
        [Header("Character Information")]
        public string characterName;
        [TextArea(3, 10)]
        public string characterDesc;
        public Sprite characterSprite;

        // public properties
        public new SpriteRenderer renderer { get; private set; }
        public new Collider2D collider { get; private set; }

        void Awake()
        {
            // get reference to components
            renderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
        }

        public void SetSpawn(bool active)
        {
            if (renderer != null)
                renderer.enabled = active;
            if (collider != null)
                collider.enabled = active;
        }
    }
}
