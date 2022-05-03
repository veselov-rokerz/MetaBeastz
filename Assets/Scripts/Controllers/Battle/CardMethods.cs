using Assets.Scripts.GSSocket.DTO;
using System.Collections.Generic;

namespace Assets.Scripts.Controllers.Battle
{
    public static class CardMethods
    {
        public static List<CardDTO> GetEvolutionCardsOfBasicCard(int cardId)
        {
            // We will store the cards.
            List<CardDTO> cards = new List<CardDTO>();

            // We get the basic card data.
            CardDTO cardData = BattleGameController.Instance.GameStartData.Deck.Find(x => x.CardId == cardId);

            // if null we will return.
            if (cardData == null) return cards;

            // if this is an evolution card just return the list.
            if (cardData.IsEvolutionCard) return cards;

            // We loop all the cards.
            while (cardData != null)
            {
                // We add to list.
                cards.Add(cardData);

                // We search for the evolution card.
                cardData = BattleGameController.Instance.GameStartData.Deck.Find(x => x.EvolutedCardId == cardData.CardId);
            }

            // We return the evolution cards.
            return cards;
        }

        public static CardDTO GetSecondStageOfBasicCard(int cardId)
        {
            // We get the basic card data.
            CardDTO cardData = BattleGameController.Instance.GameStartData.Deck.Find(x => x.CardId == cardId);

            // if null we will return.
            if (cardData == null) return null;

            // if this is an evolution card just return the list.
            if (cardData.IsEvolutionCard) return null;

            // Check for the current stage.
            int currentStage = 0;

            // We loop all the cards.
            while (cardData != null)
            {
                // We search for the evolution card.
                cardData = BattleGameController.Instance.GameStartData.Deck.Find(x => x.EvolutedCardId == cardData.CardId);

                // We increase the stage.
                currentStage++;

                // if we found the seconds stage return card.
                if (currentStage == 2)
                    return cardData;
            }

            // Otherwise we return empty.
            return null;
        }
    }
}
