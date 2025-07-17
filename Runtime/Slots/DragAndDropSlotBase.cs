using System.Collections;
using System.Collections.Generic;
using HexTecGames.Basics;
using UnityEngine;
using UnityEngine.UI;

namespace HexTecGames.DragAndDropSystem
{
    public abstract class DragAndDropSlotBase : AdvancedBehaviour
    {
        [SerializeField] protected DragAndDropController dragAndDropController = default;
        [Space]
        [SerializeField] private Image img = default;
        [Space]
        [SerializeField] protected Color normalColor = Color.white;
        [SerializeField] protected Color validColor = Color.green;
        [SerializeField] protected Color invalidColor = Color.red;

        [Space]
        [SerializeField] private bool canRecieve = true;

        public virtual bool CanRecieve
        {
            get
            {
                return canRecieve;
            }
            set
            {
                canRecieve = value;
            }
        }

        protected int backgroundLayer = 0;
        protected int contentLayer = 1;
        protected int hoverLayer = 2;

        protected ColorStack colorStack = new ColorStack(ColorStack.Mode.Single);

        protected virtual void Awake()
        {
            colorStack.OnActiveItemChanged += ColorStack_OnActiveItemChanged;
            colorStack.Add(normalColor, backgroundLayer);
        }

        private void ColorStack_OnActiveItemChanged(Color color)
        {
            img.color = color;
        }

        public abstract void TransferDisplay(DragAndDropDisplay display, bool instant = false);

        public virtual bool CanRecieveDisplay(DragAndDropDisplay display)
        {
            return CanRecieve;
        }

        public virtual void StartHovering(DragAndDropDisplay item)
        {
            DetermineBackgroundColor(item);
        }
        public virtual void EndHovering()
        {
            colorStack.ClearLayer(hoverLayer);
        }
        private void DetermineBackgroundColor(DragAndDropDisplay display)
        {
            colorStack.ClearLayer(hoverLayer);
            colorStack.Add(CanRecieveDisplay(display) ? validColor : invalidColor, hoverLayer);
        }
    }
}