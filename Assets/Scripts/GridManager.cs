﻿using System;
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
        public float aspectRatio = 1.0f;       

        private Dictionary<string, int> prefabIDs = new Dictionary<string, int>();
        private List<int> cardIDs = new List<int>();

        public TMP_InputField rowsInputField, columnsInputField;
        public TMP_Text warning;
        public GameObject menuPanel;
      

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
            if (!IsGridValid(rows, columns))
            {
                warning.text = ($"Invalid grid: {rows} x {columns} = {rows * columns} cards. Result Must be even.");
                warning.gameObject.SetActive(true);
                Invoke("WarningTextDeactivate", 3f);
                return;
            }

            SetupGridLayout();
            CreateGrid(rows, columns);
            menuPanel.SetActive(false);
        }
        public void RestartGame()
        {

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            cardIDs.Clear();
            prefabIDs.Clear();

            menuPanel.SetActive(true); 
            warning.gameObject.SetActive(false);
        }
        public void CreateGrid(int rows, int columns)
        {

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            cardIDs.Clear();
            prefabIDs.Clear();

            int totalCards = rows * columns;
            int totalPairs = totalCards / 2;

            List<int> availableCardIDs = new List<int>();  

            List<GameObject> shuffledCardPrefabs = cardPrefabs.OrderBy(x => Guid.NewGuid()).ToList(); 

            foreach (var prefab in shuffledCardPrefabs)
            {
                if (!prefabIDs.ContainsKey(prefab.name))
                {
                    prefabIDs[prefab.name] = prefabIDs.Count; 
                }

                int id = prefabIDs[prefab.name];

                availableCardIDs.Add(id);
                availableCardIDs.Add(id);

                if (availableCardIDs.Count >= totalCards) break; 
            }

            while (availableCardIDs.Count < totalCards)
            {
                foreach (var prefab in shuffledCardPrefabs)
                {
                    int id = prefabIDs[prefab.name];
                    availableCardIDs.Add(id);
                    availableCardIDs.Add(id);
                    if (availableCardIDs.Count >= totalCards) break;
                }
            }

            Shuffle(availableCardIDs);

            for (int i = 0; i < totalCards; i++)
            {
                int cardId = availableCardIDs[i];

                string prefabName = prefabIDs.FirstOrDefault(x => x.Value == cardId).Key;
                if (prefabName != null)
                {
                    GameObject cardInstance = Instantiate(cardPrefabs.Find(p => p.name == prefabName), transform);
                    Card card = cardInstance.GetComponent<Card>();
                    card.id = cardId;
                    card.Attach(FindObjectOfType<GameManager>());
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }


        private bool IsGridValid(int rows, int columns)
        {
            if (rows < 2 || rows > 10 || columns < 2 || columns > 10)
            {
                Debug.Log("Rows and columns must be between 2 and 10.");
                return false;
            }

            return (rows * columns) % 2 == 0;
        }
        private void SetupGridLayout()
        {
            GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                Debug.Log("GridLayoutGroup component not found");
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

            if (cellWidth / cellHeight > aspectRatio)
            {
                cellWidth = cellHeight * aspectRatio;
            }
            else
            {
                cellHeight = cellWidth / aspectRatio;
            }

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

        public void RestoreGrid(SaveData data, GameManager gameManager)
        {
            RestartGame();

            rows = data.rows;
            columns = data.columns;

            SetupGridLayout();

            foreach (CardData cardData in data.cards)
            {
                GameObject prefab = cardPrefabs.Find(p => p.name == cardData.prefabName);
                if (prefab != null)
                {
                    GameObject cardObj = Instantiate(prefab, transform);
                    Card card = cardObj.GetComponent<Card>();
                    card.id = cardData.id;
                    card.isFaceUp = cardData.isFaceUp;
                    card.InitializeCardSprite();
                    card.FlipForLoad();
                    card.Attach(gameManager);

                    if (cardData.isFaceUp)
                    {
                        gameManager.GetMatchedCardIds().Add(card.id);
                    }
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}
