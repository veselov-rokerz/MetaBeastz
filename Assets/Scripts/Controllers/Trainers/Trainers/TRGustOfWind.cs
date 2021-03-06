using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRGustOfWind : BaseTrainers
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
                                BattleNotiController.Instance.GOSelectOpponentBenchMonster.SetActive(true);
                            });
                        }
                    }
                    break;
                case 2:
                    {
                        // We close the notifications.
                        BattleNotiController.Instance.GOSelectOpponentBenchMonster.SetActive(false);

                        // We get opponent of player.
                        PlaygroundController op = BattleGameController.Instance.GetCurrentPlayerOpponentPlayground();

                        // We get the card in bench.
                        CardController cardInBench = op.GetCardInBench(response.TCardID);

                        // We switch the active and bench.
                        op.SwitchBetweenActiveToBench(op.PlayerActive, cardInBench, () =>
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
