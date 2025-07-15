using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.DragAndDropSystem
{
    public interface IDragAndDropItem
    {
        public bool IsValidSlot(DragAndDropSlot slot);
    }
}