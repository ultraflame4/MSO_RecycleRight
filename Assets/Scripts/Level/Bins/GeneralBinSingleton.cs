using UnityEngine;

public class GeneralBinSingleton : MonoBehaviour {
    
    public static GeneralBinSingleton instance {get; private set;}
    
    public Vector3 worldPosition => Camera.main.ScreenToWorldPoint(canvas.worldCamera.WorldToScreenPoint(transform.position));
    
    private Canvas canvas;
    private void Awake() {
        if (instance != null){
            Debug.LogError("Multiple instances of GeneralBin found! This will cause bugs!!!");
        }
        instance = this;
    }

    private void Start() {
        canvas = GetComponentInParent<Canvas>();
    }
}