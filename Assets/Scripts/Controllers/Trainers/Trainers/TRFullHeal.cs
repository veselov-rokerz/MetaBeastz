using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRFullHeal : BaseTrainers
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

                        // if card is in player hand.
                        if (cardInHand != null)
                        {
                            p.MoveFromHandToTrainer(cardInHand, () =>
                            {
                                // We remove the special condition.
                                p.PlayerActive.CardDamage.RemoveSpecialCondition();

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
                            // if trainer we will just call the back.
                            p.PlayerTrainer.SetArriveAction(() =>
                            {
                                // We remove the special condition.
                                p.PlayerActive.CardDamage.RemoveSpecialCondition();

                                // When arrived to destination.
                                p.AddToDiscard(p.PlayerTrainer);

                                // We clear the player trainer.
                                p.PlayerTrainer = null;

                                // We return callback.
                                if (onSimulationCompleted != null)
                                    onSimulationCompleted.Invoke();
                            });
                        }
                    }
                    break;
            }
        }

    }
}
