using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexTecGames.DragAndDropSystem
{
    public interface IStackable : IDragAndDropItem
    {
        public int MaxStacks
        {
            get;
        }

        public int CurrentStacks
        {
            get;
            set;
        }

        public IStackable Split();
        public void Merge(IStackable otherItem);
        public bool IsStackable(IStackable otherItem);

    }
}