using System.Collections;
using UnityEngine;

public class TrainDoorAnimation : MonoBehaviour
{
    [Header("Door Movement")]
    [SerializeField] RectTransform leftDoor;
    [SerializeField] RectTransform rightDoor, leftStationDoor, rightStationDoor;
    [SerializeField, Range(0f, 1f)] float stationDoorOffset = 0.15f;
    [SerializeField, Range(0f, 1f)] float doorPosition = 1f;
    [SerializeField] bool debug_update_door_position = false;

    [Header("Animation")]
    [SerializeField] float moveAnimationDuration = 2.5f;
    [SerializeField] float endScale = .75f;
    [SerializeField] Vector3 endOffset = Vector3.zero;
    [SerializeField, Range(0f, 1f)] float moveToZoomAnimationRatio = 0.5f;
    [SerializeField, Range(0f, 1f)] float animationProgress;
    [SerializeField] bool debug_play = false;

    float originalDoorPosX;
    float screenWidth;
    Coroutine moveAnimation;

    // Start is called before the first frame update
    void Start()
    {
        // set default values
        transform.localPosition = Vector3.zero;
        originalDoorPosX = rightDoor.localPosition.x;
        screenWidth = (Screen.width / 2f) + originalDoorPosX;
        // reset animation
        doorPosition = 0f;
        animationProgress = 0f;
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        UpdateDoorPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (debug_play) PlayAnimation();
        if (!debug_update_door_position) return;
        UpdateDoorPosition();
    }

    public void PlayAnimation()
    {
        if (moveAnimation != null) return;
        moveAnimation = StartCoroutine(Animate(animationProgress <= 0f));
    }

    #region Animation
    IEnumerator Animate(bool forward_direction)
    {
        float timeElapsed = 0f;

        while (timeElapsed <= moveAnimationDuration)
        {
            timeElapsed += Time.deltaTime;
            animationProgress = Mathf.Clamp01(forward_direction ? 
                (timeElapsed / moveAnimationDuration) : 1f - (timeElapsed / moveAnimationDuration));
            UpdateAnimation(forward_direction);
            yield return timeElapsed;
        }

        transform.localScale = forward_direction ? new Vector3(endScale, endScale, endScale) : Vector3.one;
        transform.localPosition = forward_direction ? Vector3.zero + endOffset : Vector3.zero;
        doorPosition = forward_direction ? 1f : 0f;
        UpdateDoorPosition();
        moveAnimation = null;
        debug_play = false;
    }

    void UpdateAnimation(bool forward_direction)
    {
        if (animationProgress <= moveToZoomAnimationRatio)
        {
            doorPosition = animationProgress / moveToZoomAnimationRatio;
            doorPosition = Mathf.Clamp01(doorPosition);
            UpdateDoorPosition();
            return;
        }
        else if (forward_direction && doorPosition != 1f)
        {
            doorPosition = 1f;
            UpdateDoorPosition();
        }

        float currentCycleRatio = (animationProgress - moveToZoomAnimationRatio) / (1f - moveToZoomAnimationRatio);
        // change scale
        float currentScale = endScale + ((1f - endScale) * (1f - currentCycleRatio));
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        // change position
        Vector3 newPos = currentCycleRatio * endOffset;
        transform.localPosition = newPos;
    }
    #endregion

    #region Door Movement
    void UpdateDoorPosition()
    {
        MoveDoor(leftStationDoor, rightStationDoor, doorPosition);
        float ratio = Mathf.Clamp01((doorPosition - stationDoorOffset) / (1f - stationDoorOffset));
        MoveDoor(leftDoor, rightDoor, ratio);
    }

    void MoveDoor(RectTransform left, RectTransform right, float ratio)
    {
        Vector3 newPos = new Vector3(originalDoorPosX + ((1f - ratio) * (screenWidth - originalDoorPosX)), 
            right.localPosition.y, right.localPosition.z);
        right.localPosition = newPos;
        newPos.x *= -1f;
        left.localPosition = newPos;
    }
    #endregion
}
