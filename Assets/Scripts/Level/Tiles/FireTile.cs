using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FireTile : MonoBehaviour {
    [Tooltip("How much time left (in seconds) before this fire tile is destroyed.")]
    public float lifetime = 5f;
    [Tooltip("Damage applied to player per second when standing on the fire")]
    public float fireDamage = 8f;
    [HideInInspector]
    public Tilemap associatedTilemap = null;

    private void Start() {
        StartCoroutine(LifeCycle());
        var audioSource = GetComponent<AudioSource>();
        if (SoundManager.hasInstance && audioSource != null) {
            audioSource.volume = SoundManager.Instance.volume;
        }
    }

    IEnumerator LifeCycle(){
        while (lifetime > 0){
            yield return null;
            lifetime-= Time.deltaTime;
        }
        
        if (associatedTilemap != null) {
            associatedTilemap.SetTile(associatedTilemap.WorldToCell(transform.position), null);
        }else{
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.TryGetComponent(out IFireTick fireTick)){
            fireTick.ApplyFireDamage(fireDamage);
        }
    }
}