using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKThunderbolt : BaseATK
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
                            // We get the detached energies.
                            List<CardController> detachedEnergies = p.PlayerActive.CardEnergyAttachment
                            .AttachedEnergies.Select(x => x.Item1.EnergyCard).ToList();

                            // We detach all the cards.
                            p.PlayerActive.CardEnergyAttachment.DetachAll();

                            // We detach all the energies
                            p.StartCoroutine(p.AddToDiscardMultiple(detachedEnergies));

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
