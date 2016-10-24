namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(CanvasGroup))]
    public class VRTK_UIDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Tooltip("If unchecked then the UI element can be dropped on any valid VRTK_UICanvas, if checked then the UI element can only be dropped in valid a VRTK_UIDropZone object and must start as a child of a VRTK_UIDropZone object.")]
        public bool restrictToDropZone = false;
        [Tooltip("The offset to bring the UI element forward when it is being dragged.")]
        public float forwardOffset = 0.1f;

        [HideInInspector]
        public GameObject validDropZone;

        private RectTransform dragTransform;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private GameObject startDropZone;
        private CanvasGroup canvasGroup;
        private PointerEventData storedEventData;
        private float tickTime;

        public void OnBeginDrag(PointerEventData eventData)
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
            canvasGroup.blocksRaycasts = false;

            if (restrictToDropZone)
            {
                startDropZone = GetComponentInParent<VRTK_UIDropZone>().gameObject;
                validDropZone = startDropZone;
            }

            eventData.hovered.Clear();
            SetDragPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            storedEventData = eventData;
            tickTime = Time.deltaTime;
            SetDragPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            dragTransform = null;

            transform.position += (transform.forward * forwardOffset);
            if (restrictToDropZone)
            {
                if (validDropZone && validDropZone != startDropZone)
                {
                    transform.SetParent(validDropZone.transform);
                }
                else
                {
                    transform.position = startPosition;
                    transform.rotation = startRotation;
                }
            }
            validDropZone = null;
            storedEventData = null;
        }

        private void OnEnable()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (restrictToDropZone && !GetComponentInParent<VRTK_UIDropZone>())
            {
                enabled = false;
                Debug.LogError("A VRTK_UIDraggableItem with a `freeDrop = false` is required to be a child of a VRTK_UIDropZone GameObject.");
            }
        }

        private void SetDragPosition(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
            {
                dragTransform = eventData.pointerEnter.transform as RectTransform;
            }

            Vector3 pointerPosition;
            if (dragTransform && RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTransform, eventData.position, eventData.pressEventCamera, out pointerPosition))
            {
                transform.position = pointerPosition - (transform.forward * forwardOffset);
                transform.rotation = dragTransform.rotation;
            }
        }
    }
}