using System.Collections;
using System.Collections.Generic;
using HexTecGames.Basics;
using UnityEngine;

namespace HexTecGames.DragAndDropSystem
{
    public class DragAndDropGarbageSlot : DragAndDropSlotBase
    {
        public override void TransferDisplay(DragAndDropDisplay display, bool instant = false)
        {
            display.Deactivate();
        }
    }
}