using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HexTecGames.DragAndDropSystem
{
    public class DragAndDropDisplay : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image img = default;
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float centerSpeed = 10f;
        [Space]
        [SerializeField] private bool returnToLastPosition = true;
        [Space]
        [SerializeField] private DragAndDropController dragAndDropController = default;


        public IDragAndDropItem Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }
        private IDragAndDropItem item;


        public virtual bool IsDragging
        {
            get
            {
                return isDragging;
            }
            protected set
            {
                isDragging = value;
            }
        }
        private bool isDragging;

        public virtual bool IsHoverTarget
        {
            get
            {
                return isHoverTarget;
            }
            protected  set
            {
                isHoverTarget = value;
            }
        }
        private bool isHoverTarget;


        public DragAndDropSlot Slot
        {
            get
            {
                return slot;
            }
            private set
            {
                slot = value;
            }
        }
        private DragAndDropSlot slot;


        Vector2 centerVelocity;
        Vector3 moveVelocity;
        Vector3 startingPosition;

        private Vector2 mouseOffset;
        private Vector3 targetPosition;

        private DragAndDropSlot currentHoverSlot;



        protected virtual void Reset()
        {
            img = GetComponent<Image>();
        }
        protected virtual void Awake()
        {
            targetPosition = transform.position;
            Color color = Utility.GenerateRandomColor();
            color.SetAlpha(0.8f);
            img.color = color;
        }

        protected virtual void Update()
        {
            if (mouseOffset.magnitude > 0.001f)
            {
                mouseOffset = Vector2.SmoothDamp(mouseOffset, Vector2.zero, ref centerVelocity, 1 / centerSpeed);
            }

            if (IsDragging)
            {
                var hoverSlot = dragAndDropController.DetectSlot();
                if (currentHoverSlot != hoverSlot)
                {
                    if (hoverSlot == null)
                    {
                        currentHoverSlot.EndHovering();
                    }
                    else hoverSlot.StartHovering(this);

                    currentHoverSlot = hoverSlot;
                }
                targetPosition = Camera.main.GetMousePosition();
                targetPosition.z = transform.position.z;

                if (Input.GetMouseButtonUp(0))
                {
                    StopDragging();
                }
            }
            else
            {
                if (currentHoverSlot != null)
                {
                    currentHoverSlot.EndHovering();
                    currentHoverSlot = null;
                }
            }

            if (Vector3.Distance(transform.position, targetPosition) > 0.001f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition + (Vector3)mouseOffset, ref moveVelocity, 1 / moveSpeed);
            }
        }

        public void Setup(IDragAndDropItem item, DragAndDropController dragAndDropController)
        {
            this.Item = item;
            this.dragAndDropController = dragAndDropController;
        }

        public void RemoveSlot()
        {
            Slot = null;
            //StartItemDrag();
        }

        private void StartItemDrag()
        {
            transform.SetAsLastSibling();
            startingPosition = transform.position;
            IsDragging = true;
            dragAndDropController.SelectedItem = this;
            mouseOffset = transform.position - Camera.main.GetMousePosition();
        }

        public void SetSlot(DragAndDropSlot slot)
        {
            targetPosition = slot.transform.position;
            startingPosition = Camera.main.GetMousePosition();
            this.Slot = slot;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Slot != null && Item is IStackable stackable)
            {
                if (stackable.CurrentStacks > 1)
                {
                    var item = stackable.Split();
                    var display = dragAndDropController.SpawnDisplay(item);
                    display.transform.position = transform.position;
                    display.startingPosition = startingPosition;
                    display.StartItemDrag();
                    return;
                }
            }
            StartItemDrag();
            if (Slot != null)
            {
                Slot.RemoveItem();
                startingPosition = Slot.transform.position;
                Slot = null;
            }
        }

        public void StopDragging()
        {
            IsDragging = false;

            if (currentHoverSlot == null)
            {
                if (returnToLastPosition)
                {
                    TryReturnToLastPosition();
                }
            }
            else
            {
                TryTransferItem(currentHoverSlot);
            }
        }

        private bool TryTransferItem(DragAndDropSlot slot)
        {
            if (slot.IsValidItem(this.Item))
            {
                slot.SetItem(this);
                return true;
            }
            return false;
        }

        private void TryReturnToLastPosition()
        {
            var lastSlot = dragAndDropController.DetectSlot(Camera.main.WorldToScreenPoint(startingPosition));
            if (lastSlot == null)
            {
                targetPosition = startingPosition;
            }
            else
            {
                if (!TryTransferItem(lastSlot))
                {
                    targetPosition = startingPosition;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHoverTarget = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHoverTarget = false;
        }
    }
}