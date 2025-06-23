using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PersonalizationManager : MonoBehaviour
{
    public static PersonalizationManager Instance;

    private bool selectedIsWhite;

    public ReplacementOptionsDatabase database; 
    public Transform replacementListPanelContent; 
    public GameObject replacementButtonPrefab;
    private Piece selectedPiece;

    private void Awake()
    {
        Instance = this;
    }

    public void OnPieceSelected(Piece piece)
    {

        selectedPiece = piece;

        UpdateReplacementList(piece.originalType, piece.isWhite);
    }


    private void UpdateReplacementList(PieceType type, bool isWhite)
    {

        selectedIsWhite = isWhite;
        // Clear old options

        foreach (Transform child in replacementListPanelContent)
            Destroy(child.gameObject);

        // Get sprite options from the database
        List<PieceReplacementSet.ReplacementOption> options = database.GetOptionsForPiece(type, selectedIsWhite);

        foreach (PieceReplacementSet.ReplacementOption option in options)
        {

            GameObject button = Instantiate(replacementButtonPrefab, replacementListPanelContent);
            Image img = button.transform.Find("PieceImage").GetComponent<Image>();
            img.sprite = option.blackSprite;
            TMP_Text txt = button.transform.Find("PieceText").GetComponent<TMP_Text>();
            string name = option.blackSprite.name.Split("_")[1]; // Join with space if needed
            txt.text = name;
            PieceReplacementSet.ReplacementOption capturedOption = option; // avoid closure issues
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ApplyReplacement(type, capturedOption);
            });

        }

    }
    private void ApplyReplacement(PieceType targetType, PieceReplacementSet.ReplacementOption selectedOption)
    {
        Piece[] allPieces = Object.FindObjectsByType<Piece>(FindObjectsSortMode.None);

        foreach (Piece piece in allPieces)
        {

            if (piece.originalType == targetType && piece.isWhite == selectedIsWhite)
            {
                Image img = piece.GetComponent<Image>();
                if (img == null)
                    img = piece.GetComponentInChildren<Image>();

                if (img != null)
                {
                    img.sprite = selectedIsWhite ? selectedOption.whiteSprite : selectedOption.blackSprite;
                }
                piece.pieceType = selectedOption.resultingType;
            }
        }

        // Save only the sprite and type for the selected color
        if (selectedIsWhite)
        {
            CustomizationSave.SaveSpriteChoice(targetType, true, selectedOption.whiteSprite?.name);
            CustomizationSave.SaveReplacementPieceType(targetType, selectedIsWhite, selectedOption.resultingType);

        }
        else
        {
            CustomizationSave.SaveSpriteChoice(targetType, false, selectedOption.blackSprite?.name);
            CustomizationSave.SaveReplacementPieceType(targetType, selectedIsWhite, selectedOption.resultingType);
        }

        PlayerPrefs.Save();
        UpdateReplacementList(targetType, selectedIsWhite);

    }




}
