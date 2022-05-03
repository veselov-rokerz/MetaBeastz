using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Abilities.Abilities
{
    public class ABIDamageSwap : BaseAbilities
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

            switch (response.ActionNumber)
            {
                case 2:
                    {
                        // We store the cards.
                        CardController firstCard = null, secondCard = null;

                        // We find the first card.
                        if (p.IsCardActive(response.RequestData.TCardId))
                            firstCard = p.PlayerActive;
                        else if (p.IsCardInBench(response.RequestData.TCardId))
                            firstCard = p.GetCardInBench(response.RequestData.TCardId);

                        // We find the first card.
                        if (p.IsCardActive(response.RequestData.SCardId))
                            secondCard = p.PlayerActive;
                        else if (p.IsCardInBench(response.RequestData.SCardId))
                            secondCard = p.GetCardInBench(response.RequestData.SCardId);

                        // We recover first card.
                        firstCard.CardDamage.RecoverHealth(response.RequestData.TValue);

                        // We hit the target.
                        secondCard.CardDamage.HitDamage(p.PlayerActive, response.RequestData.TValue);

                        // if still exists that can be transffered hp.
                        if (firstCard.GetCurrentHealth != firstCard.CardData.MetaData.CardHp)
                        {
                            if (p.IsRealPlayer)
                            {
                                // We clear the second card.
                                base.RequestModel.SCardId = 0;

                                // We reactive to selection.
                                BattleNotiController.Instance.GOSelectAnotherMonster.SetActive(true);
                            }else
                            {
                                onSimulationCompleted.Invoke();
                            }
                        }
                        else
                            onSimulationCompleted.Invoke();
                    }
                    break;
            }
        }

    }
}
