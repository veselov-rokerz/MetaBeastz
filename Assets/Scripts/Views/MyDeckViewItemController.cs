using Assets.Scripts.Enums;
using Assets.Scripts.GSSocket.DTO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyDeckViewItemController : MonoBehaviour
{
    [Header("We are also going to print the deck name.")]
    public TMP_Text TXTDeckName;

    [Header("Image of the deck.")]
    public Image IMGDeck;

    [Header("We store the user deck informations.")]
    public UserDeckDTO UserDeck;

    public void LoadDeckDetails(UserDeckDTO userDeck)
    {
        this.UserDeck = userDeck;

        // We load the deck image.
        IMGDeck.sprite = ResourceController.Instance.GetDeckSprite((int)userDeck.UserDeckId);

        // We set the deck name.
        TXTDeckName.text = userDeck.DeckName;

        // if this one is selected.
        transform.Find("Selected").gameObject.SetActive(userDeck.UserDeckId == (int)UserDeckService.Instance.SelectedDeck);
    }

    public void OnClick()
    {
        // We select the deck.
        UserDeckService.Instance.SelectedDeck = (Decks)UserDeck.UserDeckId;

        // We refresh the view.
        MyCardPanelController.Instance.MyDeckView.LoadMyDecks();
    }
}
