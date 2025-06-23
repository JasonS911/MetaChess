using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverOverlay;
    public TextMeshProUGUI outcomeText;
    public TextMeshProUGUI outcomeTypeText;


    public void ShowGameOver(string winner, string outcome)
    {

        outcomeText.text = winner;
        outcomeTypeText.text = outcome;
        gameOverOverlay.SetActive(true);
    }

    public void OnNewGameButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // or custom restart logic
    }

    public void OnRematchButton()
    {
        // Implement rematch logic if multiplayer, or same as new game
        OnNewGameButton();
    }
}
