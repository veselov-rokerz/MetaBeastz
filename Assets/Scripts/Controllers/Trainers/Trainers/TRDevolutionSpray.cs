using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Controllers.Trainers.Trainers
{
    public class TRDevolutionSpray : BaseTrainers
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
                            p.MoveFromHandToTrainer(cardInHand);

                        // if real player it has to select a monster.
                        if (p.IsRealPlayer)
                            BattleNotiController.Instance.GOSelectAMonster.SetActive(true);
                    }
                    break;
                case 2:
                    {
                        // We make sure the trainer card is closed.
                        BattleNotiController.Instance.GOSelectAMonster.SetActive(false);

                        // if this card is active.
                        if (p.IsCardActive(response.TCardID))
                        {
                            // We store the played card.
                            CardController originalCard = p.PlayerActive;

                            // Searched card.
                            CardController searchedCard = originalCard;

                            // We will transfer attached energies to lower stage.
                            var attachedEnergies = searchedCard.CardEnergyAttachment.AttachedEnergies.Select(x => x.Item2).ToArray();

                            // We clear the attached energies.
                            searchedCard.CardEnergyAttachment.AttachedEnergies.Clear();

                            // Cards to discard.
                            List<CardController> cards = new List<CardController>();

                            // We will loop all the evolution cards.
                            while (searchedCard != null)
                            {
                                // We search is it basic.
                                if (searchedCard.CardData.MetaData.IsBasic)
                                {
                                    // We pass to next search card.
                                    if (originalCard != searchedCard)
                                    {
                                        // We pass to next search card.
                                        p.AddToActive(searchedCard, () =>
                                        {
                                            // We transfer attached energies.
                                            foreach (CardController energy in attachedEnergies)
                                                p.PlayerActive.CardEnergyAttachment.AttachAnEnergyToMonsterCard(energy);
                                        });

                                        // We transfer the taken damage.
                                        searchedCard.CardDamage.SetDamage(originalCard.CardDamage.TakenDamage);

                                        // We clear special condition effect.
                                        searchedCard.SetSpecialCondition(BGSpecialConditions.None);
                                    }
                                    // We break the loop.
                                    break;
                                }

                                // We send to discard pile.
                                cards.Add(searchedCard);

                                // We go to next one.
                                searchedCard = searchedCard.AttachedLowStage;
                            }

                            // We discard evolution cards.
                            p.StartCoroutine(p.AddToDiscardMultiple(cards, () =>
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
                        else if (p.IsCardInBench(response.TCardID)) // We make sure the card is in bench.
                        {
                            // We store the played card.
                            CardController originalCard = p.GetCardInBench(response.TCardID);

                            // Searched card.
                            CardController searchedCard = originalCard;

                            // We will transfer attached energies to lower stage.
                            var attachedEnergies = searchedCard.CardEnergyAttachment.AttachedEnergies.Select(x => x.Item2).ToArray();

                            // We clear the attached energies.
                            searchedCard.CardEnergyAttachment.AttachedEnergies.Clear();

                            // Cards to discard.
                            List<CardController> cards = new List<CardController>();

                            // We will loop all the evolution cards.
                            while (searchedCard != null)
                            {
                                // We search is it basic.
                                if (searchedCard.CardData.MetaData.IsBasic)
                                {
                                    // We pass to next search card.
                                    if (originalCard != searchedCard)
                                    {
                                        // We replace benched monster with new one.
                                        p.SwitchBenchWithNewOne(originalCard, searchedCard, () =>
                                         {
                                             // We transfer attached energies.
                                             foreach (CardController energy in attachedEnergies)
                                                 p.PlayerActive.CardEnergyAttachment.AttachAnEnergyToMonsterCard(energy);
                                         });

                                        // We transfer the taken damage.
                                        searchedCard.CardDamage.SetDamage(originalCard.CardDamage.TakenDamage);

                                        // We clear special condition effect.
                                        searchedCard.SetSpecialCondition(BGSpecialConditions.None);
                                    }

                                    // We break the loop.
                                    break;
                                }

                                // We send to discard pile.
                                cards.Add(searchedCard);

                                // We go to next one.
                                searchedCard = searchedCard.AttachedLowStage;
                            }

                            // We discard evolution cards.
                            p.StartCoroutine(p.AddToDiscardMultiple(cards, () =>
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
