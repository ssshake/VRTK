namespace VRTK
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class VRTK_UIDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private VRTK_UIDraggableItem droppableItem;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag)
            {
                var dragItem = eventData.pointerDrag.GetComponent<VRTK_UIDraggableItem>();
                if (dragItem && dragItem.restrictToDropZone)
                {
                    dragItem.validDropZone = gameObject;
                    droppableItem = dragItem;
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (droppableItem)
            {
                droppableItem.validDropZone = null;
            }
            droppableItem = null;
        }
    }
}