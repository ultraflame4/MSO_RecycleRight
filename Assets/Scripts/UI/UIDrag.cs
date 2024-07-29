using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIDrag :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [Header("Essentials")]
        [SerializeField] private RectTransform Canvas;
        [SerializeField] private RectTransform target;

        [Header("Padding")]
        [SerializeField] private float padding = 15f;
        [SerializeField] private float left, right, up, down;

        private Vector3 originalPosition;
        private Vector2 maxPos, minPos, originalLocalPointerPosition, tempPos;

        // Update is called once per frame
        void Update()
        {
            // update boundaries before checking if the target transform is within bounds
            UpdateBoundaries();
            // ensure target is within min and max constraints
            KeepWithinBoundaries();
        }

        public void OnBeginDrag(PointerEventData data)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    Canvas,
                    data.position,
                    data.pressEventCamera,
                    out originalLocalPointerPosition
                );

            originalPosition = target.localPosition;
        }

        public void OnDrag(PointerEventData data)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    Canvas,
                    data.position,
                    data.pressEventCamera,
                    out localPointerPosition)
                )
            {
                Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
                target.localPosition = originalPosition + offsetToOriginal;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            originalPosition = Vector3.zero;
            originalLocalPointerPosition = Vector3.zero;
            tempPos = Vector2.zero;
        }

        void UpdateBoundaries()
        {
            maxPos.x = (Screen.width / 2f) + (target.localScale.x * (target.sizeDelta.y / 4f) + padding + right);
            maxPos.y = (Screen.height / 2f) - (target.localScale.y * (target.sizeDelta.y / 2f) + padding + up);
            minPos.x = -(Screen.width / 2f) - (target.localScale.x * (target.sizeDelta.y / 4f) + padding + left);
            minPos.y = -(Screen.height / 2f) + (target.localScale.y * (target.sizeDelta.y / 2f) + padding + down);
        }

        void KeepWithinBoundaries()
        {
            tempPos = target.localPosition;

            if (target.localPosition.x < minPos.x || target.localPosition.x > maxPos.x)
                tempPos.x = target.localPosition.x < minPos.x? minPos.x : maxPos.x;
            
            if (target.localPosition.y < minPos.y || target.localPosition.y > maxPos.y)
                tempPos.y = target.localPosition.y < minPos.y? minPos.y : maxPos.y;

            target.localPosition = tempPos;
        }
    }
}
