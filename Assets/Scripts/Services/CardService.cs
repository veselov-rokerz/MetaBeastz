using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CardService : MonoBehaviour
{
    public static CardService Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void LoadCards(int[] cardIds, Action<List<CardDTO>> onLoaded = null)
    {
        CardDataRequestDTO requestData = new CardDataRequestDTO { CardIds = cardIds };
        GSController.Instance.SendToServer(GSMethods.SystemCards, requestData, (GSSocketResponse response) =>
        {
            // We set the system cards.
            List<CardDTO> systemCards = response.GetDataList<CardDTO>();

            // We invoke listeners.
            if (onLoaded != null)
                onLoaded.Invoke(systemCards);
        });
    }
    public CardDTO GetCardById(List<CardDTO> cards, int cardId) => cards.Find(x => x.CardId == cardId);
    public string GetCardName(int cardId) => LocalizationController.Instance.GetLanguage($"Card_{cardId}");
}
