using System;
using System.Collections;
using System.Collections.Generic;
using HexTecGames.Basics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HexTecGames.DragAndDropSystem
{
    public class DragAndDropSlot : DragAndDropSlotBase
    {
        [Space]
        [SerializeField] private Color fullColor = Color.white;

        [Space]
        [SerializeField] private bool canSend = true;

        public bool CanSend
        {
            get
            {
                return canSend;
            }
            set
            {
                canSend = value;
            }
        }

        public DragAndDropDisplay Display
        {
            get
            {
                return display;
            }
            private set
            {
                display = value;
                OnDisplayChanged?.Invoke(display);
            }
        }
        private DragAndDropDisplay display;


        public event Action<DragAndDropDisplay> OnDisplayChanged;


        private void AddDisplayEvents(DragAndDropDisplay display)
        {
            display.OnDeactivated += Display_OnDeactivated;
        }
        private void RemoveDisplayEvents(DragAndDropDisplay display)
        {
            display.OnDeactivated -= Display_OnDeactivated;
        }
        private void Display_OnDeactivated(DragAndDropDisplay display)
        {
            RemoveDisplay();
        }

        public void RemoveDisplay()
        {
            RemoveDisplayEvents(Display);
            Display = null;
            colorStack.ClearLayer(contentLayer);
        }
        public override void TransferDisplay(DragAndDropDisplay display, bool instant = false)
        {
            if (display.Item is IStackable stackable && CanMerge(stackable))
            {
                MergeItem(stackable);
                display.Deactivate();
                colorStack.ClearLayer(hoverLayer);
            }
            else
            {
                Display = display;
                AddDisplayEvents(display);
                display.SetSlot(this, instant);
            }
            colorStack.Add(fullColor, contentLayer);
        }
        public void MergeItem(IStackable otherItem)
        {
            if (Display.Item is IStackable stackable)
            {
                stackable.Merge(otherItem);
            }
        }
        public bool CanMerge(IStackable otherItem)
        {
            if (Display == null)
            {
                return false;
            }
            if (Display.Item is IStackable stackable)
            {
                return stackable.IsStackable(otherItem);
            }
            return false;
        }
        public override bool CanRecieveDisplay(DragAndDropDisplay display)
        {
            if (!CanRecieve)
            {
                return false;
            }

            return display.IsValidSlot(this);
        }
        public T TryGetItem<T>() where T : IDragAndDropItem
        {
            if (Display == null)
            {
                return default;
            }
            if (Display.Item == null)
            {
                return default;
            }
            if (Display.Item is not T t)
            {
                return default;
            }
            return t;
        }
    }
}