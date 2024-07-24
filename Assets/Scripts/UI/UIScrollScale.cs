using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIScrollScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Vector2 scaleBoundaries;
        [SerializeField] float scaleAmount = 5f;
        Vector3 originalScale;
        float input;
        bool pointer_over_self;

        // Start is called before the first frame update
        void Start()
        {
            originalScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            pointer_over_self = false;
        }

        // Update is called once per frame
        void Update()
        {
            // ensure mouse is not over other UI
            if (!pointer_over_self) return;
            // get input and ensure it is not 0
            input = Input.mouseScrollDelta.y;
            if (input == 0f) return;
            // scale UI based on input
            Scale(scaleAmount * (input < 0f ? -1f : 1f));
        }

        void Scale(float amount)
        {
            transform.localScale += Vector3.one * amount * Time.deltaTime;
            // ensure scale is within boundaries, only need to check one value because the scale amount is the same
            // if not within scale boundaries, reset scale to boundary
            if (transform.localScale.x < originalScale.x - scaleBoundaries.x)
                transform.localScale = originalScale - Vector3.one * scaleBoundaries.x;
            else if (transform.localScale.x > originalScale.y + scaleBoundaries.y)
                transform.localScale = originalScale + Vector3.one * scaleBoundaries.y;
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            pointer_over_self = true;
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            pointer_over_self = false;
        }
    }
}

