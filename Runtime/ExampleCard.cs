using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.DragAndDropSystem
{
    [System.Serializable]
    public class ExampleCard : IStackable
    {
        public int MaxStacks
        {
            get
            {
                return maxStacks;
            }
            protected set
            {
                maxStacks = value;
            }
        }
        private int maxStacks = 3;

        public int CurrentStacks
        {
            get
            {
                return currentStacks;
            }
            set
            {
                currentStacks = value;
            }
        }
        private int currentStacks = 1;

        public bool IsStackable(IStackable otherItem)
        { 
            if (otherItem is not ExampleCard otherCard)
            {
                return false;
            }
            if (MaxStacks >= otherCard.CurrentStacks + this.CurrentStacks)
            {
                return true;
            }
            else return false;
        }
        public bool IsValidSlot(DragAndDropSlot slot)
        {
            if (slot.Display == null)
            {
                return true;
            }
            if (slot.Display.Item is IStackable stackable)
            {
                return IsStackable(stackable);
            }
            return false;
        }
        public void Merge(IStackable otherItem)
        {
            CurrentStacks += otherItem.CurrentStacks;
        }

        public IStackable Split()
        {
            CurrentStacks--;
            return new ExampleCard();
        }

        public int CalculateSellValue()
        {
            throw new NotImplementedException();
        }
    }
}