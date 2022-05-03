using Assets.Scripts.BSSocket.Enums;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscardPileViewerController : MonoBehaviour
{
    public static DiscardPileViewerController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("We will activate the view.")]
    public GameObject GODiscardPileViewer;

    [Header("We will use to load discard pile.")]
    public GameObject DiscardPileViewerItem;

    [Header("Discard pile content area.")]
    public ScrollRect SVDiscardPile;

    public void LoadDiscardPile(PlaygroundController cardMovement)
    {
        // if the game is over just retur.
        if (BattleGameController.Instance.IsGameOver) return;

        // We activate the pile.
        GODiscardPileViewer.gameObject.SetActive(true);

        // We remove older cards.
        SVDiscardPile.content.RemoveAllChildsOfTransform();

        // We group all the cards.
        var discardPile = cardMovement.PlayerDiscard.GroupBy(x => x.CardData.MetaData.CardId)
            .Select(x => new { Key = x.Key, Count = x.Count() })
            .Reverse()
            .ToList();

        // We load all the discard pile.
        foreach (var discardItem in discardPile)
        {
            // We create a pile item.
            GameObject discardPileItem = Instantiate(DiscardPileViewerItem, SVDiscardPile.content);

            // We load the image.
            discardPileItem.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(discardItem.Key);

            // We set the quantity text.
            discardPileItem.transform.GetComponentInChild<TMP_Text>("QuantityText").text = discardItem.Count.ToString();
        }
    }

}
