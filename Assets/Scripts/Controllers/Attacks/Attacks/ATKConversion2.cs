using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.GSSocket.DTO;
using System;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKConversion2 : BaseATK
    {
        public void ManualAttack(EnergyTypes energy)
        {
            // We set the energy.
            base.RequestModel.TEnergy = energy;

            // We send to server.
            BattleGameController.Instance.SendGameAction(BattleGameActions.AttackToOpponent, base.RequestModel);
        }

        public override void Simulate(BGAttackResponseDTO response, Action onSimulationCompleted)
        {
            switch (response.ActionNumber)
            {
                case 1:
                    {
                        // We get attacker player.
                        PlaygroundController p = BattleGameController.Instance.GetPlaygroundByPlayer(response.APlayer);

                        // We get opponent of player.
                        PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(p);

                        // We update the energy.
                        p.PlayerActive.SetResistance(response.TEnergy);

                        // We tell action is completed.
                        if (onSimulationCompleted != null)
                            onSimulationCompleted.Invoke();
                    }
                    break;
            }
        }
    }
}
