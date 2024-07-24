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
        [SerializeField] private RectTransform Canvas;
        [SerializeField] private RectTransform target;
        [SerializeField] private Vector2 maxPos;
        [SerializeField] private Vector2 minPos;

        private Vector3 originalPosition;
        private Vector2 originalLocalPointerPosition, tempPos;

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

                // check if target is within min and max constraints
                tempPos = target.localPosition;

                if (target.localPosition.x < minPos.x || target.localPosition.x > maxPos.x)
                    tempPos.x = target.localPosition.x < minPos.x? minPos.x : maxPos.x;
                
                if (target.localPosition.y < minPos.y || target.localPosition.y > maxPos.y)
                    tempPos.y = target.localPosition.y < minPos.y? minPos.y : maxPos.y;

                target.localPosition = tempPos;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            originalPosition = Vector3.zero;
            originalLocalPointerPosition = Vector3.zero;
            tempPos = Vector2.zero;
        }
    }
}
