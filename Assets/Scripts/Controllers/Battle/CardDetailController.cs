using Assets.Scripts.BSSocket.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailController : MonoBehaviour
{
    public static CardDetailController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    [Header("We will move this transform onto card.")]
    public RectTransform DetailRect;

    [Header("We will activate when the player 1 card info shown.")]
    public GameObject GOPlayer;

    [Header("We will activate when the player 2 card info shown.")]
    public GameObject GOOpponent;

    [Header("We will use to copy energy items.")]
    public GameObject GOEnergyItem;

    [Header("We hold the delay value.")]
    public float Delay;

    private Coroutine _activeCoroutine;
    private Transform originalParent;

    private void Start()
    {
        // We store the original parent.
        originalParent = transform;
    }

    public void ShowCard(CardController card)
    {
        // if coroutine exists we stop it
        if (_activeCoroutine != null)
        {
            // We stop the coroutine.
            StopCoroutine(_activeCoroutine);

            // We clear it.
            _activeCoroutine = null;
        }

        // We set a new coroutine.
        _activeCoroutine = StartCoroutine(ShowCardCoroutine(card));
    }

    private IEnumerator ShowCardCoroutine(CardController card)
    {
        // We wait for a second.
        yield return new WaitForSeconds(Delay);

        // if card is holding just break.
        // Or if card is holding for the action.
        if (card.IsCardDragging)
            yield break;

        // if card is not in bench or in hand just break;
        if (card.CardState != BGCardStates.Hand && card.CardState != BGCardStates.Bench)
            yield break;

        // We disable both.
        GOPlayer.gameObject.SetActive(false);
        GOOpponent.gameObject.SetActive(false);

        // We check if the realplayer card we will show the player view.
        if (card.Playground.IsRealPlayer)
        {
            // We change the parent.
            DetailRect.SetParent(card.transform);

            // We update the card position.
            DetailRect.anchoredPosition3D = Vector3.zero;

            // We change the parent.
            DetailRect.SetParent(originalParent);

            // We activate the view.
            GOPlayer.gameObject.SetActive(true);

            // We also load the card.
            GOPlayer.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(card.CardData.MetaData.CardId);

            // We will print the energies.
            Transform energies = GOPlayer.transform.Find("Energies");

            // We remove older energies.
            energies.RemoveAllChildsOfTransform();

            // We add all the energies.
            foreach(var energy in card.CardEnergyAttachment.AttachedEnergies)
            {
                // We create energy item.
                GameObject energyItem = Instantiate(GOEnergyItem, energies);

                // We load the energy.
                energyItem.GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(energy.Item2.CardData.MetaData.EnergyTypeId);
            }
        }
        else
        {
            // We change the parent.
            DetailRect.SetParent(card.transform);

            // We update the card position.
            DetailRect.anchoredPosition3D = Vector3.zero;

            // We change the parent.
            DetailRect.SetParent(originalParent);

            // We activate the view.
            GOOpponent.gameObject.SetActive(true);

            // We also load the card.
            GOOpponent.GetComponent<Image>().sprite = ResourceController.Instance.GetCardSprite(card.CardData.MetaData.CardId);
            
            // We will print the energies.
            Transform energies = GOPlayer.transform.Find("Energies");

            // We remove older energies.
            energies.RemoveAllChildsOfTransform();

            // We add all the energies.
            foreach (var energy in card.CardEnergyAttachment.AttachedEnergies)
            {
                // We create energy item.
                GameObject energyItem = Instantiate(GOEnergyItem, energies);

                // We load the energy.
                energyItem.GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(energy.Item2.CardData.MetaData.EnergyTypeId);
            }
        }
    }

    public void CloseShownCard()
    {
        // if exists we stop the coroutine.
        if (_activeCoroutine != null)
        {
            // Coroutine stopped.
            StopCoroutine(_activeCoroutine);

            // We remove it.
            _activeCoroutine = null;
        }

        // We disable the coroutines.
        GOPlayer.gameObject.SetActive(false);
        GOOpponent.gameObject.SetActive(false);
    }

}
