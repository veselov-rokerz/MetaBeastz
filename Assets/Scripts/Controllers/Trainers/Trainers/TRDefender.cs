using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRDefender : BaseTrainers
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
                        }else
                        {
                            // We ask for select a monster.
                            BattleNotiController.Instance.GOSelectAMonster.SetActive(true);
                        }
                    }
                    break;
                case 2:
                    {
                        // We ask for select a monster.
                        BattleNotiController.Instance.GOSelectAMonster.SetActive(false);

                        // We attach trainer card to hand.
                        if (p.IsCardActive(response.TCardID))
                            p.PlayerActive.SetAttachATrainerCard(base.PlayedCard);
                        else if (p.IsCardInBench(response.TCardID))
                            p.GetCardInBench(response.TCardID).SetAttachATrainerCard(base.PlayedCard);

                        // When arrived to destination.
                        p.AddToDiscard(p.PlayerTrainer);

                        // We clear the player trainer.
                        p.PlayerTrainer = null;

                        // We callback.
                        if (onSimulationCompleted != null)
                            onSimulationCompleted.Invoke();
                    }
                    break;
            }
        }

    }
}
