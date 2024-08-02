using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FireTile : MonoBehaviour {
    [Tooltip("How much time left (in seconds) before this fire tile is destroyed.")]
    public float lifetime = 5f;

    [HideInInspector]
    public Tilemap associatedTilemap = null;

    private void Start() {
        StartCoroutine(LifeCycle());
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

}