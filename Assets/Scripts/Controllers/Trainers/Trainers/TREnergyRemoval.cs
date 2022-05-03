using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TREnergyRemoval : BaseTrainers
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
                            BattleNotiController.Instance.GOSelectYourOpponentMonster.SetActive(true);
                    }
                    break;
                case 2:
                    {
                        // We close the energy detach view.
                        CardEnergyDetachController.Instance.RollbackAllDetachedEnergiesAndClose();

                        // We get the opponent active.
                        if (op.IsCardActive(response.TCardID))
                        {
                            // We get the detached psychic to detach it..
                            var detachedEnergys = op.PlayerActive.CardEnergyAttachment.AttachedEnergies
                            .Where(x => response.TCardIDs.Contains(x.Item2.CardData.UniqueCardID))
                            .ToList();

                            // We are going to remove all the required retreat cost.
                            detachedEnergys.ForEach(e => op.PlayerActive.CardEnergyAttachment.DetachCard(e.Item1));

                            // We detach all the items.
                            StartCoroutine(op.AddToDiscardMultiple(detachedEnergys.Select(x => x.Item2).ToList(), () =>
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
                        else if (op.IsCardInBench(response.TCardID)) // Is card is in bench?
                        {
                            // We get the card in bench.
                            CardController cardInBench = op.GetCardInBench(response.TCardID);

                            // We get the detached psychic to detach it..
                            var detachedEnergys = cardInBench.CardEnergyAttachment.AttachedEnergies
                            .Where(x => response.TCardIDs.Contains(x.Item2.CardData.UniqueCardID))
                            .ToList();

                            // We are going to remove all the required retreat cost.
                            detachedEnergys.ForEach(e => cardInBench.CardEnergyAttachment.DetachCard(e.Item1));

                            // We detach all the items.
                            StartCoroutine(op.AddToDiscardMultiple(detachedEnergys.Select(x => x.Item2).ToList(), () =>
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
                    }
                    break;
            }
        }

    }
}
