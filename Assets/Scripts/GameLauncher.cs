using FishNet.Object;
using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load scenes

public class GameLauncher : MonoBehaviour
{
    public TimerButtonDropdown timerDropdown;
    public static int selectedTimeGlobal;


    public void PlayLocal()
    {
        int selectedTime = timerDropdown.SelectedTime;
        // Load game scene  
        selectedTimeGlobal = selectedTime; // Store globally  
        BoardManager.opponentFound = true;
        SceneManager.LoadScene("GameScene");
    }

    public void PlayOnline()
    {
        int selectedTime = timerDropdown.SelectedTime;
        Debug.Log($"Starting ONLINE game with timer: {selectedTime} minutes");

        // TODO: Load online game scene or show matchmaking screen  
    }

    public void PlayWithSteamFriend()
    {
        GameManager.Instance.IsMultiplayer = true;
        SceneManager.LoadScene("GameScene");
    }



    //go to board personalization scene  
    public void EditBoard()
    {
        SceneManager.LoadScene("PersonalizationScene");
    }

    //go to home scene  
    public void GoToHomeScene()
    {
        GameManager.Instance.IsMultiplayer = false;
        BoardManager.opponentFound = false;
        SceneManager.LoadScene("HomeScene");

    }
}
