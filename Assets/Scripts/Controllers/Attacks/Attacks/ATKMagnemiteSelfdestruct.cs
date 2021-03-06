using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKMagnemiteSelfdestruct : BaseATK
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
                            // Player also hit itself.
                            p.PlayerActive.CardDamage.HitDamage(p.PlayerActive, response.SelfDamage);

                            // We hit all the players benched monsters.
                            p.PlayerBenched.ToList().ForEach(e => e.CardDamage.HitDamage(p.PlayerActive, response.BenchDamage));

                            // We hit all the players benched monsters.
                            op.PlayerBenched.ToList().ForEach(e => e.CardDamage.HitDamage(p.PlayerActive, response.BenchDamage));

                            // We tell action is completed.
                            if (onSimulationCompleted != null)
                                onSimulationCompleted.Invoke();
                        }));

                    }
                    break;
            }
        }
    }
}
