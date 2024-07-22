using System.Collections;
using UnityEngine;

public class TrainDoorAnimation : MonoBehaviour
{
    [Header("Door Movement")]
    [SerializeField] RectTransform leftDoor;
    [SerializeField] RectTransform rightDoor;
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
        // reset animations
        doorPosition = 0f;
        animationProgress = 0f;
        UpdateDoorPosition();
        
        // TODO: this is temporary, subscribe and check for actual animation when done
        if (GameManager.Instance == null) return;
        GameManager.Instance.StartedLevelLoad += TestAnim;
    }

    // Update is called once per frame
    void Update()
    {
        if (debug_play) PlayAnimation();
        if (!debug_update_door_position) return;
        UpdateDoorPosition();
    }

    // TODO: this is temporary for testing purposes
    void TestAnim()
    {
        PlayAnimation();
        if (GameManager.Instance == null) return;
        GameManager.Instance.StartedLevelLoad -= TestAnim;
    }

    public void PlayAnimation()
    {
        if (moveAnimation != null) return;
        moveAnimation = StartCoroutine(Animate(animationProgress <= 0f));
    }

    IEnumerator Animate(bool forward_direction)
    {
        float timeElapsed = 0f;

        while (timeElapsed <= moveAnimationDuration)
        {
            timeElapsed += Time.deltaTime;
            animationProgress = Mathf.Clamp01(forward_direction ? 
                (timeElapsed / moveAnimationDuration) : 1f - (timeElapsed / moveAnimationDuration));
            UpdateAnimation();
            yield return timeElapsed;
        }

        transform.localScale = forward_direction ? new Vector3(endScale, endScale, endScale) : Vector3.one;
        transform.localPosition = forward_direction ? Vector3.zero + endOffset : Vector3.zero;
        doorPosition = forward_direction ? 1f : 0f;
        UpdateDoorPosition();
        moveAnimation = null;
        debug_play = false;
    }


    void UpdateAnimation()
    {
        if (animationProgress <= moveToZoomAnimationRatio)
        {
            doorPosition = animationProgress / moveToZoomAnimationRatio;
            doorPosition = Mathf.Clamp01(doorPosition);
            UpdateDoorPosition();
            return;
        }

        float currentCycleRatio = (animationProgress - moveToZoomAnimationRatio) / (1f - moveToZoomAnimationRatio);
        // change scale
        float currentScale = endScale + ((1f - endScale) * (1f - currentCycleRatio));
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        // change position
        Vector3 newPos = currentCycleRatio * endOffset;
        transform.localPosition = newPos;
    }

    void UpdateDoorPosition()
    {
        Vector3 newPos = new Vector3(originalDoorPosX + ((1f - doorPosition) * (screenWidth - originalDoorPosX)), 
            rightDoor.localPosition.y, rightDoor.localPosition.z);
        rightDoor.localPosition = newPos;
        newPos.x *= -1f;
        leftDoor.localPosition = newPos;
    }
}
