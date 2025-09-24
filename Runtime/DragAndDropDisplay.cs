using System;
using System.Collections;
using System.Collections.Generic;
using HexTecGames.Basics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HexTecGames.DragAndDropSystem
{
    public class DragAndDropDisplay : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, ISpawnable<DragAndDropDisplay>
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
            protected set
            {
                isHoverTarget = value;
            }
        }
        private bool isHoverTarget;

        public bool IsMoveable
        {
            get
            {
                return isMoveable;
            }
            set
            {
                isMoveable = value;
            }
        }
        private bool isMoveable = true;

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

        private DragAndDropSlotBase currentHoverSlot;


        public event Action<DragAndDropDisplay> OnClicked;
        //public event Action<DragAndDropDisplay> OnChanged;
        public event Action<DragAndDropDisplay> OnDeactivated;

        protected virtual void Reset()
        {
            img = GetComponent<Image>();
        }
        protected virtual void Awake()
        {
            targetPosition = transform.position;
            //Color color = Utility.GenerateRandomColor();
            //color.SetAlpha(0.8f);
            //img.color = color;
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
                    if (currentHoverSlot != null)
                    {
                        currentHoverSlot.EndHovering();
                    }
                    if (hoverSlot != null)
                    {
                        hoverSlot.StartHovering(this);
                    }

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
                ClearHoverSlot();
            }

            if (Vector3.Distance(transform.position, targetPosition) > 0.001f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition + (Vector3)mouseOffset, ref moveVelocity, 1 / moveSpeed);
            }
        }

        private void ClearHoverSlot()
        {
            if (currentHoverSlot != null)
            {
                currentHoverSlot.EndHovering();
                currentHoverSlot = null;
            }
        }

        public virtual void Setup(IDragAndDropItem item, DragAndDropController dragAndDropController)
        {
            this.Item = item;
            this.dragAndDropController = dragAndDropController;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            startingPosition = position;
            targetPosition = position;
        }

        private void StartDragging()
        {
            transform.SetAsLastSibling();
            startingPosition = transform.position;
            IsDragging = true;
            mouseOffset = transform.position - Camera.main.GetMousePosition();
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
                if (TryTransferItem(currentHoverSlot))
                {

                }
                else
                {
                    if (returnToLastPosition)
                    {
                        TryReturnToLastPosition();
                    }
                }
            }
        }
        public void SetSlot(DragAndDropSlot slot, bool instant = false)
        {
            if (this.Slot != null)
            {
                this.Slot.RemoveDisplay();
            }

            targetPosition = slot.transform.position;
            if (instant)
            {
                transform.position = targetPosition;
            }
            this.Slot = slot;
        }

        private void CreateSplit(IStackable stackable)
        {
            var item = stackable.Split();
            var display = dragAndDropController.SpawnDisplay(item);
            display.transform.position = transform.position;
            display.startingPosition = startingPosition;
            display.StartDragging();
        }

        private bool TryTransferItem(DragAndDropSlotBase slot)
        {
            if (slot.CanRecieveDisplay(this))
            {
                slot.TransferDisplay(this);
                return true;
            }
            return false;
        }

        private void TryReturnToLastPosition()
        {
            //var lastSlot = dragAndDropController.DetectSlot(Camera.main.WorldToScreenPoint(startingPosition));
            if (slot == null)
            {
                targetPosition = startingPosition;
            }
            else
            {
                targetPosition = slot.transform.position;
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsMoveable)
            {
                OnClicked?.Invoke(this);
                return;
            }
            if (Slot == null)
            {
                StartDragging();
                return;
            }
            if (!Slot.CanSend)
            {
                return;
            }
            if (Item is IStackable stackable)
            {
                if (stackable.CurrentStacks > 1)
                {
                    CreateSplit(stackable);
                    return;
                }
            }
            //Slot.RemoveDisplay();
            //Slot = null;
            StartDragging();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHoverTarget = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHoverTarget = false;
        }

        public virtual void Deactivate()
        {
            Slot = null;
            ClearHoverSlot();
            gameObject.SetActive(false);
            OnDeactivated?.Invoke(this);
        }

        public virtual bool IsValidSlot(DragAndDropSlot dragAndDropSlot)
        {
            if (Item == null)
            {
                return false;
            }
            return Item.IsValidSlot(dragAndDropSlot);
        }

        public override string ToString()
        {
            if (Item == null)
            {
                return $"{base.ToString()} [Empty]";
            }
            else return $"{base.ToString()} {Item}";
        }
    }
}