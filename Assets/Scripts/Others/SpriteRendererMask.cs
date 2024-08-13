using System;
using UnityEngine;

/// <summary>
/// This component updates the SpriteMask's sprite to match the SpriteRenderer's sprite.
/// </summary>
public class SpriteRendererMask : MonoBehaviour
{
    [Tooltip("The SpriteRenderer to get the source sprite from.")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("The SpriteMask to update the sprite of.")]
    public SpriteMask spriteMask;

    private void Start()
    {
        spriteRenderer.RegisterSpriteChangeCallback(OnSpriteChange);
    }

    private void OnSpriteChange(SpriteRenderer arg0)
    {
        spriteMask.sprite = spriteRenderer.sprite;
    }

    private void OnDestroy()
    {
        spriteRenderer.UnregisterSpriteChangeCallback(OnSpriteChange);
    }

    private void OnPreRender() {
        spriteMask.sprite = spriteRenderer.sprite;
    }

    void Update()
    {
        spriteMask.sprite = spriteRenderer.sprite;
    }
    void LateUpdate()
    {
        spriteMask.sprite = spriteRenderer.sprite;
    }

    
}