using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKLure : BaseATK
    {
        /// <summary>
        /// We will use to track the target card.
        /// </summary>
        public BGAttackRequestDTO RequestModel { get; set; }

        public override void Attack(int actionNumber = 0)
        {
            // We get a request model.
            RequestModel = base.RequestModel;

            // We wait until a selection.
            StartCoroutine(WaitForTargetSelection(actionNumber));
        }

        private IEnumerator WaitForTargetSelection(int actionNumber)
        {
            // We close the action view.
            base.CardData.Playground.DeactivateAction();

            // We ask for select an opponent benched monster.
            BattleNotiController.Instance.GOSelectOpponentBenchMonster.SetActive(true);

            // We wait until a card selected.
            yield return new WaitUntil(() => this.RequestModel.TargetCardId > 0);

            // We close the view.
            BattleNotiController.Instance.GOSelectOpponentBenchMonster.SetActive(false);

            // Send to request to server.
            if (actionNumber == 0)
                BattleGameController.Instance.SendGameAction(BattleGameActions.AttackToOpponent, this.RequestModel);
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
                            // We get the card to switch it..
                            CardController cardInBench = op.GetCardInBench(response.TargetCID);

                            // We switch active with in bench.
                            op.SwitchBetweenActiveToBench(op.PlayerActive, cardInBench);

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
