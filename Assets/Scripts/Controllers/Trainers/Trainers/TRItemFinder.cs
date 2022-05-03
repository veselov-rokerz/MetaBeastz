using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRItemFinder : BaseTrainers
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
                                BattleNotiController.Instance.GOSelect2CardFromYourHand.SetActive(true);
                            });
                        }
                    }
                    break;
                case 2:
                    {
                        // We get the player card from hand.
                        p.StartCoroutine(p.MoveFromHandToDiscardMultiple(response.TCardIDs, () =>
                        {
                            // We close the notifications.
                            BattleNotiController.Instance.GOSelect2CardFromYourHand.SetActive(false);

                            // When its real player.
                            if (p.IsRealPlayer)
                            {
                                // We ask for select cards.
                                DiscardPileSelectionViewController.Instance.ShowDiscardPileSelectionView(response.Deck, 1, false, (selectedCards) =>
                                 {
                                     if (selectedCards.Count > 0)
                                     {
                                        // We update the selected card.
                                        base.RequestModel.TCardID = selectedCards[0].UniqueCardID;
                                     }

                                     // We play the action.
                                     Play(false);
                                 });
                            }
                        }));
                    }
                    break;
                case 3:
                    {
                        // We draw cards.
                        p.StartCoroutine(p.MoveFromDiscardToHandMultiple(response.DrawnCards.ToList(), () =>
                        {
                            // When arrived to destination.
                            p.AddToDiscard(p.PlayerTrainer);

                            // We clear the player trainer.
                            p.PlayerTrainer = null;

                            // We return callback.
                            if (onSimulationCompleted != null)
                                onSimulationCompleted.Invoke();
                        }));

                        // We have to tell what player select to other player.
                        if (!p.IsRealPlayer)
                        {
                            // We will show the cards to opponent.
                            PlayerSelectionViewController.Instance.LoadCards(response.SCards);
                        }
                    }
                    break;
            }
        }

    }
}
