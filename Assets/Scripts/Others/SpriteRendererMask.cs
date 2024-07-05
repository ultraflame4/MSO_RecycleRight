using UnityEngine;

/// <summary>
/// This component updates the SpriteMask's sprite to match the SpriteRenderer's sprite.
/// </summary>
public class SpriteRendererMask : MonoBehaviour {
    [Tooltip("The SpriteRenderer to get the source sprite from.")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("The SpriteMask to update the sprite of.")]
    public SpriteMask spriteMask;

    void LateUpdate() {
        spriteMask.sprite = spriteRenderer.sprite;
    }
}