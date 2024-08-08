using UnityEngine;

public class UIDoubleDoors : MonoBehaviour {
    public RectTransform leftDoor;
    public RectTransform rightDoor;
    
    public float OpenedDistance = 200f;
    public float ClosedDistance = 200f;

    [Range(0f, 1f)]
    public float openPercent = 0f;



    private void UpdateDoorsPosition(){
        float openWidth = Mathf.Lerp(ClosedDistance, OpenedDistance, openPercent);
        leftDoor.anchoredPosition = new Vector2(-openWidth / 2, 0);
        rightDoor.anchoredPosition = new Vector2(openWidth / 2, 0);
    }

    private void Update() {
        UpdateDoorsPosition();
    }

    private void OnAnimatorMove() {
        UpdateDoorsPosition();
    }

    private void OnValidate() {
        UpdateDoorsPosition();
    }
}
