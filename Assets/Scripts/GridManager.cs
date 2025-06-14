using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

            int totalCards = rows * columns; 

          
            for (int i = 0; i < totalCards; i++)
            {
               
                GameObject cardPrefab = cardPrefabs[Random.Range(0, cardPrefabs.Count)];

               
                GameObject cardInstance = Instantiate(cardPrefab, transform);
                cardInstance.transform.localPosition = Vector3.zero; 
            }

         
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
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

         
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            gridLayoutGroup.spacing = new Vector2(spacing, spacing);
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns;
        }

        private void WarningTextDeactivate()
        {
            warning.gameObject.SetActive(false);
        }
    }
}
