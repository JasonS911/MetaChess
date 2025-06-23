using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TimerButtonDropdown : MonoBehaviour
{
    private int selectedTime;
    public int SelectedTime => selectedTime;

    private void Awake()
    {
        selectedTime = 10; // Default to 10 minutes (Rapid)
    }
    public GameObject dropdownContent;
    private bool isOpen = false;
    public RectTransform arrowIcon;

    public void ToggleDropdown()
    {
        isOpen = !isOpen;
        dropdownContent.SetActive(isOpen);
        arrowIcon.rotation = Quaternion.Euler(0, 0, isOpen ? 180 : 0);

    }

    public TextMeshProUGUI timerText;
    public Image gameTypeIcon;
    public Sprite bulletIcon;
    public Sprite blitzIcon;
    public Sprite rapidIcon;
    public Sprite classicalIcon;

    public GameObject bulletButton1;
    public GameObject blitzButton3;
    public GameObject blitzButton5;
    public GameObject rapidButton10;
    public GameObject rapidButton15;
    public GameObject classicalButton30;
    public void SetTimerText(int time)
    {
        selectedTime = time;
        // First disable all outlines
        bulletButton1.GetComponent<Outline>().enabled = false;
        blitzButton3.GetComponent<Outline>().enabled = false;
        blitzButton5.GetComponent<Outline>().enabled = false;
        rapidButton10.GetComponent<Outline>().enabled = false;
        rapidButton15.GetComponent<Outline>().enabled = false;
        classicalButton30.GetComponent<Outline>().enabled = false;

        // Now handle the selected one
        Outline outline = null;

        switch (time)
        {
            case 1:
                timerText.text = "1 min (Bullet)";
                gameTypeIcon.sprite = bulletIcon;
                gameTypeIcon.rectTransform.sizeDelta = new Vector2(40, 40);
                outline = bulletButton1.GetComponent<Outline>();
                break;

            case 3:
                timerText.text = "3 min (Blitz)";
                gameTypeIcon.sprite = blitzIcon;
                gameTypeIcon.rectTransform.sizeDelta = new Vector2(40, 40);
                outline = blitzButton3.GetComponent<Outline>();
                break;

            case 5:
                timerText.text = "5 min (Blitz)";
                gameTypeIcon.sprite = blitzIcon;
                gameTypeIcon.rectTransform.sizeDelta = new Vector2(40, 40);
                outline = blitzButton5.GetComponent<Outline>();
                break;

            case 10:
                timerText.text = "10 min (Rapid)";
                gameTypeIcon.sprite = rapidIcon;
                gameTypeIcon.rectTransform.sizeDelta = new Vector2(30, 40);
                outline = rapidButton10.GetComponent<Outline>();
                break;

            case 15:
                timerText.text = "15 min (Rapid)";
                gameTypeIcon.sprite = rapidIcon;
                gameTypeIcon.rectTransform.sizeDelta = new Vector2(30, 40);
                outline = rapidButton15.GetComponent<Outline>();
                break;

            case 30:
                timerText.text = "30 min (Rapid)";
                gameTypeIcon.sprite = classicalIcon;
                gameTypeIcon.rectTransform.sizeDelta = new Vector2(20, 30);
                outline = classicalButton30.GetComponent<Outline>();
                break;

            default:
                Debug.LogWarning($"Unknown time option: {time}");
                return;
        }

        // Finally enable the selected outline
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

}
