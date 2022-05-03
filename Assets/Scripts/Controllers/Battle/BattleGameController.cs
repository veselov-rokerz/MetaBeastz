using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.BSSocket.Extends;
using Assets.Scripts.BSSocket.Models;
using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleGameController : MonoBehaviour
{
    public static BattleGameController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Game data when battle start.
    /// </summary>
    public BGGameStartDTO GameStartData { get; private set; }

    /// <summary>
    /// if game started true.
    /// </summary>
    public bool IsGameStarted { get; private set; }

    /// <summary>
    /// Only one times an energy can be attached in a turn.
    /// </summary>
    public bool IsAnEnergyAttached { get; set; }

    /// <summary>
    /// Current player state.
    /// </summary>
    public BattleGamePlayers CurrentPlayer { get; set; }

    /// <summary>
    /// True when player turn.
    /// </summary>
    public bool IsPlayerTurn => CurrentPlayer == GameStartData.PColor;

    /// <summary>
    /// When was the first round it should be smaller than 0.
    /// </summary>
    public bool IsFirstRound => CurrentRaund <= 0;

    /// <summary>
    /// Current round counter.
    /// </summary>
    public int CurrentRaund { get; private set; }

    /// <summary>
    /// When the game is over just return.
    /// </summary>
    public bool IsGameOver { get; private set; }

    [Header("When your turn it is going to be activated.")]
    public GameObject GOYourTurn;

    [Header("When opponent turn it is going to be activated.")]
    public GameObject GOOpponentTurn;

    [Header("Player playground.")]
    public PlaygroundController Player;

    [Header("Opponent playground.")]
    public PlaygroundController Opponent;

    [Header("We will store which card is currently selected.")]
    public List<CardController> SelectedCards;

    private IEnumerator Start()
    {
        // When we receive a response we call.
        BattleServerController.Instance.OnUnExpectedDataReceived += new Action<BSSocketResponse>((response) =>
        {
            if (response.UnExpectedMethods == BSUnExpectedMethods.GameServer)
            {
                BattleResponseDataDTO responseData = response.GetData<BattleResponseDataDTO>();
                OnResponseReceived(responseData);
            }
        });

        // We wait until connect.
        yield return new WaitUntil(() => BattleServerController.Instance.IsConnected);

        // We connect to server.
        SendGameAction(BattleGameActions.Connect);
    }

    public void SendGameAction(BattleGameActions ba, object data = null)
    {
        // We connect with test data.
        if (SceneDataController.Instance.SoloModeOnly)
        {
            BattleServerController.Instance.SendToServer(BSMethods.BattleData, new BattleRequestDataDTO
            {
                BA = ba,
                UAId = Guid.NewGuid().ToString(),
                UGID = "d39027a7-e868-4465-b318-bb699c513831",
                UUT = SceneDataController.Instance.GetToken(),
                Data = data.ToJson()
            });
        }
        else
        {
            BattleServerController.Instance.SendToServer(BSMethods.BattleData, new BattleRequestDataDTO
            {
                BA = ba,
                UAId = Guid.NewGuid().ToString(),
                UGID = SceneDataController.Instance.BattleServerData.UniqueGameID,
                UUT = SceneDataController.Instance.GetToken(),
                Data = data.ToJson()
            });
        }
    }

    private void OnResponseReceived(BattleResponseDataDTO responseData)
    {
        switch (responseData.BA)
        {
            case BattleGameActions.Connect:
                {
                    // When the game start.
                    this.GameStartData = responseData.Data.ToObject<BGGameStartDTO>();
                    BattleNotiController.Instance.AskForTheHeadsOrTails();
                }
                break;
            case BattleGameActions.FlipCoinHeads:
            case BattleGameActions.FlipCoinTails:
                {
                    // We get the action data.
                    BGGameStartFlipDTO flipData = responseData.Data.ToObject<BGGameStartFlipDTO>();
                    BattleNotiController.Instance.ExecuteFlipCoin(flipData);
                }
                break;
            case BattleGameActions.PlayHandToActive:
                {
                    // We get the play data.
                    BGPlayCardResponseDTO playData = responseData.Data.ToObject<BGPlayCardResponseDTO>();

                    // We find the card in user hand.
                    CardController cardInPlayer = GetPlaygroundByPlayer(playData.Player).GetCardInHand(playData.PlayedCard.UniqueCardID);

                    // if not exists means sender played card.
                    if (cardInPlayer != null)
                    {
                        // We update the card details.
                        cardInPlayer.SetCardData(playData.PlayedCard);

                        // And we play card to active position.
                        GetPlaygroundByPlayer(playData.Player).MoveFromHandToActive(cardInPlayer);
                    }
                    else
                    {
                        // if the game not started we ask for ready button.
                        if (!this.IsGameStarted)
                            BattleNotiController.Instance.ShowDoneButton();
                    }
                }
                break;
            case BattleGameActions.PlayHandToBench:
                {
                    // We get the card information to play from hand to bench.
                    BGPlayCardResponseDTO playData = responseData.Data.ToObject<BGPlayCardResponseDTO>();

                    // We get the card from players hand.
                    CardController cardInPlayer = GetPlaygroundByPlayer(playData.Player).GetCardInHand(playData.PlayedCard.UniqueCardID);

                    // if not exists means it is the player who plays the card.
                    if (cardInPlayer != null)
                    {
                        // We update the card details.
                        cardInPlayer.SetCardData(playData.PlayedCard);

                        // And we play card to active position.
                        GetPlaygroundByPlayer(playData.Player).MoveFromHandToBench(cardInPlayer);
                    }
                }
                break;
            case BattleGameActions.DoneAction:
                {
                    // Current player data.
                    BGSwitchPlayerDTO playData = responseData.Data.ToObject<BGSwitchPlayerDTO>();

                    // We update the round.
                    this.CurrentRaund = playData.Raund;

                    // if older player was asleep then we will flip a coin then we will draw a card.
                    if (playData.Asleep != null && playData.Asleep.Coin != Flips.No)
                    {
                        // We flip a coin for asleep monster.
                        GetPlaygroundByPlayer(playData.Asleep.PColor).PlayerCoin.FlipCoin(() =>
                        {
                            // if heads we will remove the asleep condition.
                            if (playData.Asleep.Coin == Flips.Heads)
                                GetPlaygroundByPlayer(playData.Asleep.PColor).PlayerActive?.SetSpecialCondition(BGSpecialConditions.None);

                            // We update the current player.
                            StartCoroutine(this.SwitchPlayer(playData.Player));

                            // We draw the card.
                            StartCoroutine(BGTurnGameController.Instance.DrawACardWithinSeconds(playData.DrawnCard));

                        }, playData.Asleep.Coin);

                        // We dont have to go more.
                        return;
                    }

                    // We draw the card.
                    if (playData.DrawnCard != null && playData.DrawnCard.Player != BattleGamePlayers.No)
                        StartCoroutine(BGTurnGameController.Instance.DrawACardWithinSeconds(playData.DrawnCard));

                    // We update the current player.
                    StartCoroutine(this.SwitchPlayer(playData.Player));
                }
                break;
            case BattleGameActions.RevealCardsInActiveAndBench:
                {
                    // We set game as started.
                    this.IsGameStarted = true;

                    // Cards to reveals.
                    BGRevealCardDTO cardsToGiveInfo = responseData.Data.ToObject<BGRevealCardDTO>();

                    // We loop the cards to reveal.
                    cardsToGiveInfo.Cards.ForEach(e =>
                    {
                        // We get the card from players hand.
                        CardController cardInBench = GetPlaygroundByPlayer(cardsToGiveInfo.Player).GetCardInBench(e.UniqueCardID);

                        // We make sure card exists.
                        if (cardInBench != null)
                            cardInBench.SetCardData(e);

                        // if the revealed card is players active card.
                        if (GetPlaygroundByPlayer(cardsToGiveInfo.Player).IsCardActive(e.UniqueCardID))
                            GetPlaygroundByPlayer(cardsToGiveInfo.Player).PlayerActive.SetCardData(e);
                    });

                    // We get the first player card manager.
                    PlaygroundController p1CardManager = GetPlaygroundByPlayer(BattleGamePlayers.P1);

                    // We draw prizes.
                    p1CardManager.StartCoroutine(p1CardManager.MoveFromDeckToPrizeMultiple(cardsToGiveInfo.P1Prizes));

                    // We get the second player card manager.
                    PlaygroundController p2CardManager = GetPlaygroundByPlayer(BattleGamePlayers.P2);

                    // We draw prizes.
                    p2CardManager.StartCoroutine(p2CardManager.MoveFromDeckToPrizeMultiple(cardsToGiveInfo.P2Prizes));
                }
                break;
            case BattleGameActions.AttachACardToCard:
                {
                    // Attached informations.
                    BGCardAttachResponseDTO attachData = responseData.Data.ToObject<BGCardAttachResponseDTO>();

                    // We get the card in hand.
                    CardController cardInHand = GetPlaygroundByPlayer(attachData.Player).GetCardInHand(attachData.ECardID);

                    // if this ability is active we clear it.
                    if (GetPlaygroundByPlayer(attachData.Player).IsRealPlayer)
                    {
                        // We check player has energy?
                        bool hasPlayerWaterEnergyInHand = GetPlaygroundByPlayer(attachData.Player).PlayerHand.Any(x => x.CardData.MetaData.CardTypeId == CardTypes.Energy && x.CardData.MetaData.EnergyTypeId == EnergyTypes.Water);

                        // if not we disable it.
                        if (!hasPlayerWaterEnergyInHand)
                            AbilityController.Instance.ClearAbility();
                    }

                    // We make sure card exists.
                    if (cardInHand != null)
                    {
                        // Only can attach to an active card or benched card.
                        cardInHand.SetCardData(attachData.ECardData);

                        // if player attached to active card.
                        if (GetPlaygroundByPlayer(attachData.Player).IsCardActive(attachData.MCardID))
                        {
                            // We get the player active card.
                            CardController activeCard = GetPlaygroundByPlayer(attachData.Player).PlayerActive;

                            // We move the attached card on to the active card.
                            GetPlaygroundByPlayer(attachData.Player).AttachAnEnergyCardFromHand(cardInHand, activeCard);
                        }
                        else // if not active card then it must be benched card.
                        {
                            // We get the card from benched.
                            CardController benchedCard = GetPlaygroundByPlayer(attachData.Player).GetCardInBench(attachData.MCardID);

                            // We move the attached card on to the active card.
                            GetPlaygroundByPlayer(attachData.Player).AttachAnEnergyCardFromHand(cardInHand, benchedCard);
                        }
                    }
                }
                break;
            case BattleGameActions.EvolveAMonster:
                {
                    // We get the evolve data.
                    BGEvolveMonsterResponseDTO evolveData = responseData.Data.ToObject<BGEvolveMonsterResponseDTO>();

                    // We get the evolve card.
                    CardController evolveCard = GetPlaygroundByPlayer(evolveData.Player).GetCardInHand(evolveData.EvUCardID);

                    // We update the card data.
                    if (evolveCard != null)
                        evolveCard.SetCardData(evolveData.EvCardData);

                    // if active monster is evolved.
                    if (GetPlaygroundByPlayer(evolveData.Player).IsCardActive(evolveData.UCardID))
                    {
                        // We evolve the card.
                        GetPlaygroundByPlayer(evolveData.Player).EvolveActiveMonster(evolveCard);
                    }
                    else // if evolve monster is not active monster then it must be evolved.
                    {
                        // We get the benched moster to evolve.
                        CardController benchedMonster = GetPlaygroundByPlayer(evolveData.Player).GetCardInBench(evolveData.UCardID);

                        // Then we evolve it.
                        if (benchedMonster != null)
                            GetPlaygroundByPlayer(evolveData.Player).EvolveBenchedMonster(benchedMonster, evolveCard);
                    }
                }
                break;
            case BattleGameActions.RetreatAMonster:
                {
                    // We get the retreat data.
                    BGRetreatACardResponseDTO retreatData = responseData.Data.ToObject<BGRetreatACardResponseDTO>();

                    // if card still active.
                    if (GetPlaygroundByPlayer(retreatData.Player).IsCardActive(retreatData.ReCardId))
                    {
                        // We get the active card.
                        CardController cardInActive = GetPlaygroundByPlayer(retreatData.Player).PlayerActive;

                        // if confused when retreat just change its condition.
                        if (cardInActive.SpecialCondition == BGSpecialConditions.Confused ||
                            cardInActive.SpecialCondition == BGSpecialConditions.Poisoned)
                            cardInActive.SetSpecialCondition(BGSpecialConditions.None);

                        // if card in bench not found just return.
                        if (GetPlaygroundByPlayer(retreatData.Player).IsCardInBench(retreatData.TargetCardId))
                        {
                            // We get the benched card.
                            CardController cardInBench = GetPlaygroundByPlayer(retreatData.Player).GetCardInBench(retreatData.TargetCardId);

                            // We get the detached cards.
                            var detachedCards = cardInActive.CardEnergyAttachment.AttachedEnergies
                                .Where(y => retreatData.DtCards.Contains(y.Item2.CardData.UniqueCardID))
                                .ToList();

                            // The cards going to discard pile.
                            List<CardController> discardCards = detachedCards.Select(x => x.Item2).ToList();

                            // We are going to remove all the required retreat cost.
                            detachedCards.ForEach(e => cardInActive.CardEnergyAttachment.DetachCard(e.Item1));

                            // We detach all the items.
                            StartCoroutine(GetPlaygroundByPlayer(retreatData.Player).AddToDiscardMultiple(discardCards));

                            // We switch two cards.
                            GetPlaygroundByPlayer(retreatData.Player).SwitchBetweenActiveToBench(cardInActive, cardInBench);

                            // Clefairy will goto discard automatically.
                            if (cardInActive.CardData.MetaData.CardId == (int)BGCards.ClefairyDoll)
                                GetPlaygroundByPlayer(retreatData.Player).MoveFromBenchToDiscard(cardInActive.CardData.UniqueCardID);
                            else
                            {
                                // We close retreat.
                                GetPlaygroundByPlayer(retreatData.Player).IsRetreatable = false;
                            }
                        }
                    }
                }
                break;
            case BattleGameActions.AttackToOpponent:
            case BattleGameActions.AttackToOpponentEffect:
                {
                    // Attack response data.
                    BGAttackResponseDTO ad = responseData.Data.ToObject<BGAttackResponseDTO>();

                    // We get the attacker player.
                    PlaygroundController ap = GetPlaygroundByPlayer(ad.APlayer);

                    // We get the defender player.
                    PlaygroundController dp = GetPlaygroundByPlayer(ad.OPlayer);

                    // Attack info.
                    AttackDTO attackerAttack = null;

                    // if not a copy we can look at the attacker.
                    if (!ad.IsCopy)
                        attackerAttack = ap.PlayerActive.CardData.MetaData.GetAttackByID(ad.AttackID);
                    else // if it is a coppy we just need to look for opponent active monster.
                        attackerAttack = dp.PlayerActive.CardData.MetaData.GetAttackByID(ad.AttackID);

                    // We update the last damage data.
                    dp.PlayerActive.LastDamageData = ad;

                    // We just receive the response.
                    if (!AttackController.Instance.IsAttackActive(ad.UniqueID))
                    {
                        AttackController.Instance.NewAttack(ap.PlayerActive, attackerAttack);
                        AttackController.Instance.ActiveAttack.UniqueID = ad.UniqueID;
                        AttackController.Instance.ActiveAttack.Generate();
                    }

                    // When attacker confused we will hitself.
                    if (ad.IsConfused)
                    {
                        // We just flip a coin and hit the opponent.
                        ap.PlayerCoin.FlipCoin(() =>
                        {
                            // We create a damage.
                            DamageDTO damage = new DamageDTO { Damage = ad.SelfDamage };

                            // Player will hitself.
                            ap.StartCoroutine(ap.HitToTarget(ap.PlayerActive, ap.PlayerActive, damage, () => OnAttackEnd(ad, ap, dp)));
                        }, Flips.Tails);

                        // We dont have to go deep.
                        return;
                    }

                    // Otherwise we hit the opponent.

                    // When the damage is dodged.
                    if (ad.Damage.DodgedTried)
                    {
                        // We flip a coin for the attacker.
                        GetPlaygroundByPlayer(ad.APlayer).PlayerCoin.FlipCoin(() =>
                        {
                            // Player attack dodged.
                            if (ad.Damage.DodgeSucceed)
                                this.OnAttackEnd(ad, ap, dp);
                            else
                                this.OnAttackBegin(ad, ap, dp);
                        }, ad.Damage.DodgedCoin);

                        // We dont have to go deep.
                        return;
                    }

                    // if damage not dodged.
                    // But before hit if attacker monster confused we also flip a coin.
                    if (ap.PlayerActive.SpecialCondition == BGSpecialConditions.Confused)
                    {
                        // We just flip a coin and hit the opponent.
                        ap.PlayerCoin.FlipCoin(() =>
                        {
                            // We load also the response.
                            //AttackController.Instance.ActiveAttack.Simulate(ad, () => OnAttackEnd(ad, ap, dp));
                            // We begin attack.
                            this.OnAttackBegin(ad, ap, dp);

                        }, Flips.Heads);

                        // We dont have to go deep.
                        return;
                    }

                    // We begin attack.
                    this.OnAttackBegin(ad, ap, dp);

                }
                break;
            case BattleGameActions.PlayNewActiveMonster:
                {
                    // We get the data.
                    BGPlayCardResponseDTO newCardData = responseData.Data.ToObject<BGPlayCardResponseDTO>();

                    // We get the card in bench.
                    CardController cardInBench = GetPlaygroundByPlayer(newCardData.Player).GetCardInBench(newCardData.PlayedCard.UniqueCardID);

                    // We activate the card.
                    GetPlaygroundByPlayer(newCardData.Player).MoveFromBenchToActive(cardInBench);

                    // if player itself we are going to disable the game.
                    if (newCardData.Player == GameStartData.PColor)
                        BattleNotiController.Instance.GOForceToSelectActiveCard.SetActive(false);
                    else
                        BattleNotiController.Instance.GOYourOpSelectingAMonster.SetActive(false);

                    // We show done if possible.
                    BattleNotiController.Instance.ShowDoneButton();
                }
                break;
            case BattleGameActions.SelectAPrize:
                {
                    // We get the prize data.
                    BGPlayCardResponseDTO receivedPrizeData = responseData.Data.ToObject<BGPlayCardResponseDTO>();

                    // We get the playground.
                    PlaygroundController playground = GetPlaygroundByPlayer(receivedPrizeData.Player);

                    // We get the prize card.
                    CardController card = playground.GetCardInPrize(receivedPrizeData.PlayedCard.UniqueCardID);

                    // We set the data.
                    card.SetCardData(receivedPrizeData.PlayedCard);

                    // We transfered card to hand.
                    playground.PrizeToHand(card);
                }
                break;
            case BattleGameActions.GameIsOver:
                {
                    // The game is over.
                    this.IsGameOver = true;

                    // We get the game over data.
                    BGGameOverDTO gameOverData = responseData.Data.ToObject<BGGameOverDTO>();

                    // We finalize the game and show the winner.
                    if (gameOverData.Winners.Count == 1)
                    {
                        // if winner is the player.
                        if (GetPlaygroundByPlayer(gameOverData.Winners[0]).IsRealPlayer)
                        {
                            // Player win the game.
                            BattleNotiController.Instance.ShowGameOverForWin();
                        }
                        else // Otherwise other player win the game.
                        {
                            // Opponent win the game.
                            BattleNotiController.Instance.ShowGameOverForLose();
                        }
                    }
                    else // Means count is 2 there is a tie.
                    {
                        // Tie view.
                        BattleNotiController.Instance.ShowGameOverForTie();
                    }
                }
                break;
            case BattleGameActions.PlayTrainerCard:
                {
                    BGTrainerResponsePlayerDTO pd = responseData.Data.ToObject<BGTrainerResponsePlayerDTO>();

                    // We get the attacker player.
                    PlaygroundController ap = GetPlaygroundByPlayer(pd.Player);

                    // We get the defender player.
                    PlaygroundController dp = GetOpponentOfPlayground(ap);

                    // We get the card in hand.
                    CardController cardData = ap.GetCardInHand(pd.PCard.UniqueCardID);

                    // if card is not exists in the hand.
                    if (cardData == null)
                    {
                        // We get the trainer card.
                        cardData = ap.PlayerTrainer;

                        // if card not exists just return.
                        if (cardData == null) return;

                        // if not same card just return.
                        if (cardData.CardData.UniqueCardID != pd.PCard.UniqueCardID) return;
                    }

                    // We update the played cards.
                    cardData.SetCardData(pd.PCard);

                    // We just receive the response.
                    if (!TrainerController.Instance.IsTrainerActive(pd.UniqueID))
                    {
                        TrainerController.Instance.NewTrainer(cardData);
                        TrainerController.Instance.ActiveTrainer.UniqueID = pd.UniqueID;
                        TrainerController.Instance.ActiveTrainer.Generate();
                    }

                    // On simulation completed.
                    TrainerController.Instance.ActiveTrainer.Simulate(pd, () =>
                    {
                        // When the trainer card played its going to be true.
                        BattleNotiController.Instance.ShowDoneButton();
                    });
                }
                break;
            case BattleGameActions.PlayTrainerCardEffect:
                {
                    // Trainer response data.
                    BGTrainerResponsePlayerDTO tr = responseData.Data.ToObject<BGTrainerResponsePlayerDTO>();

                    // We get the player playground.
                    PlaygroundController ap = GetPlaygroundByPlayer(tr.Player);

                    // We get the opponent playground.
                    PlaygroundController dp = GetOpponentOfPlayground(ap);

                    // We make sure attack is active.
                    if (TrainerController.Instance.IsTrainerActive(tr.UniqueID))
                    {
                        // We continue the simulation of trainer card.
                        TrainerController.Instance.ActiveTrainer.Simulate(tr, () =>
                        {
                            // When the trainer card played its going to be true.
                            BattleNotiController.Instance.ShowDoneButton();
                        });
                    }
                }
                break;
            case BattleGameActions.Mulligan:
                {
                    // We get the mulligan data.
                    BGMulliganDTO mulliganData = responseData.Data.ToObject<BGMulliganDTO>();

                    // We store the mulligan player.
                    PlaygroundController mPlayer = GetPlaygroundByPlayer(mulliganData.Player);

                    // We show the player the opponent hand.
                    if (!mPlayer.IsRealPlayer)
                    {
                        // We show the mulligan cards.
                        HandViewerController.Instance.ShowMulliganHand(mulliganData.MCards);
                    }

                    // We draw a card for the opponent.
                    GetOpponentOfPlayground(mPlayer).DrawACard(mulliganData.DCard);

                    // We store the player hand.
                    List<CardController> pHand = mPlayer.PlayerHand.ToList();

                    // We throw all the cards to deck.
                    mPlayer.StartCoroutine(mPlayer.MoveFromHandToDeckMultiple(pHand, () =>
                    {
                        // Then we redraw the given cards.
                        mPlayer.StartCoroutine(mPlayer.DrawMultipleCards(mulliganData.NewHand, () =>
                        {
                            // if this is the real player we will check one more time.
                            if (mPlayer.IsRealPlayer)
                                StartCoroutine(CheckPlayerHandIsMulligan());
                        }));
                    }));
                }
                break;
            case BattleGameActions.PlayAbility:
                {
                    BGAbilityResponseDTO abr = responseData.Data.ToObject<BGAbilityResponseDTO>();

                    // We get the attacker player.
                    PlaygroundController ap = GetPlaygroundByPlayer(abr.RequestData.Player);

                    // We get the defender player.
                    PlaygroundController dp = GetOpponentOfPlayground(ap);

                    CardController card = null;

                    // We get the played card.
                    if (ap.IsCardActive(abr.RequestData.CardID))
                        card = ap.PlayerActive;
                    else if (ap.IsCardInBench(abr.RequestData.CardID))
                        card = ap.GetCardInBench(abr.RequestData.CardID);

                    // We just receive the response.
                    if (!AbilityController.Instance.IsAbilityActive(abr.RequestData.UniqueID))
                    {
                        AbilityController.Instance.NewAbility(card);
                        AbilityController.Instance.ActiveAbility.UniqueID = abr.RequestData.UniqueID;
                        AbilityController.Instance.ActiveAbility.Generate();
                    }

                    // On simulation completed.
                    AbilityController.Instance.ActiveAbility.Simulate(abr, () =>
                    {
                        // We clear the ability.
                        BattleNotiController.Instance.OnClickCancelAbility();
                    });
                }
                break;
            case BattleGameActions.PlayAbilityEffect:
                {
                    // We get the ability data.
                    BGAbilityResponseDTO abr = responseData.Data.ToObject<BGAbilityResponseDTO>();

                    // We get the player playground.
                    PlaygroundController ap = GetPlaygroundByPlayer(abr.RequestData.Player);

                    // We get the opponent playground.
                    PlaygroundController dp = GetOpponentOfPlayground(ap);

                    // We make sure attack is active.
                    if (AbilityController.Instance.IsAbilityActive(abr.RequestData.UniqueID))
                    {
                        // We continue the simulation of ability card.
                        AbilityController.Instance.ActiveAbility.Simulate(abr, () =>
                        {
                            // We clear the ability.
                            BattleNotiController.Instance.OnClickCancelAbility();
                        });
                    }
                }
                break;
            case BattleGameActions.CancelAbility:
                {
                    // Currently empty.
                }
                break;
            default:
                break;
        }
    }

    public void OnAttackBegin(BGAttackResponseDTO ad, PlaygroundController ap, PlaygroundController dp)
    {
        // We load also the response.
        AttackController.Instance.ActiveAttack.Simulate(ad, () =>
        {
            // When the attacker forced to knockout.
            if (ad.AKnockedOut)
            {
                // We get the attacker monster.
                CardController apAttackerMonster = ap.PlayerActive;

                // We run the knockout.
                StartCoroutine(ap.ToKnockout(apAttackerMonster, () =>
                {
                    // We tell defender now will pick a monster for our knocked out monster.
                    if (apAttackerMonster.CardData.MetaData.CardId != (int)BGCards.ClefairyDoll)
                        GetOpponentOfPlayground(ap).WaitingPrizeQuantity++;

                    // We execute final data.
                    OnAttackEnd(ad, ap, dp);
                }));
            }
            else
            {
                // We execute final data.
                OnAttackEnd(ad, ap, dp);
            }
        });
    }
    public void OnAttackEnd(BGAttackResponseDTO ad, PlaygroundController ap, PlaygroundController dp)
    {
        // if both of them is zero then its means we just skip automatically.
        if (GetPlaygroundByPlayer(ad.APlayer).IsRealPlayer)
        {
            if (ap.WaitingPrizeQuantity == 0 && dp.WaitingPrizeQuantity == 0)
                BattleNotiController.Instance.OnClickDone();

            // if action completed we are going to complete it.
            if (ap.PlayerActive != null && ap.PlayerActive.CardState == BGCardStates.Action)
                ap.DeactivateAction();

            // When the trainer card played its going to be true.
            BattleNotiController.Instance.ShowDoneButton();
        }
    }

    public PlaygroundController GetOpponentOfPlayground(PlaygroundController player)
    {
        // if players turn just return the opponent.
        if (Player == player)
            return this.Opponent;

        // Otherwise return player.
        return this.Player;
    }
    public PlaygroundController GetPlaygroundByPlayer(BattleGamePlayers player)
    {
        // if players turn just return the player.
        if (this.GameStartData.PColor == player)
            return this.Player;

        // Otherwise return opponent.
        return this.Opponent;
    }
    public PlaygroundController GetCurrentPlayerPlayground()
    {
        // if players turn just return the player.
        if (this.GameStartData.PColor == CurrentPlayer)
            return this.Player;

        // Otherwise return opponent.
        return this.Opponent;
    }
    public PlaygroundController GetCurrentPlayerOpponentPlayground()
    {
        // if players turn just return the player.
        if (this.GameStartData.PColor != CurrentPlayer)
            return this.Player;

        // Otherwise return opponent.
        return this.Opponent;
    }

    public IEnumerator SwitchPlayer(BattleGamePlayers currentPlayer)
    {
        // if same player cant change.
        if (CurrentPlayer == currentPlayer) yield break;

        // We clear the active attack and active trainer.
        AttackController.Instance.ClearAttack();
        TrainerController.Instance.ClearTrainer();
        AbilityController.Instance.ClearAbility();

        // if action is active then we will disable it in changes.
        if (GetPlaygroundByPlayer(this.GameStartData.PColor).PlayerActive?.CardState == BGCardStates.Action)
            GetPlaygroundByPlayer(this.GameStartData.PColor).PlayerActive.SetState(BGCardStates.Active);

        // We get the current play ground.
        PlaygroundController p = GetCurrentPlayerPlayground();

        // We remove the blocked attack.
        p.BlockTheAttack(BGAttacks.None);

        // We reset the retreat state.
        p.IsRetreatable = true;

        // We check active pokemon exists of older player.
        if (p.PlayerActive != null)
        {
            // We check card is poisoned. Then we hit.
            if (p.PlayerActive.SpecialCondition == BGSpecialConditions.Poisoned)
            {
                // We make sure it is not shielded.
                if (p.PlayerActive.Shield == BGShieldTypes.None)
                {
                    // We hit the poison damage.
                    p.PlayerActive.CardDamage.HitDamage(p.PlayerActive, p.PlayerActive.PoisonDamage);
                }
            }

            // if monster was paralyzed we remove it when its player turn.
            if (p.PlayerActive.SpecialCondition == BGSpecialConditions.Paralyzed)
                p.PlayerActive.SetSpecialCondition(BGSpecialConditions.None);
        }

        // Set the current player.
        this.CurrentPlayer = currentPlayer;

        // We deactivate the selection.
        SelectedCards.ForEach(e => e.DeSelect());

        // We clear the list of cards.
        SelectedCards.Clear();

        // if its player turn your turn object going to be active.
        GOYourTurn.gameObject.SetActive(this.CurrentPlayer == this.GameStartData.PColor);

        // if opponent turn it is going to be active.
        GOOpponentTurn.gameObject.SetActive(this.CurrentPlayer != this.GameStartData.PColor);

        // When the game started if user is the same we activate done button.
        if (this.IsGameStarted)
        {
            if (this.CurrentPlayer == this.GameStartData.PColor)
                BattleNotiController.Instance.ShowDoneButton();
            else
                BattleNotiController.Instance.GODone.SetActive(false);

            // We reset as not energy attached.
            this.IsAnEnergyAttached = false;

            // We get the active monster.
            CardController activeMonsterOfPlayer = GetPlaygroundByPlayer(this.GameStartData.PColor).PlayerActive;

            // We remove the block.
            if (activeMonsterOfPlayer != null && activeMonsterOfPlayer.EvolveBlocked)
            {
                // We reset all temp energies.
                activeMonsterOfPlayer.CardEnergyAttachment.AttachedEnergies.ForEach(e => e.Item1.ClearTempEnergy());

                /// We also clear per turn data.
                activeMonsterOfPlayer.PerTurnData = false;

                // We remove the evolution block.
                activeMonsterOfPlayer.EvolveBlocked = false;

                // We clear the attached trainer.
                activeMonsterOfPlayer.SetAttachATrainerCard(null);
            }

            // We also remove all the blocked benches.
            foreach (CardController bench in GetPlaygroundByPlayer(this.GameStartData.PColor).PlayerBenched)
            {
                // We reset all temp energies.
                bench.CardEnergyAttachment.AttachedEnergies.ForEach(e => e.Item1.ClearTempEnergy());

                /// We also clear per turn data.
                bench.PerTurnData = false;

                // if already blocked we remove block.
                bench.EvolveBlocked = false;

                // We will remove the trainer card.
                bench.SetAttachATrainerCard(null);
            }

            // Oyuncunun kendisine ait korumayý kaldýrýyoruz.
            GetCurrentPlayerPlayground().PlayerActive?.SetShield(BGShieldTypes.None);

            // We make sure it is real player.
            if (GetCurrentPlayerPlayground().IsRealPlayer)
            {
                // if prize exists click to collect.
                if (GetCurrentPlayerPlayground().WaitingPrizeQuantity > 0)
                    GetCurrentPlayerPlayground().ShowPrizeView();

                // We wait until prize quantity 0.
                yield return new WaitUntil(() => GetCurrentPlayerPlayground().WaitingPrizeQuantity == 0);

                // We force to select.
                if (GetCurrentPlayerPlayground().ForceToSelectActive)
                    BattleNotiController.Instance.GOForceToSelectActiveCard.SetActive(true);
            }
        }
    }

    public IEnumerator CheckPlayerHandIsMulligan()
    {
        // if player hand is mulligan we will tell it.
        if (!Player.PlayerHand.Any(x => x.CardData.MetaData.IsBasic && x.CardData.MetaData.CardTypeId == CardTypes.Pokemon))
        {
            // We will tell it is mulligan.
            BattleNotiController.Instance.GOPlayerMulligan.SetActive(true);
        }

        // We wait until its player turn.
        yield return new WaitUntil(() => CurrentPlayer == GameStartData.PColor);

        // We wait some seconds.
        yield return new WaitForSeconds(1);

        // if no any basic pokemon.
        if (!Player.PlayerHand.Any(x => x.CardData.MetaData.IsBasic && x.CardData.MetaData.CardTypeId == CardTypes.Pokemon))
        {
            // We tell the server my hand is mulligan.
            SendGameAction(BattleGameActions.Mulligan);
        }
        else
        {
            // We will tell it is mulligan.
            BattleNotiController.Instance.GOPlayerMulligan.SetActive(false);
        }
    }
}
