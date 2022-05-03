using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;

namespace Assets.Scripts.Controllers.Attacks
{
    public class ATKWhirlwind : BaseATK
    {
        /// <summary>
        /// We store the last response.
        /// </summary>
        public BGAttackResponseDTO LastResponse { get; set; }

        public override void Attack(int actionNumber = 0)
        {
            // if last response is null then we will do attack.
            if (LastResponse == null)
                BattleGameController.Instance.SendGameAction(BattleGameActions.AttackToOpponent, RequestModel);
            else // Means we make effect.
                BattleGameController.Instance.SendGameAction(BattleGameActions.AttackToOpponentEffect, RequestModel);
        }

        public override void Simulate(BGAttackResponseDTO response, Action onSimulationCompleted)
        {
            // We store the last response.
            this.LastResponse = response;

            // We get attacker player.
            PlaygroundController p = BattleGameController.Instance.GetPlaygroundByPlayer(response.APlayer);

            // We get opponent of player.
            PlaygroundController op = BattleGameController.Instance.GetOpponentOfPlayground(p);

            // We do the action depends on number.
            switch (response.ActionNumber)
            {
                case 1:
                    {
                        // We get the opponent active monster.
                        CardController opActive = op.PlayerActive;

                        // We hit to target.
                        StartCoroutine(p.HitToTarget(p.PlayerActive, opActive, response.Damage, () =>
                        {
                            // if opponent active monster is not dead. We will force to swap it.
                            if (!opActive.CardDamage.IsDeath && op.PlayerBenched.Count > 0 && opActive.Shield != BGShieldTypes.AllDamageAndEffects)
                            {
                                // After hit we will force the opponent select a new active monster.
                                if (op.IsRealPlayer)
                                    BattleNotiController.Instance.GOSelectABenchedAMonster.SetActive(true);
                                else // if opponent we will tell opponent will select monster.
                                    BattleNotiController.Instance.GOYourOpSelectingAMonster.SetActive(true);
                            }
                            else
                            {
                                // We tell action is completed.
                                if (onSimulationCompleted != null)
                                    onSimulationCompleted.Invoke();
                            }
                        }));
                    }
                    break;
                case 2:
                    {
                        // We get the card in bench.
                        CardController cardInBench = op.GetCardInBench(response.TargetCID);

                        // Select a monster text closed.
                        BattleNotiController.Instance.GOSelectABenchedAMonster.SetActive(false);

                        // We close the selecting view.
                        BattleNotiController.Instance.GOYourOpSelectingAMonster.SetActive(false);

                        // We switch the active and bench.
                        op.SwitchBetweenActiveToBench(op.PlayerActive, cardInBench);

                        // We tell action is completed.
                        if (onSimulationCompleted != null)
                            onSimulationCompleted.Invoke();
                    }
                    break;
            }
        }
    }
}
