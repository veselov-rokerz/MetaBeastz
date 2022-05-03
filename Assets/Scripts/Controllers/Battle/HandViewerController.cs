using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.GSSocket.DTO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandViewerController : MonoBehaviour
{
    public static HandViewerController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("We will activate the view.")]
    public GameObject GOHandViewer;

    [Header("We will use to load deck.")]
    public GameObject GOHandItem;

    [Header("Hand content area.")]
    public ScrollRect SVHand;

    [Header("We will change the text depends on situation.")]
    public TMP_Text TXTTitle;

    public void ShowPlayerHand(BGCardDTO[] cards)
    {
        // We will show the opponent hand.
        TXTTitle.text = $"YOUR OPPONENT HAND";

        // We activate the pile.
        GOHandViewer.gameObject.SetActive(true);

        // We remove older cards.
        SVHand.content.RemoveAllChildsOfTransform();

        // We remove older selected items.
        SVHand.content.RemoveAllChildsOfTransform();

        // We load all the deck.
        foreach (BGCardDTO card in cards.Reverse())
        {
            // We create a deck item.
            GameObject deckItem = Instantiate(GOHandItem, SVHand.content);

            // We load the image.
            deckItem.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(card.MetaData.CardId);
        }
    }

    public void ShowMulliganHand(CardDTO[] cards)
    {
        // We will show the opponent hand.
        TXTTitle.text = $"YOUR OPPONENT HAND IS MULLIGAN";

        // We activate the pile.
        GOHandViewer.gameObject.SetActive(true);

        // We remove older cards.
        SVHand.content.RemoveAllChildsOfTransform();

        // We remove older selected items.
        SVHand.content.RemoveAllChildsOfTransform();

        // We load all the deck.
        foreach (CardDTO card in cards.Reverse())
        {
            // We create a deck item.
            GameObject deckItem = Instantiate(GOHandItem, SVHand.content);

            // We load the image.
            deckItem.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(card.CardId);
        }
    }
}
