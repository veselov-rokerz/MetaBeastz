using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRScoopUp : BaseTrainers
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
                            BattleNotiController.Instance.GOSelectAMonster.SetActive(true);
                        else
                            BattleNotiController.Instance.GOYourOpSelectingAMonster.SetActive(true);
                    }
                    break;
                case 2:
                    {
                        // We make sure the trainer card is closed.
                        BattleNotiController.Instance.GOSelectAMonster.SetActive(false);
                        BattleNotiController.Instance.GOYourOpSelectingAMonster.SetActive(false);

                        // We move active card to hand.
                        StartCoroutine(ActionCorouitine(response, p, onSimulationCompleted));
                    }
                    break;
            }
        }

        public IEnumerator ActionCorouitine(BGTrainerResponsePlayerDTO response, PlaygroundController p, Action onSimulationCompleted)
        {
            // if card is active.
            if (p.IsCardActive(response.TCardID))
            {
                // We get the card.
                CardController activeCard = p.PlayerActive;

                // We loop all the cards.
                while (activeCard != null)
                {
                    // We get the attached energies.
                    var attachedEnergies = activeCard.CardEnergyAttachment.AttachedEnergies.ToList();

                    // We tell we detach all the energies.
                    activeCard.CardEnergyAttachment.DetachAll();

                    // We discard all the cards.
                    yield return p.StartCoroutine(p.AddToDiscardMultiple(attachedEnergies.Select(x => x.Item2).ToList(), null));

                    // We wait some seconds.
                    yield return new WaitForSeconds(.3f);

                    // if the next card is null means, it is basic monster.
                    if (activeCard.AttachedLowStage == null)
                    {
                        // We add card to hand.
                        p.AddToHand(activeCard);

                        // We also clear the active player monster.
                        p.PlayerActive = null;
                    }
                    else
                    {
                        // We add card to discard.
                        p.AddToDiscard(activeCard);
                    }

                    // We wait some seconds.
                    yield return new WaitForSeconds(.3f);

                    // We search for the next.
                    activeCard = activeCard.AttachedLowStage;
                }

            }
            else if (p.IsCardInBench(response.TCardID))
            {
                // We get the card.
                CardController benchedCard = p.GetCardInBench(response.TCardID);

                // We loop all the cards.
                while (benchedCard != null)
                {
                    // We get the attached energies.
                    var attachedEnergies = benchedCard.CardEnergyAttachment.AttachedEnergies.ToList();

                    // We tell we detach all the energies.
                    benchedCard.CardEnergyAttachment.DetachAll();

                    // We discard all the cards.
                    yield return p.StartCoroutine(p.AddToDiscardMultiple(attachedEnergies.Select(x => x.Item2).ToList(), null));

                    // We wait some seconds.
                    yield return new WaitForSeconds(.3f);

                    // We also clear the attached cards.
                    benchedCard.SetAttachATrainerCard(null);

                    // We reset card states.
                    benchedCard.ResetState();

                    // if the next card is null means, it is basic monster.
                    if (benchedCard.AttachedLowStage == null)
                    {
                        // We add the hand from bench.
                        p.AddToHand(benchedCard);

                        // We also remove from the bench.
                        p.PlayerBenched.Remove(benchedCard);
                    }
                    else
                    {
                        // We throw the discard pile.
                        p.AddToDiscard(benchedCard);
                    }

                    // We wait some seconds.
                    yield return new WaitForSeconds(.3f);

                    // We search for the next.
                    benchedCard = benchedCard.AttachedLowStage;
                }
            }

            // We wait some seconds.
            yield return new WaitForSeconds(.3f);

            // When arrived to destination.
            p.AddToDiscard(p.PlayerTrainer);

            // We clear the player trainer.
            p.PlayerTrainer = null;

            // We clear the active player.
            p.PlayerActive = null;

            // We force to select an active monster.
            if (p.IsRealPlayer)
                BattleNotiController.Instance.GOForceToSelectActiveCard.SetActive(true);
            else
                BattleNotiController.Instance.GOYourOpSelectingAMonster.SetActive(true);

            // We return callback.
            if (onSimulationCompleted != null)
                onSimulationCompleted.Invoke();
        }

    }
}
