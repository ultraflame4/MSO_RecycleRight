using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainDoorManager : MonoBehaviour
{
    [SerializeField] RectTransform leftDoor;
    [SerializeField] RectTransform rightDoor;

    [SerializeField, Range(0f, 1f)] float doorPosition = 1f;
    public float DoorPosition
    {
        get { return doorPosition; }
        set 
        { 
            doorPosition = Mathf.Clamp01(value); 
            UpdateDoorPosition();
        }
    }

    [SerializeField] bool debug_update_door_position = false;

    float originalDoorPosX;
    float screenWidth;

    // Start is called before the first frame update
    void Start()
    {
        originalDoorPosX = rightDoor.localPosition.x;
        screenWidth = (Screen.width / 2f) + originalDoorPosX;
    }

    // Update is called once per frame
    void Update()
    {
        if (!debug_update_door_position) return;
        UpdateDoorPosition();
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
