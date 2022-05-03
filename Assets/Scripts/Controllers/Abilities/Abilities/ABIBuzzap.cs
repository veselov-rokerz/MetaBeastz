using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Abilities.Abilities
{
    public class ABIBuzzap : BaseAbilities
    {
        public override void Play(bool isFirstTime)
        {
            // Send to request to server.
            if (isFirstTime)
                BattleGameController.Instance.SendGameAction(BattleGameActions.PlayAbility, RequestModel);
            else
                BattleGameController.Instance.SendGameAction(BattleGameActions.PlayAbilityEffect, RequestModel);
        }

        public override void Simulate(BGAbilityResponseDTO response, Action onSimulationCompleted)
        {
            // We have to override action.
            base.Simulate(response, onSimulationCompleted);

            // We get playground of player.
            PlaygroundController p = BattleGameController.Instance.GetPlaygroundByPlayer(response.RequestData.Player);

            // We get playground of opponent player.
            PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(p);

            switch (response.ActionNumber)
            {
                case 2:
                    {
                        // We store the cards.
                        CardController firstCard = null, secondCard = null;

                        // We find the first card.
                        if (p.IsCardActive(response.RequestData.CardID))
                            firstCard = p.PlayerActive;
                        else if (p.IsCardInBench(response.RequestData.CardID))
                            firstCard = p.GetCardInBench(response.RequestData.CardID);

                        // We find the first card.
                        if (p.IsCardActive(response.RequestData.TCardId))
                            secondCard = p.PlayerActive;
                        else if (p.IsCardInBench(response.RequestData.TCardId))
                            secondCard = p.GetCardInBench(response.RequestData.TCardId);

                        // We knockout the monster.
                        p.StartCoroutine(p.ToKnockout(firstCard, () =>
                        {
                            // We remove the card from discard pile.
                            p.RemoveFromDiscard(firstCard);

                            // We change its energy.
                            firstCard.TempEnergy = response.RequestData.Energy;

                            // We attach card to another.
                            p.AddAttachmentToCard(firstCard, secondCard);

                            /// We increase opponent prize view.
                            op.WaitingPrizeQuantity++;

                            // We complete the simulation.
                            onSimulationCompleted.Invoke();
                        }));
                    }
                    break;
            }
        }

    }
}
