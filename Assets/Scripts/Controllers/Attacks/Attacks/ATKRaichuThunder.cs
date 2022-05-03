using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKRaichuThunder : BaseATK
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

                        // We hit to target.
                        StartCoroutine(p.HitToTarget(p.PlayerActive, op.PlayerActive, response.Damage, () =>
                        {
                            // When hit action completed. We flip the coin.
                            p.PlayerCoin.FlipCoin(() =>
                            {
                                // if tails attack failed.
                                if (response.Flips[0] == Flips.Tails)
                                    p.PlayerActive.CardDamage.HitDamage(p.PlayerActive, response.SelfDamage);

                                // We tell action is completed.
                                if (onSimulationCompleted != null)
                                    onSimulationCompleted.Invoke();
                            }, response.Flips);
                        }));
                    }
                    break;
            }
        }

    }
}
