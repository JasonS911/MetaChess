using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MoveHistoryUI : MonoBehaviour
{
    public GameObject moveRowPrefab;
    public Transform moveHistoryContent;

    public void UpdateMoveHistoryUI()
    {
        foreach (Transform child in moveHistoryContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < BoardManager.moveHistory.Count; i++)
        {
            var movePair = BoardManager.moveHistory[i];

            GameObject rowObj = Instantiate(moveRowPrefab, moveHistoryContent);

            // Turn number
            rowObj.transform.Find("TurnNumber").GetComponent<TextMeshProUGUI>().text = $"{i + 1}.";

            // White piece
            var whitePieceImage = rowObj.transform.Find("WhiteMoveGroup/WhitePieceImage").GetComponent<Image>();
            var whitePieceText = rowObj.transform.Find("WhiteMoveGroup/WhitePieceText").GetComponent<TextMeshProUGUI>();

            whitePieceImage.sprite = movePair.whitePieceSprite;
            whitePieceImage.color = Color.white;
            whitePieceText.text = movePair.whiteMove ?? "";

            // Black piece
            var blackPieceImage = rowObj.transform.Find("BlackMoveGroup/BlackPieceImage").GetComponent<Image>();
            var blackPieceText = rowObj.transform.Find("BlackMoveGroup/BlackPieceText").GetComponent<TextMeshProUGUI>();

            if (movePair.blackMove != null)
            {
                blackPieceImage.sprite = movePair.blackPieceSprite;
                blackPieceImage.color = Color.white;
                blackPieceText.text = movePair.blackMove;
            }
            else
            {
                blackPieceImage.sprite = null;
                blackPieceImage.color = new Color(1, 1, 1, 0); // transparent
                blackPieceText.text = "";
            }
        }
    }

}
