using UnityEngine;

public class GeneralBinSingleton : MonoBehaviour {
    
    public static GeneralBinSingleton instance {get; private set;}

    private void Awake() {
        if (instance != null){
            Debug.LogError("Multiple instances of GeneralBin found! This will cause bugs!!!");
        }
        instance = this;
    }

}