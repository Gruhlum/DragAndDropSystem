using System.Collections;
using System.Collections.Generic;
using HexTecGames.Basics;
using UnityEngine;
using UnityEngine.UI;

namespace HexTecGames.DragAndDropSystem
{
    public class DragAndDropController : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster raycaster = default;
        [SerializeField] private Spawner<DragAndDropDisplay> displaySpawner = default;

        public DragAndDropDisplay SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
            }
        }
        private DragAndDropDisplay selectedItem;

        public DragAndDropSlot CurrentSlot
        {
            get
            {
                return currentSlot;
            }
            set
            {
                currentSlot = value;
            }
        }
        private DragAndDropSlot currentSlot;


        public DragAndDropSlot DetectSlot()
        {
            return DetectSlot(Input.mousePosition);
        }
        public DragAndDropSlot DetectSlot(Vector2 position)
        {
            raycaster.DetectUIObject(out DragAndDropSlot slot, position);
            return slot;
        }

        public DragAndDropDisplay SpawnDisplay(IDragAndDropItem item)
        {
            var display = displaySpawner.Spawn();
            display.Setup(item, this);
            return display;
        }
    }
}