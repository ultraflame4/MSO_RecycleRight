using UnityEngine;

public class TestSpin : MonoBehaviour {
    private void Update() {
        var rot = transform.localEulerAngles;
        rot.z+=100 * Time.deltaTime;
        transform.localEulerAngles = rot;
    }
}