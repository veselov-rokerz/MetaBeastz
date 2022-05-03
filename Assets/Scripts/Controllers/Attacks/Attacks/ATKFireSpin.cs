using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Linq;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKFireSpin : BaseATK
    {

        public void AttackManual(int[] detachCardIds)
        {
            // We also send the detached card ids.
            base.RequestModel.DCardIds = detachCardIds;

            // Send to server.
            BattleGameController.Instance.SendGameAction(BattleGameActions.AttackToOpponent, base.RequestModel);
        }

        public override void Simulate(BGAttackResponseDTO response, Action onSimulationCompleted)
        {
            switch (response.ActionNumber)
            {
                case 1:
                    {
                        // We can close and detach all the releated items.
                        CardEnergyDetachController.Instance.RollbackAllDetachedEnergiesAndClose();

                        // We get attacker player.
                        PlaygroundController p = BattleGameController.Instance.GetPlaygroundByPlayer(response.APlayer);

                        // We get opponent of player.
                        PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(p);

                        // We hit to target.
                        StartCoroutine(p.HitToTarget(p.PlayerActive, op.PlayerActive, response.Damage, () =>
                        {
                            // We get the detached psychic to detach it..
                            var detachedEnergys = p.PlayerActive.CardEnergyAttachment.AttachedEnergies
                            .Where(x => response.DACards.Contains(x.Item2.CardData.UniqueCardID))
                            .ToList();

                            // We are going to remove all the required retreat cost.
                            detachedEnergys.ForEach(e => p.PlayerActive.CardEnergyAttachment.DetachCard(e.Item1));

                            // We detach all the items.
                            StartCoroutine(p.AddToDiscardMultiple(detachedEnergys.Select(x => x.Item2).ToList()));

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
