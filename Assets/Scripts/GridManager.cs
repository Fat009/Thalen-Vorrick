using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace CardGame
{
    public class GridManager : MonoBehaviour
    {
        public List<GameObject> cardPrefabs;  
        public int rows = 2;                 
        public int columns = 2;              
        public float spacing = 10f;           
        public TMP_InputField rowsInputField, columnsInputField;
        public TMP_Text warning;              
        public GameObject menuPanel;        

        private List<Card> cards = new List<Card>(); 

        public void StartGame()
        {
          
            if (string.IsNullOrEmpty(rowsInputField.text) || string.IsNullOrEmpty(columnsInputField.text))
            {
                warning.text = "Rows and columns fields cannot be empty!";
                warning.gameObject.SetActive(true);
                Invoke("WarningTextDeactivate", 3f);
                return;
            }

            rows = int.Parse(rowsInputField.text); 
            columns = int.Parse(columnsInputField.text); 

            SetupGridLayout();
            CreateGrid(rows, columns);
            menuPanel.SetActive(false); 
        }

        public void CreateGrid(int rows, int columns)
        {
            
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            cards.Clear();  

            int totalCards = rows * columns; 
            int totalPairs = totalCards / 2; 

            List<int> availableCardIDs = new List<int>(); 

          
            List<GameObject> shuffledCardPrefabs = cardPrefabs.OrderBy(x => Guid.NewGuid()).ToList(); 

          
            foreach (var prefab in shuffledCardPrefabs)
            {
                int id = shuffledCardPrefabs.IndexOf(prefab);
                availableCardIDs.Add(id);
                availableCardIDs.Add(id);

                if (availableCardIDs.Count >= totalCards) break; 
            }

           
            Shuffle(availableCardIDs);

           
            for (int i = 0; i < totalCards; i++)
            {
                int cardId = availableCardIDs[i];


                GameObject cardPrefab = cardPrefabs[cardId % cardPrefabs.Count];
                GameObject cardInstance = Instantiate(cardPrefab, transform);
                Card card = cardInstance.GetComponent<Card>();
                card.id = cardId; 

                cards.Add(card);
            }

         
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private void SetupGridLayout()
        {
            GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                Debug.LogError("GridLayoutGroup component not found");
                return;
            }

            RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRect.sizeDelta;

            int padding = 10;
            gridLayoutGroup.padding = new RectOffset(padding, padding, padding, padding);

            float availableWidth = canvasSize.x - (padding * 2);
            float availableHeight = canvasSize.y - (padding * 2);

            float cellWidth = (availableWidth - (spacing * (columns - 1))) / columns;
            float cellHeight = (availableHeight - (spacing * (rows - 1))) / rows;

            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            gridLayoutGroup.spacing = new Vector2(spacing, spacing);
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns;
        }

        private void Shuffle(List<int> list)
        {
            System.Random rand = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = rand.Next(n--);
                int temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }

        private void WarningTextDeactivate()
        {
            warning.gameObject.SetActive(false);
        }
    }
}
