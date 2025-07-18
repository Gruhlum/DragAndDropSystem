using System.Collections;
using System.Collections.Generic;
using HexTecGames.Basics;
using UnityEngine;

namespace HexTecGames.DragAndDropSystem
{
    public static class Extensions
    {
        public static DragAndDropSlot FindSlot(this List<DragAndDropSlot> slots, IDragAndDropItem card)
        {
            foreach (var slot in slots)
            {
                if (slot.Display == null)
                {
                    continue;
                }
                if (slot.Display.Item == card)
                {
                    return slot;
                }
            }
            return null;
        }
        public static DragAndDropDisplay FindDisplay(this List<DragAndDropSlot> slots, IDragAndDropItem card)
        {
            foreach (var slot in slots)
            {
                if (slot.Display == null)
                {
                    continue;
                }
                if (slot.Display.Item == card)
                {
                    return slot.Display;
                }
            }
            return null;
        }
    }
}