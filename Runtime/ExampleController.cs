using System.Collections;
using System.Collections.Generic;
using HexTecGames.Basics;
using UnityEngine;

namespace HexTecGames.DragAndDropSystem
{
    public class ExampleController : AdvancedBehaviour
    {
        [SerializeField] private DragAndDropController dragAndDropController = default;

        private List<ExampleCard> cards = new List<ExampleCard>();
        
        private void Start()
        {
            for (int i = 0; i < 5; i++)
            {
                ExampleCard card = new ExampleCard();
                DragAndDropDisplay display = dragAndDropController.SpawnDisplay(card);
                Vector3 targetPosition = Random.insideUnitCircle * 10;
                targetPosition.z = display.transform.position.z;
                display.SetPosition(targetPosition);
                cards.Add(card);
            }
        }
    }
}