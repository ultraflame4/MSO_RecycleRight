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

        void Awake()
        {
            // get reference to renderer component
            renderer = GetComponent<SpriteRenderer>();
        }

        public void SetSpawn(bool active)
        {
            if (renderer == false) return;
            renderer.enabled = active;
        }
    }
}
