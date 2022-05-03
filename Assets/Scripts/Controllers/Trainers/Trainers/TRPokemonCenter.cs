using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRPokemonCenter : BaseTrainers
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
                        {
                            p.MoveFromHandToTrainer(cardInHand, () =>
                            {
                                // We recover active monster health.
                                p.PlayerActive.CardDamage.RecoverFullHealth();

                                // We recover benched monsters health.
                                p.PlayerBenched.ForEach(e => e.CardDamage.RecoverFullHealth());

                                // We detach all the energies to active monster.
                                var activeEnergies = p.PlayerActive.CardEnergyAttachment.AttachedEnergies.Select(x => x.Item2).ToList();

                                // We detach all the benched monsters energies.
                                var benchedEnergies = p.PlayerBenched.SelectMany(x => x.CardEnergyAttachment.AttachedEnergies).Select(x => x.Item2).ToList();

                                // We combine all of them.
                                benchedEnergies.AddRange(activeEnergies);

                                StartCoroutine(p.AddToDiscardMultiple(benchedEnergies, () =>
                                 {
                                      // When arrived to destination.
                                      p.AddToDiscard(p.PlayerTrainer);

                                      // We clear the player trainer.
                                      p.PlayerTrainer = null;
                                 }));

                                // We detach all the energy cards.
                                p.PlayerActive.CardEnergyAttachment.DetachAll();

                                // We also detach all the energies from 
                                p.PlayerBenched.ForEach(e => e.CardEnergyAttachment.DetachAll());

                                // We return callback.
                                if (onSimulationCompleted != null)
                                    onSimulationCompleted.Invoke();
                            });
                        }
                        else
                        {
                            p.PlayerTrainer.SetArriveAction(() =>
                            {
                                // We recover active monster health.
                                p.PlayerActive.CardDamage.RecoverFullHealth();

                                // We recover benched monsters health.
                                p.PlayerBenched.ForEach(e => e.CardDamage.RecoverFullHealth());

                                // We detach all the energies to active monster.
                                var activeEnergies = p.PlayerActive.CardEnergyAttachment.AttachedEnergies.Select(x => x.Item2).ToList();

                                // We detach all the benched monsters energies.
                                var benchedEnergies = p.PlayerBenched.SelectMany(x => x.CardEnergyAttachment.AttachedEnergies).Select(x => x.Item2).ToList();

                                // We combine all of them.
                                benchedEnergies.AddRange(activeEnergies);

                                StartCoroutine(p.AddToDiscardMultiple(benchedEnergies, () =>
                                {
                                    // When arrived to destination.
                                    p.AddToDiscard(p.PlayerTrainer);

                                    // We clear the player trainer.
                                    p.PlayerTrainer = null;

                                }));

                                // We detach all the energy cards.
                                p.PlayerActive.CardEnergyAttachment.DetachAll();

                                // We also detach all the energies from 
                                p.PlayerBenched.ForEach(e => e.CardEnergyAttachment.DetachAll());

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
