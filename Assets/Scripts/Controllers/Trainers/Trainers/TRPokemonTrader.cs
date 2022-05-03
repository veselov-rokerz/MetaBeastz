using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRPokemonTrader : BaseTrainers
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

                        // if real ask for selection.
                        if (p.IsRealPlayer)
                        {
                            BattleNotiController.Instance.GOSelect1MonsterCardFromYourHand.SetActive(true);
                        }
                    }
                    break;
                case 2:
                    {
                        // We deactivate alert.
                        BattleNotiController.Instance.GOSelect1MonsterCardFromYourHand.SetActive(false);

                        // if real player it has to select a monster.
                        if (p.IsRealPlayer)
                        {
                            // We throw the card to target.
                            p.MoveFromHandToDeck(response.TCardID, () =>
                            {
                                // We ask for select cards.
                                DeckViewerController.Instance.ShowDeckSelectionView(response.Deck, 1, (selectedCards) =>
                                {
                                    // if there is a selecion we bind it.
                                    if (selectedCards.Count > 0)
                                    {
                                        // We update the selected card.
                                        base.RequestModel.TCardID = selectedCards[0].UniqueCardID;
                                    }

                                    // We play the action.
                                    Play(false);
                                });
                            });
                        }
                        else
                        {
                            // We throw card to deck for opponent.
                            p.MoveFromHandToDeck(response.TCardID, () =>
                            {
                                // We show the cards who selected by player.
                                PlayerSelectionViewController.Instance.LoadCards(response.SCards);
                            });
                        }
                    }
                    break;
                case 3:
                    {
                        // We make sure the card exists.
                        if (response.DrawnCards.Count > 0)
                        {
                            // We throw the card to target.
                            p.DrawACard(response.DrawnCards[0], () =>
                            {
                                // We show the cards who selected by player.
                                if (!p.IsRealPlayer)
                                    PlayerSelectionViewController.Instance.LoadCards(response.SCards);

                                // When arrived to destination.
                                p.AddToDiscard(p.PlayerTrainer);

                                // We clear the player trainer.
                                p.PlayerTrainer = null;

                                // We return callback.
                                if (onSimulationCompleted != null)
                                    onSimulationCompleted.Invoke();
                            });
                        }
                        else
                        {
                            // When arrived to destination.
                            p.AddToDiscard(p.PlayerTrainer);

                            // We clear the player trainer.
                            p.PlayerTrainer = null;

                            // We return callback.
                            if (onSimulationCompleted != null)
                                onSimulationCompleted.Invoke();
                        }
                    }
                    break;
            }
        }

    }
}
