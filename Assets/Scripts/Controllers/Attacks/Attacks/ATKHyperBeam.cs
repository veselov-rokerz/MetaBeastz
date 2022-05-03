using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKHyperBeam : BaseATK
    {
        public override void Attack(int actionNumber = 0)
        {
        }

        public void AttackManual(int[] detachedIds)
        {
            // We store the detached card ids.
            base.RequestModel.DCardIds = detachedIds;

            // Send to request to server.
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

                        // We get the player active.
                        CardController opActive = op.PlayerActive;

                        // We hit to target.
                        StartCoroutine(p.HitToTarget(p.PlayerActive, opActive, response.Damage, () =>
                        {
                            // We will detach the opponent card.
                            if (response.DDCards.Length > 0)
                                opActive.CardEnergyAttachment.DetachCard(response.DDCards[0]);

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
