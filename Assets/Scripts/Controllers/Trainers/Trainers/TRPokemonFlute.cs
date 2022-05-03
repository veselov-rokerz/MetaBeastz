using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRPokemonFlute : BaseTrainers
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

            // We get opponent.
            PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(p);

            switch (response.ActionNumber)
            {
                case 1:
                    {
                        // We get the card in player hand.
                        CardController cardInHand = p.GetCardInHand(response.PCard.UniqueCardID);

                        // if card is in player hand.
                        if (cardInHand != null)
                            p.MoveFromHandToTrainer(cardInHand);

                        // if real player it has to select a monster.
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
                    }
                    break;
                case 2:
                    {
                        // Move from discard to bench.
                        op.MoveFromDiscardToBench(response.TCardID, () =>
                         {
                             // When arrived to destination.
                             p.AddToDiscard(p.PlayerTrainer);

                             // We clear the player trainer.
                             p.PlayerTrainer = null;

                             // We return callback.
                             if (onSimulationCompleted != null)
                                 onSimulationCompleted.Invoke();
                         });
                    }
                    break;
            }
        }

    }
}
