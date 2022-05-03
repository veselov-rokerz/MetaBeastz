using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TREnergyRetrieval : BaseTrainers
    {
        public override void Play(bool firstTime)
        {
            // Send to request to server.
            if (firstTime)
                BattleGameController.Instance.SendGameAction(BattleGameActions.PlayTrainerCard, base.RequestModel);
            else
                BattleGameController.Instance.SendGameAction(BattleGameActions.PlayTrainerCardEffect, base.RequestModel);
        }

        public override void Simulate(BGTrainerResponsePlayerDTO response, Action onSimulationCompleted)
        {
            // We have to override action.
            base.Simulate(response, onSimulationCompleted);

            // We get player.
            PlaygroundController p = BattleGameController.Instance.GetPlaygroundByPlayer(response.Player);

            switch (response.ActionNumber)
            {
                case 1:
                    {
                        // We get the card in player hand.
                        CardController cardInHand = p.GetCardInHand(response.PCard.UniqueCardID);

                        // if card is in player hand.
                        if (cardInHand != null)
                        {
                            p.MoveFromHandToTrainer(cardInHand);
                        }
                        else
                        {
                            // if player who played trainer we will just call the back.
                            p.PlayerTrainer.SetArriveAction(() =>
                            {
                                // We ask for the selection.
                                BattleNotiController.Instance.GOSelect1CardFromYourHand.SetActive(true);
                            });
                        }
                    }
                    break;
                case 2:
                    {
                        // if not real player we have to set card data.
                        if (!p.IsRealPlayer)
                        {
                            // We make sure its in player hand.
                            if (p.IsCardInHand(response.PCard.UniqueCardID))
                            {
                                // if it is we get the card in hand.
                                CardController cardInPlayerHand = p.GetCardInHand(response.PCard.UniqueCardID);

                                // We load the data.
                                cardInPlayerHand.SetCardData(response.PCard);
                            }
                        }

                        // We get the player card from hand.
                        p.MoveFromHandToDiscard(response.TCardID);

                        // We close the notifications.
                        BattleNotiController.Instance.GOSelect1CardFromYourHand.SetActive(false);

                        // When its real player.
                        if (p.IsRealPlayer)
                        {
                            // We ask for select cards.
                            DiscardPileSelectionViewController.Instance.ShowDiscardPileSelectionView(response.Deck, 2, false, (selectedCards) =>
                            {
                                // We update the selected card.
                                base.RequestModel.TCardIDs = selectedCards.Select(x => x.UniqueCardID).ToList();

                                // We play the action.
                                Play(false);
                            });
                        }
                    }
                    break;
                case 3:
                    {
                        // We have to tell what player select to other player.
                        if (!p.IsRealPlayer)
                        {
                            // We will show the cards to opponent.
                            PlayerSelectionViewController.Instance.LoadCards(response.SCards);
                        }

                        // We draw cards.
                        p.StartCoroutine(p.MoveFromDiscardToHandMultiple(response.DrawnCards, () =>
                        {
                            // When arrived to destination.
                            p.AddToDiscard(p.PlayerTrainer);

                            // We clear the player trainer.
                            p.PlayerTrainer = null;

                            // We return callback.
                            if (onSimulationCompleted != null)
                                onSimulationCompleted.Invoke();
                        }));
                    }
                    break;
            }
        }

    }
}
