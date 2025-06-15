using CardGame;
using UnityEngine;

public class UIControls : MonoBehaviour
{
    public GridManager gridManager;
    public GameManager gameManager;

    public void SaveGame()
    {
            SaveLoadManager.SaveGame(gridManager, gameManager); 
    }
    

    public void LoadGame()
    {
        SaveLoadManager.LoadGame(gridManager, gameManager);
        gridManager.menuPanel.SetActive(false);
    }
}
