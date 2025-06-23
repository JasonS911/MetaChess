using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameMode { Local, Online }
    public GameMode currentGameMode;
    public bool playerIsWhite;

    public static GameManager Instance { get; private set; }

    public int selectedTimeMinutes { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
        SetSelectedTime();
    }

    public void SetSelectedTime()
    {
        selectedTimeMinutes = GameLauncher.selectedTimeGlobal; //time selected in GameLauncher
    }


}
