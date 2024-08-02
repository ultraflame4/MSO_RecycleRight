using System.Collections;
using System.Collections.Generic;
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
            SetCharacter(image.sprite);
        }

        /// <summary>
        /// Set the image on the character slot to the currently selected character
        /// </summary>
        /// <param name="sprite">Sprite of character to display</param>
        public void SetCharacter(Sprite sprite)
        {
            if (image == null) 
            {
                Debug.LogWarning("Image is not set! (CharacterSelectSlot.cs)");
                return;
            }

            this.sprite = sprite;
            rectTransform.pivot = GetPivot();
            image.sprite = this.sprite;
            image.SetNativeSize();
        }

        Vector2 GetPivot()
        {
            Bounds bounds = sprite.bounds;
            var pivotX = - bounds.center.x / bounds.extents.x / 2 + 0.5f;
            var pivotY = - bounds.center.y / bounds.extents.y / 2 + 0.5f;
            return new Vector2(pivotX, pivotY);
        }
    }
}
