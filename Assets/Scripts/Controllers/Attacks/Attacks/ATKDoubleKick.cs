using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKDoubleKick : BaseATK
    {
        public override void Attack(int actionNumber = 0)
        {
            // Send to request to server.
            if (actionNumber == 0)
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

                        // When hit action completed. We flip the coin.
                        p.PlayerCoin.FlipCoin(() =>
                        {
                            // We maker sure damage hit.
                            if (response.Flips.Contains(Flips.Heads))
                            {
                                p.HitToTarget(p.PlayerActive, op.PlayerActive, response.Damage, () =>
                                {
                                    // We tell action is completed.
                                    if (onSimulationCompleted != null)
                                        onSimulationCompleted.Invoke();
                                });
                            }
                            else
                            {
                                // We tell action is completed.
                                if (onSimulationCompleted != null)
                                    onSimulationCompleted.Invoke();
                            }
                        }, response.Flips);
                    }
                    break;
            }
        }

    }
}
