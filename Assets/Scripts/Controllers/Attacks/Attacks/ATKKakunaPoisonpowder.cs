using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKKakunaPoisonpowder : BaseATK
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

                        // We store the opponent active monster.
                        CardController opActive = op.PlayerActive;

                        // We hit to target.
                        StartCoroutine(p.HitToTarget(p.PlayerActive, opActive, response.Damage, () =>
                        {
                            // When hit action completed. We flip the coin.
                            p.PlayerCoin.FlipCoin(() =>
                            {
                                // We update monster condition as confused.
                                if (response.TargetSC != BGSpecialConditions.None)
                                    opActive.SetSpecialCondition(response.TargetSC, response.PoDamage);

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
