using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRLass : BaseTrainers
    {
        public override void Play(bool firstTime)
        {
            // Send to request to server.
            if (firstTime)
                BattleGameController.Instance.SendGameAction(BattleGameActions.PlayTrainerCard, base.RequestModel);
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

                        // We will execute this action below.
                        void actionToExecute()
                        {
                            // We show the opponent hand.
                            HandViewerController.Instance.ShowPlayerHand(response.SCards);

                            List<CardController> pTrainers = null;
                            List<CardController> opTrainers = null;

                            // We make sure it is real.
                            if (p.IsRealPlayer)
                            {
                                // We get the player trainers.
                                pTrainers = p.PlayerHand.Where(x => x.CardData.MetaData.CardTypeId == CardTypes.Trainer)
                                .ToList();

                                // Opponent trainer cards.
                                opTrainers = response.SCards.Where(x => x.MetaData.CardTypeId == CardTypes.Trainer)
                                .Select(x => op.GetCardInHand(x.UniqueCardID))
                                .Where(x => x != null)
                                .ToList();
                            }
                            else
                            {
                                // We get the player trainers.
                                opTrainers = op.PlayerHand.Where(x => x.CardData.MetaData.CardTypeId == CardTypes.Trainer)
                                .ToList();

                                // Opponent trainer cards.
                                pTrainers = response.SCards.Where(x => x.MetaData.CardTypeId == CardTypes.Trainer)
                                .Select(x => p.GetCardInHand(x.UniqueCardID))
                                .Where(x => x != null)
                                .ToList();
                            }

                            // We discard all the player trainers.
                            p.StartCoroutine(p.MoveFromHandToDeckMultiple(pTrainers, () =>
                            {
                                // We prevent multiple discard of trainer card.
                                if (pTrainers.Count >= opTrainers.Count)
                                {
                                    // When arrived to destination.
                                    p.AddToDiscard(p.PlayerTrainer);

                                    // We clear the player trainer.
                                    p.PlayerTrainer = null;
                                }
                            }));

                            // We discard opponent hands todeck.
                            op.StartCoroutine(op.MoveFromHandToDeckMultiple(opTrainers, () =>
                            {
                                // We prevent multiple discard of trainer card.
                                if (opTrainers.Count > pTrainers.Count)
                                {
                                    // When arrived to destination.
                                    p.AddToDiscard(p.PlayerTrainer);

                                    // We clear the player trainer.
                                    p.PlayerTrainer = null;
                                }
                            }));

                            // We return callback.
                            if (onSimulationCompleted != null)
                                onSimulationCompleted.Invoke();
                        }

                        // if card is in player hand.
                        if (cardInHand != null)
                            p.MoveFromHandToTrainer(cardInHand, () => actionToExecute());
                        else
                            p.PlayerTrainer.SetArriveAction(() => actionToExecute());
                    }
                    break;
            }
        }

    }
}
