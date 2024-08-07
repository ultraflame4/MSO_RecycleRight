using UnityEngine;
using UnityEngine.UI;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterSelectSlot : MonoBehaviour
    {
        [SerializeField] Image image;
        RectTransform rectTransform;
        Sprite sprite;

        // Start is called before the first frame update
        void Start()
        {
            if (image == null) 
            {
                Debug.LogWarning("Image is not set! (CharacterSelectSlot.cs)");
                return;
            }

            rectTransform = image.GetComponent<RectTransform>();
            foreach (var button in GetComponentsInChildren<Button>())
            {
                button.onClick.AddListener(OnClicked);
            }
            SetCharacter();
        }

        /// <summary>
        /// Set the image on the character slot to the currently selected character
        /// </summary>
        /// <param name="sprite">Sprite of character to display, leave as 'null' to deselect</param>
        public void SetCharacter(Sprite sprite = null)
        {
            if (image == null) 
            {
                Debug.LogWarning("Image is not set! (CharacterSelectSlot.cs)");
                return;
            }
            
            if (sprite == null)
            {
                image.enabled = false;
                return;
            }

            this.sprite = sprite;
            rectTransform.pivot = GetPivot();
            image.sprite = this.sprite;
            image.SetNativeSize();
            image.enabled = true;
        }

        Vector2 GetPivot()
        {
            Bounds bounds = sprite.bounds;
            var pivotX = - bounds.center.x / bounds.extents.x / 2 + 0.5f;
            var pivotY = - bounds.center.y / bounds.extents.y / 2 + 0.5f;
            return new Vector2(pivotX, pivotY);
        }

        void OnClicked(){
            Debug.Log("Character slot clicked!");
            GetComponentInParent<CharacterSelectionManager>().SetHologramActive(true);
        }
    }
}
