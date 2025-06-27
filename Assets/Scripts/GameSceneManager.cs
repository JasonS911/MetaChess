using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSceneManager : MonoBehaviour
{
    public enum GameMode { Local, Online }
    public GameMode currentGameMode;
    public bool playerIsWhite;

    public GameObject soundManager;
    public GameObject allButtons;
    public TMP_Text playSoundsButtonText;
    public TMP_Text flipBoardButtonText;
    public TMP_Text autoFlipBoardButtonText;
    public TMP_Text showLegalMovesButtonText;
    public Button playSoundsButton;
    public Button flipBoardButton;
    public Button autoFlipBoardButton;
    public Button showLegalMovesButton;

    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public Sprite onCheckbox;
    public Sprite offCheckbox;
    public static GameSceneManager Instance { get; private set; }

    public int selectedTimeMinutes { get; private set; }
    public bool IsMultiplayer { get; set; }

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Persist across scenes
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

    public void OnPanelHoverEnter()
    {
        playSoundsButtonText.alpha = 1.0f;
        flipBoardButtonText.alpha= 1.0f;
        autoFlipBoardButtonText.alpha= 1.0f;
        showLegalMovesButtonText.alpha= 1.0f;
    }

    public void OnPanelHoverExit()
    {
        playSoundsButtonText.alpha= 0.0f;
        flipBoardButtonText.alpha= 0.0f;
        autoFlipBoardButtonText.alpha= 0.0f;
        showLegalMovesButtonText.alpha= 0.0f;
    }

    public void playSoundsToggle()
    {
        AudioSource audioSource = soundManager.GetComponent<AudioSource>();

        if (audioSource == null || playSoundsButton == null) return;

        // Toggle mute
        audioSource.mute = !audioSource.mute;

        // Update button sprite
        playSoundsButton.image.sprite = audioSource.mute ? soundOffSprite : soundOnSprite;
    }


    public void autoFlipBoardToggle()
    {   

        BoardManager.autoFlipBoard = !BoardManager.autoFlipBoard;
        if (BoardManager.autoFlipBoard)
        {
            autoFlipBoardButton.image.sprite = onCheckbox;
        } else
        {
            autoFlipBoardButton.image.sprite = offCheckbox;

        }
    }

    public void showLegalMovesToggle()
    {
        BoardManager.showLegalMovesCheckboxOn = !BoardManager.showLegalMovesCheckboxOn;
        if (BoardManager.showLegalMovesCheckboxOn)
        {
            showLegalMovesButton.image.sprite = onCheckbox;
        } else
        {
            showLegalMovesButton.image.sprite = offCheckbox;
        }
    }

}
