using Assets.Scripts.BSSocket.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscardPileSelectionViewController : MonoBehaviour
{
    public static DiscardPileSelectionViewController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("We will activate the view.")]
    public GameObject GODiscardViewer;

    [Header("We will use to load discard pile.")]
    public GameObject DiscardViewerItem;

    [Header("Discard content area.")]
    public ScrollRect SVDiscard;

    [Header("When an item selected we will create this.")]
    public GameObject GOSelectedItem;

    [Header("When no selected item we will show this.")]
    public GameObject GOEmptySelectedItem;

    [Header("We will store the selected items.")]
    public Transform TRSelection;

    [Header("When selection conditions ready we will activate button.")]
    public Button BTNOk;

    /// <summary>
    /// We store the expected size to prevent more than size.
    /// </summary>
    public int ExpectedSize { get; set; }

    /// <summary>
    /// Is all of them must be selected?
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// When selection completed this action will be callback.
    /// </summary>
    public Action<List<BGCardDTO>> OnCardSelected { get; private set; }

    /// <summary>
    /// List of cards.
    /// </summary>
    public BGCardDTO[] Cards { get; private set; }

    /// <summary>
    /// We will store here the selected cards.
    /// </summary>
    public List<BGCardDTO> SelectedCards = new List<BGCardDTO>();

    public void ShowDiscardPileSelectionView(BGCardDTO[] cards, int selectionCount, bool isRequired, Action<List<BGCardDTO>> onCardSelected)
    {
        // if the game is over just retur.
        if (BattleGameController.Instance.IsGameOver) return;

        // We store the cards.
        this.Cards = cards;

        // When player has to select all.
        this.IsRequired = isRequired;

        // We will wait for selection.
        this.ExpectedSize = selectionCount;

        // We set the callback.
        this.OnCardSelected = onCardSelected;

        // We activate the pile.
        GODiscardViewer.gameObject.SetActive(true);

        // We clear the list of selected cards.
        this.SelectedCards.Clear();

        // We refresh the selection list.
        RefreshSelectedList();
    }

    public void SelectCard(int cardId)
    {
        // We get the card not in selected cards.
        BGCardDTO selectedCard = this.Cards.FirstOrDefault(x => !SelectedCards.Exists(y => y.UniqueCardID == x.UniqueCardID) && x.MetaData.CardId == cardId);

        // if card is null just return.
        if (selectedCard == null) return;

        // if the size exceed return.
        if (SelectedCards.Count >= this.ExpectedSize) return;

        // We set as selected.
        SelectedCards.Add(selectedCard);

        // We refresh the selection list.
        RefreshSelectedList();
    }

    public void DeSelect(BGCardDTO card)
    {
        // We remove from the list.
        this.SelectedCards.Remove(card);

        // We refresh the view.
        RefreshSelectedList();
    }

    public void RefreshSelectedList()
    {
        // We also refresh the discard view.
        SVDiscard.content.RemoveAllChildsOfTransform();

        // We remove older selected items.
        TRSelection.RemoveAllChildsOfTransform();

        // We group all the cards.
        List<Tuple<int, int>> deck = this.Cards.GroupBy(x => x.MetaData.CardId)
            .Select(x => new Tuple<int, int>(x.Key, x.Count()))
            .Reverse()
            .ToList();

        // We load all the deck.
        foreach (Tuple<int, int> card in deck)
        {
            // We create a deck item.
            GameObject deckItem = Instantiate(DiscardViewerItem, SVDiscard.content);

            // We load the image.
            deckItem.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(card.Item1);

            // We find the left quantity.
            int quantity = card.Item2 - SelectedCards.Count(x => x.MetaData.CardId == card.Item1);

            // We set the quantity text.
            deckItem.transform.GetComponentInChild<TMP_Text>("QuantityText").text = $"{quantity}";

            // When clicked on item we will add to selected list.
            deckItem.GetComponent<Button>().onClick.AddListener(() => SelectCard(card.Item1));
        }

        // We load all the deck.
        foreach (BGCardDTO card in SelectedCards)
        {
            // We create a deck item.
            GameObject deckItem = Instantiate(GOSelectedItem, TRSelection);

            // We load the image.
            deckItem.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(card.MetaData.CardId);

            // When clicked on item we will add to selected list.
            deckItem.GetComponent<Button>().onClick.AddListener(() => DeSelect(card));
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
        // if not required just return.
        if (!this.IsRequired)
        {
            // We activate the button.
            this.BTNOk.interactable = true;

            // We dont have to go more.
            return;
        }

        // We refresh the ok button depends on the size.
        this.BTNOk.interactable = this.SelectedCards.Count == ExpectedSize;
    }

    public void OnClickOk()
    {
        // if call back exists we just send back to response.
        if (this.OnCardSelected != null)
            this.OnCardSelected.Invoke(this.SelectedCards);

        // We deactivate the deck.
        GODiscardViewer.gameObject.SetActive(false);

        // We remove older cards.
        SVDiscard.content.RemoveAllChildsOfTransform();

        // We also clear the selected cards.
        this.SelectedCards.Clear();

        // We also clear selection view.
        this.TRSelection.RemoveAllChildsOfTransform();
    }

}
