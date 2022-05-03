using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using System;

namespace Assets.Scripts.Controllers.Abilities.Abilities
{
    public class ABIEnergyBurn : BaseAbilities
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
                case 1:
                    {
                        // We set as played.
                        base.PlayedCard.PerTurnData = true;

                        // We convert all the energies.
                        base.PlayedCard.CardEnergyAttachment.AttachedEnergies.ForEach(e => e.Item1.LoadTempEnergy(EnergyTypes.Fire));

                        // We complete the simulation.
                        onSimulationCompleted.Invoke();
                    }
                    break;
            }
        }
    }
}
