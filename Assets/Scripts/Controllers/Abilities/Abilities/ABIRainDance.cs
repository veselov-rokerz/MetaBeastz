using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Abilities.Abilities
{
    public class ABIRainDance : BaseAbilities
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

            switch (response.ActionNumber)
            {
                case 2:
                    {
                    }
                    break;
            }
        }

    }
}
