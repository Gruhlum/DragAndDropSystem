using System;
using System.Collections;
using System.Collections.Generic;
using HexTecGames.Basics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HexTecGames.DragAndDropSystem
{
    public class DragAndDropSlot : MonoBehaviour
    {
        [SerializeField] private Image img = default;
        [Space]
        [SerializeField] private DragAndDropController slotSystemController = default;
        [Space]
        [SerializeField] private Color fullColor = Color.white;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color validColor = Color.green;
        [SerializeField] private Color invalidColor = Color.red;

        private int backgroundLayer = 0;
        private int contentLayer = 1;
        private int hoverLayer = 2;

        public DragAndDropDisplay Display
        {
            get
            {
                return display;
            }
            private set
            {
                display = value;
            }
        }
        private DragAndDropDisplay display;

        ColorStack colorStack = new ColorStack(ColorStack.Mode.Single);


        private void Awake()
        {
            colorStack.OnActiveItemChanged += ColorStack_OnActiveItemChanged;
            colorStack.Add(normalColor, backgroundLayer);
        }

        private void ColorStack_OnActiveItemChanged(Color color)
        {
            img.color = color;
        }

        public void StartHovering(DragAndDropDisplay item)
        {
            slotSystemController.CurrentSlot = this;
            DetermineBackgroundColor(item);
        }
        public void EndHovering()
        {
            slotSystemController.CurrentSlot = null;
            colorStack.ClearLayer(hoverLayer);
        }

        public void RemoveItem()
        {
            Display = null;
            colorStack.ClearLayer(contentLayer);
        }
        public void SetItem(DragAndDropDisplay display)
        {
            if (display.Item is IStackable stackable && CanMerge(stackable))
            {
                MergeItem(stackable);
                display.gameObject.SetActive(false);
                colorStack.ClearLayer(hoverLayer);
            }
            else
            {
                Display = display;
                display.SetSlot(this);
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
        private void DetermineBackgroundColor(DragAndDropDisplay display)
        {
            colorStack.ClearLayer(hoverLayer);
            colorStack.Add(IsValidItem(display.Item) ? validColor : invalidColor, hoverLayer);
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

        public bool IsValidItem(IDragAndDropItem item)
        {
            return item.IsValidSlot(this);
        }
    }
}