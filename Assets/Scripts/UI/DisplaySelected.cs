using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySelected : MonoBehaviour
{
    //[SerializeField] Image image;
    RectTransform rectTransform;
    Sprite sprite;
    public PlayerCharacterSO[] selected_characters;


    

    // Start is called before the first frame update
    void Start()
    {
        //rectTransform = image.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    Vector2 GetPivot()
    {
        Bounds bounds = sprite.bounds;
        var pivotX = - bounds.center.x / bounds.extents.x / 1 + 0.5f;
        var pivotY = - bounds.center.y / bounds.extents.y / 1 + 0.5f;
        return new Vector2(pivotX, pivotY);
    }
}
