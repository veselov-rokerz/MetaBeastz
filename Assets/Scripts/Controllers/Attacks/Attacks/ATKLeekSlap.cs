using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKLeekSlap : BaseATK
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

                        // We will prevent this ability use again.
                        p.PlayerActive.AttackDataPersistInPlay = true;

                        // We hit to target.
                        p.PlayerCoin.FlipCoin(() =>
                        {
                            if (response.Flips[0] == Flips.Heads)
                            {
                                StartCoroutine(p.HitToTarget(p.PlayerActive, op.PlayerActive, response.Damage, () =>
                                {
                                    // We tell action is completed.
                                    if (onSimulationCompleted != null)
                                        onSimulationCompleted.Invoke();
                                }));
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
