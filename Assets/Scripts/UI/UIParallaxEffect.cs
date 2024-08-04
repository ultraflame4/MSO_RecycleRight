using UnityEngine;

namespace UI
{
    public class UIParallaxEffect : MonoBehaviour
    {
        [SerializeField] float maxMoveAmount = 15f;
        Vector2 originalPosition, newPosition, mousePosition;
        float width, height, x, y;

        // Start is called before the first frame update
        void Start()
        {
            originalPosition = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            mousePosition = GetMousePosition();
            newPosition = originalPosition;
            newPosition.x = originalPosition.x + ((mousePosition.x / width) * maxMoveAmount);
            newPosition.y = originalPosition.y + ((mousePosition.y / height) * maxMoveAmount);
            transform.localPosition = newPosition;
        }

        Vector2 GetMousePosition()
        {
            width = Screen.width / 2f;
            height = Screen.height / 2f;
            x = Input.mousePosition.x - width;
            y = Input.mousePosition.y - height;
            x = Mathf.Clamp(x, -width, width);
            y = Mathf.Clamp(y, -height, height);
            return new Vector2(x, y);
        }
    }
}
