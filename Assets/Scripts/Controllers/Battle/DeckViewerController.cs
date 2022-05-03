using Assets.Scripts.BSSocket.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewerController : MonoBehaviour
{
    public static DeckViewerController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("We will activate the view.")]
    public GameObject GODeckViewer;

    [Header("We will use to load deck.")]
    public GameObject DeckViewerItem;

    [Header("Deck content area.")]
    public ScrollRect SVDeck;

    [Header("We store the expected size to prevent more than size.")]
    public int ExpectedSize;

    [Header("When an item selected we will create this.")]
    public GameObject GOSelectedItem;

    [Header("When no selected item we will show this.")]
    public GameObject GOEmptySelectedItem;

    [Header("We will store the selected items.")]
    public Transform TRSelection;

    [Header("When selection conditions ready we will activate button.")]
    public Button BTNOk;

    /// <summary>
    /// When selection completed this action will be callback.
    /// </summary>
    public Action<List<BGCardDTO>> OnCardSelected { get; private set; }

    /// <summary>
    /// We will store here the selected cards.
    /// </summary>
    public List<Tuple<BGCardDTO, GameObject>> SelectedCards = new List<Tuple<BGCardDTO, GameObject>>();

    public void ShowDeckSelectionView(BGCardDTO[] cards, int selectionCount, Action<List<BGCardDTO>> onCardSelected)
    {
        // if the game is over just retur.
        if (BattleGameController.Instance.IsGameOver) return;

        // We will wait for selection.
        this.ExpectedSize = selectionCount;

        // We set the callback.
        this.OnCardSelected = onCardSelected;

        // We activate the pile.
        GODeckViewer.gameObject.SetActive(true);

        // We remove older cards.
        SVDeck.content.RemoveAllChildsOfTransform();

        // We clear the list of selected cards.
        this.SelectedCards.Clear();

        // We remove older selected items.
        TRSelection.RemoveAllChildsOfTransform();

        // We load all the deck.
        foreach (BGCardDTO card in cards.Reverse())
        {
            // We create a deck item.
            GameObject deckItem = Instantiate(DeckViewerItem, SVDeck.content);

            // We load the image.
            deckItem.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(card.MetaData.CardId);

            // When clicked on item we will add to selected list.
            deckItem.GetComponent<Button>().onClick.AddListener(() => SelectCard(cards, deckItem, card.MetaData.CardId));
        }

        // We refresh the selection list.
        RefreshSelectedList();
    }

    public void SelectCard(BGCardDTO[] cards, GameObject deckItem, int key)
    {
        // We get the card not in selected cards.
        BGCardDTO selectedCard = cards.FirstOrDefault(x => !SelectedCards.Exists(y => y.Item1 == x) && x.MetaData.CardId == key);

        // if card is null just return.
        if (selectedCard == null) return;

        // if the size exceed return.
        if (SelectedCards.Count >= this.ExpectedSize) return;

        // We set as selected.
        SelectedCards.Add(new Tuple<BGCardDTO, GameObject>(selectedCard, deckItem));

        // We get the total card in deck.
        int quantityInDec = cards.Count(x => x.MetaData.CardId == key);

        // We find the left quantity.
        int quantity = quantityInDec - SelectedCards.Count(x => x.Item1.MetaData.CardId == key);

        // We set the quantity text.
        deckItem.transform.GetComponentInChild<TMP_Text>("QuantityText").text = $"{quantity}";

        // We refresh the selection list.
        RefreshSelectedList();
    }

    public void RefreshSelectedList()
    {
        // We remove older selected items.
        TRSelection.RemoveAllChildsOfTransform();

        // We will loop all the cards.
        foreach (Tuple<BGCardDTO, GameObject> sCard in SelectedCards)
        {
            // We create the selected item.
            GameObject deckItem = Instantiate(GOSelectedItem, TRSelection);

            // We load the image.
            deckItem.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(sCard.Item1.MetaData.CardId);

            // When clicked on item we will remove from selected list.
            deckItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                // We remove from the list.
                this.SelectedCards.Remove(sCard);

                // We set the quantity text.
                TMP_Text txtQuantity = sCard.Item2.transform.GetComponentInChild<TMP_Text>("QuantityText");

                // We parse the older quantity.
                int.TryParse(txtQuantity.text, out int quantity);

                // We increase the quantity with new one.
                txtQuantity.text = $"{quantity + 1}";

                // Refresh the list.
                RefreshSelectedList();
            });
        }

        // We also have to put empty spaces.
        for (int i = 0; i < ExpectedSize - SelectedCards.Count; i++)
        {
            // We create the selected item.
            GameObject deckItem = Instantiate(GOEmptySelectedItem, TRSelection);
        }

        // We refresh the ok button state.
        RefreshOkState();
    }

    public void RefreshOkState()
    {
        // We refresh the ok button depends on the size.
        this.BTNOk.interactable = this.SelectedCards.Count == ExpectedSize;
    }

    public void OnClickOk()
    {
        // if call back exists we just send back to response.
        if (this.OnCardSelected != null)
            this.OnCardSelected.Invoke(this.SelectedCards.Select(x => x.Item1).ToList());

        // We deactivate the deck.
        GODeckViewer.gameObject.SetActive(false);

        // We remove older cards.
        SVDeck.content.RemoveAllChildsOfTransform();

        // We also clear the selected cards.
        this.SelectedCards.Clear();

        // We also clear selection view.
        this.TRSelection.RemoveAllChildsOfTransform();
    }

}
