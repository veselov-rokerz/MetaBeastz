using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKBarrier : BaseATK
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
                            // We get the detached psychic to detach it..
                            var de = p.PlayerActive.CardEnergyAttachment.AttachedEnergies
                            .FirstOrDefault(x => x.Item2.CardData.UniqueCardID == response.DACards[0]);

                            // We detach all the cards.
                            p.PlayerActive.CardEnergyAttachment.DetachCard(de.Item1);

                            // We detach all the energies
                            p.AddToDiscard(de.Item2);

                            // We give the shield.
                            p.PlayerActive.SetShield(response.Shield);

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
