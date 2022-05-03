using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKAmnesia : BaseATK
    {
        public override void Attack(int actionNumber = 0)
        {
        }

        public void ManualAttack(int attackId)
        {
            // We set the selected attack.
            base.RequestModel.TAttackID = attackId;

            // We send to server.
            BattleGameController.Instance.SendGameAction(BattleGameActions.AttackToOpponent, base.RequestModel);
        }

        public override void Simulate(BGAttackResponseDTO response, Action onSimulationCompleted)
        {
            switch (response.ActionNumber)
            {
                case 1:
                    {
                        // We close the select your opponent attack view.
                        BattleNotiController.Instance.GOSelectYourOpponentAttack.SetActive(false);

                        // We get attacker player.
                        PlaygroundController p = BattleGameController.Instance.GetPlaygroundByPlayer(response.APlayer);

                        // We get opponent of player.
                        PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(p);

                        // We hit to target.
                        StartCoroutine(p.HitToTarget(p.PlayerActive, op.PlayerActive, response.Damage, () =>
                        {
                            // We block the attack.
                            op.BlockTheAttack((BGAttacks)response.TAttackID);

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
