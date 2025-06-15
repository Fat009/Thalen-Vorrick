using CardGame;
using UnityEngine;

public class AutoSaveHandler : MonoBehaviour
{
    public GridManager gridManager;
    public GameManager gameManager;

    private void OnApplicationQuit()
    {
        SaveLoadManager.SaveGame(gridManager, gameManager);
        Debug.Log("Auto-saved on quit.");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // true = app is going into background
        {
            SaveLoadManager.SaveGame(gridManager, gameManager);
            Debug.Log("Auto-saved on pause.");
        }
    }
}
