using System.Collections;
using System.Collections.Generic;
using HexTecGames.Basics;
using UnityEngine;

namespace HexTecGames.DragAndDropSystem
{
    public class ExampleController : MonoBehaviour
    {
        [SerializeField] private DragAndDropController dragAndDropController = default;

        private List<Card> cards = new List<Card>();
        
        private void Start()
        {
            for (int i = 0; i < 5; i++)
            {
                Card card = new Card();
                dragAndDropController.SpawnDisplay(card);
                cards.Add(card);
            }
        }
    }
}