using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlaygroundController : MonoBehaviour
{
    #region PROPS AND FIELDS

    [Header("Is that real player")]
    public bool IsRealPlayer;

    [Header("Move speed of card.")]
    public float MoveSpeed;

    [Header("Rotation speed of card.")]
    public float RotationSpeed;

    [Header("Player coin.")]
    public CoinController PlayerCoin;

    [Header("Card to use.")]
    public GameObject GOCard;

    [Header("Benched cards.")]
    public RectTransform TRBench;

    [Header("Spawn position of deck.")]
    public RectTransform TRDeck;

    [Header("We are going to discard cards to this position.")]
    public RectTransform TRDiscardPile;

    [Header("Hand center position.")]
    public RectTransform TRHand;

    [Header("Active card place.")]
    public RectTransform TRActive;

    [Header("When clicked card we will show the possible actions for the card.")]
    public RectTransform TRAction;

    [Header("When knockout we will redirect to the point.")]
    public RectTransform TRKnockout;

    [Header("When card will attack it will go to this point.")]
    public RectTransform TRHitPoint;

    [Header("Trainer playground.")]
    public RectTransform TRTrainer;

    [Header("Card front rotation")]
    public Vector3 CardRotation;

    [Header("We are going to store prize cards in here.")]
    public RectTransform TRPrize;

    [Header("Player cards in hand.")]
    public List<CardController> PlayerHand;

    [Header("Player discard pile")]
    public List<CardController> PlayerDiscard;

    [Header("Player prizes.")]
    public Dictionary<int, CardController> PlayerPrizes = new Dictionary<int, CardController>();

    [Header("Player benched cards.")]
    public List<CardController> PlayerBenched;

    [Header("Player active cards.")]
    public CardController PlayerActive;

    [Header("Players selected card to show details.")]
    public CardController PlayerAction;

    [Header("Player trainer card.")]
    public CardController PlayerTrainer;

    [Header("When the card player hand exceed then cards force to offset eachothers.")]
    public int ResizeQuantity;

    [Header("When an attack is blocked it is going to store here.")]
    public BGAttacks BlockedAttack;

    [Header("We will print the player prize quantity.")]
    public TMP_Text TXTPrizeQuantity;

    [Header("Player waiting to receive prize quantity.")]
    public int WaitingPrizeQuantity;

    [Header("Is player card retreatable.")]
    public bool IsRetreatable;

    /// <summary>
    /// When player has to select a new active card this field is going to be true.
    /// </summary>
    public bool ForceToSelectActive => PlayerActive == null && BattleGameController.Instance?.IsFirstRound == false;

    #endregion

    #region ACTIVATE / DEACTIVATE ACTIONS

    public void ActivateAction(CardController active)
    {
        // We deactivate the older one.
        if (PlayerAction != null)
            DeactivateAction();

        // if card state is not active just return.
        if (active.CardState != BGCardStates.Active && active.CardState != BGCardStates.Bench) return;

        // We active the details.
        active.ActivateDetails();

        // We update the parent to show in detail view.
        active.transform.SetParent(TRAction);

        // We update the target to go to position.
        active.GoToTarget(Vector3.zero);

        // We rotate to start rotation.
        active.transform.localRotation = Quaternion.identity;

        // We rescale the card.
        active.transform.localScale = Vector3.one;

        // We change the state as active detail.
        active.SetState(BGCardStates.Action);

        // We activate the blocker to prevent click back.
        TRAction.transform.Find("Blocker").gameObject.SetActive(true);

        // We set to active.
        PlayerAction = active;
    }
    public void DeactivateAction()
    {
        // if card not defined we use default.
        if (PlayerAction == null) return;

        // if forced to switch an opponent monster with their benched monster then you cant activate detail
        if (BattleNotiController.Instance.GOSelectOpponentBenchMonster.activeSelf) return;

        // We make sure the state of card is not active.
        if (PlayerAction.CardState != BGCardStates.Action) return;

        // if forced we wont let do this.
        if (CardEnergyDetachController.Instance.IsDetachActive &&
            CardEnergyDetachController.Instance.IsForced)
            return;

        // We disable the details informations.
        PlayerAction.DeactivateDetails();

        // We disable the blocker to let player click to playground.
        TRAction.transform.Find("Blocker").gameObject.SetActive(false);

        // if the card is in hand.
        if (IsCardInBench(PlayerAction.CardData.UniqueCardID))
        {
            // We change the card state as benched.
            PlayerAction.SetState(BGCardStates.Bench);

            // We carry from detail to bench.
            PlayerAction.transform.SetParent(TRBench);

            // We set the position.
            PlayerAction.GoToTarget(Vector3.zero);

            // We rescale.
            PlayerAction.transform.localScale = Vector3.one;

            // We update card in bench.
            UpdateCardPositionsInBench();
        }
        else if (IsCardActive(PlayerAction.CardData.UniqueCardID))
        {
            // We change the card state as active.
            PlayerAction.SetState(BGCardStates.Active);

            // We carry from detail to active.
            PlayerAction.transform.SetParent(TRActive);

            // We set the position.
            PlayerAction.GoToTarget(Vector3.zero);

            // We rescale.
            PlayerAction.transform.localScale = Vector3.one;
        }
    }

    #endregion

    #region EVOLUTIONS

    public void EvolveActiveMonster(CardController evolveCard)
    {
        // Otherwise we will evolve it.
        CardController olderCard = this.PlayerActive;

        // We update the evoluted card.
        this.PlayerActive = null;

        // if older card is in active then we will activate.
        this.MoveFromHandToActive(evolveCard);

        // We attach older pokemon with new.
        evolveCard.AttachedLowStage = olderCard;
        olderCard.AttachedHighStage = evolveCard;

        // We transfer the taken damage.
        evolveCard.CardDamage.SetDamage(olderCard.CardDamage.TakenDamage);

        // We add energies into the new evolution.
        olderCard.CardEnergyAttachment.AttachedEnergies.ForEach(e =>
        {
            // We update evolve card attached data.
            e.Item2.EnergyAttachedTo = evolveCard;

            // We attach all the energy cards to the new evolution card.
            evolveCard.CardEnergyAttachment.AttachAnEnergyToMonsterCard(e.Item2);
        });

        // We clear special condition.
        olderCard.SetSpecialCondition(BGSpecialConditions.None);

        // We deteach all the energies.
        olderCard.CardEnergyAttachment.DetachAll();

        // We disable the card.
        olderCard.gameObject.SetActive(false);
    }
    public void EvolveBenchedMonster(CardController benchCard, CardController evolveCard)
    {
        // We get the older index.
        int benchIndex = this.PlayerBenched.IndexOf(benchCard);

        // We remove the card.
        this.RemoveFromBench(benchCard);

        // We play card to bench.
        this.MoveFromHandToBench(evolveCard, benchIndex);

        // We attach older pokemon with new.
        evolveCard.AttachedLowStage = benchCard;
        benchCard.AttachedHighStage = evolveCard;

        // We transfer the taken damage.
        evolveCard.CardDamage.SetDamage(benchCard.CardDamage.TakenDamage);

        // We add energies into the new evolution.
        benchCard.CardEnergyAttachment.AttachedEnergies.ForEach(e =>
        {
            // We update evolve card attached data.
            e.Item2.EnergyAttachedTo = evolveCard;

            // We attach all the energy cards to the new evolution card.
            evolveCard.CardEnergyAttachment.AttachAnEnergyToMonsterCard(e.Item2);
        });

        // We deteach all the energies.
        benchCard.CardEnergyAttachment.DetachAll();

        // We disable the card.
        benchCard.gameObject.SetActive(false);
    }

    #endregion

    #region HIT AND KNOCKOUT

    public void BlockTheAttack(BGAttacks blockedAttack)
    {
        this.BlockedAttack = blockedAttack;
    }
    
    public IEnumerator HitToTarget(CardController attacker, CardController defender, DamageDTO damage, Action onCallBack)
    {
        // We update the parent to show in detail view.
        attacker.transform.SetParent(TRHitPoint);

        // Auto rotate is going to be disabled to prevent card rotate in detail view.
        attacker.AutoRotate = false;

        // We apply speed.
        attacker.SpeedMultiplier = 2;

        // We update the target to go to position.
        attacker.GoToTarget(Vector3.zero);

        yield return new WaitForSeconds(.1f);

        // We hit damage to target.
        defender.CardDamage.HitDamage(attacker, damage);

        // We activate the prize view.
        if (defender.CardDamage.IsDeath && defender.CardData.MetaData.CardId != (int)BGCards.ClefairyDoll)
            BattleGameController.Instance.GetOpponentOfPlayground(defender.Playground).WaitingPrizeQuantity++;

        // We reactivate auto rotate.
        attacker.AutoRotate = true;

        // We deactivate the action.
        DeactivateAction();

        // We apply speed.
        attacker.SpeedMultiplier = 1;

        // We update the parent to show in detail view.
        attacker.transform.SetParent(TRActive);

        // We update the target to go to position.
        attacker.GoToTarget(Vector3.zero, onCallBack);
    }
    public IEnumerator ToKnockout(CardController card, Action onKnockoutCompleted)
    {
        // We delete from the state.
        switch (card.CardState)
        {
            case BGCardStates.Bench:
                this.RemoveFromBench(card);
                break;
            case BGCardStates.Active:
            case BGCardStates.Action:
                this.RemoveFromActive();
                break;
        }

        // We set card state as knockout.
        card.SetState(BGCardStates.Knockout);

        // We clear its special condition.
        card.SetSpecialCondition(BGSpecialConditions.None);

        // We wait 1 sec.
        yield return new WaitForSeconds(1);

        // We set knockout parent.
        card.transform.SetParent(TRKnockout);

        // We rescale the card.
        card.transform.localScale = Vector3.one;

        // We send to target.
        card.GoToTarget(Vector3.zero, () => { });

        // We wait until arrive.
        yield return new WaitUntil(() => card.OnArrivedToTarget == null);

        // We get the attached cards.
        List<CardController> cardsToDetach = new List<CardController>();

        // We get the attach.
        CardController attached = card.EnergyAttachedTo;

        // We loop until add attach list all the attached cards.
        while (attached != null)
        {
            // We get the attach of the card.
            cardsToDetach.Add(attached);

            // We go to next.
            attached = attached.EnergyAttachedTo;
        }

        // We get the attach.
        attached = card.AttachedLowStage;

        // We loop until add attach list all the attached cards.
        while (attached != null)
        {
            // We get the attach of the card.
            cardsToDetach.Add(attached);

            // We go to next.
            attached = attached.AttachedLowStage;
        }

        // We also add all the attached energies.
        cardsToDetach.AddRange(card.CardEnergyAttachment.AttachedEnergies.Select(x => x.Item2));

        // if energy card exists we will detach them.
        if (cardsToDetach.Count > 0)
        {
            // We wait some seconds to make it a little bit smooth.
            yield return new WaitForSeconds(.5f);

            // We loop all the cards.
            foreach (CardController detachCard in cardsToDetach)
            {
                // We detach the card.
                this.AddToDiscard(detachCard);

                // We wait some seconds.
                yield return new WaitForSeconds(.3f);
            }
        }

        // Lastly we detach the card.
        this.AddToDiscard(card);

        // We activate the prize view.
        PlaygroundController opponentPlayground = BattleGameController.Instance.GetOpponentOfPlayground(this);

        // We show the prize view. when knockout monster is not player monster.
        if (opponentPlayground.IsRealPlayer && BattleGameController.Instance.IsPlayerTurn)
        {
            // We show the prize.
            if (opponentPlayground.WaitingPrizeQuantity > 0)
                opponentPlayground.ShowPrizeView();

            // We close force view its because we will wait until the prize selected.
            BattleNotiController.Instance.GOForceToSelectActiveCard.SetActive(false);

            // We wait untill all rewards received.
            yield return new WaitUntil(() => opponentPlayground.WaitingPrizeQuantity == 0);

            // if force exists we will show the view.
            if (opponentPlayground.ForceToSelectActive)
                BattleNotiController.Instance.GOForceToSelectActiveCard.SetActive(true);
        }

        // We also return the state.
        if (onKnockoutCompleted != null)
            onKnockoutCompleted.Invoke();
    }

    #endregion

    #region ATTACHMENTS

    public bool AddToAttachmentsFromHand(CardController attachmentItem, CardController target)
    {
        // if card is not in hand just return.
        if (!IsCardInHand(attachmentItem.CardData.UniqueCardID)) return false;

        // We remove card from hand.
        this.RemoveFromHand(attachmentItem);

        // We attach the card.
        attachmentItem.transform.SetParent(target.transform);

        // We set as attached.
        attachmentItem.SetState(BGCardStates.Attached);

        // We set target to 0.
        attachmentItem.GoToTarget(Vector3.zero);

        // We update the hand cards.
        UpdateCardPositionsInHand();

        // We return.
        return true;
    }

    public bool AddAttachmentToCard(CardController attachmentItem, CardController target)
    {
        // We attach the card.
        target.CardEnergyAttachment.AttachAnEnergyToMonsterCard(attachmentItem);

        // Set as attached.
        attachmentItem.EnergyAttachedTo = target;

        // We attach the card.
        attachmentItem.transform.SetParent(target.transform);

        // We set as attached.
        attachmentItem.SetState(BGCardStates.Attached);

        // We set target to 0.
        attachmentItem.GoToTarget(Vector3.zero);

        // We return.
        return true;
    }

    public void AttachAnEnergyCardFromHand(CardController energyCard, CardController target)
    {
        // We remove from the hand.
        this.RemoveFromHand(energyCard);

        // We attach to active card.
        target.CardEnergyAttachment.AttachAnEnergyToMonsterCard(energyCard);

        // We tell the energy card attached to active card.
        energyCard.EnergyAttachedTo = target;

        // We change the state of the card.
        energyCard.SetState(BGCardStates.Attached);

        // We change the parent.
        energyCard.transform.SetParent(target.transform);

        // We update the target position to move card.
        energyCard.GoToTarget(Vector3.zero);
    }

    #endregion

    #region PRIZE

    public void PrizeToHand(CardController prizeItem)
    {
        // We make sure card is in player prize.
        if (!PlayerPrizes.ContainsValue(prizeItem)) return;

        // We get the item in prize.
        var prizeInList = PlayerPrizes.FirstOrDefault(x => x.Value == prizeItem);

        // We remove from the prize list.
        PlayerPrizes.Remove(prizeInList.Key);

        // We update the prize quantity.
        this.UpdatePlayerPrizeText();

        // We add to player hand.
        this.AddToHand(prizeItem);

        // We reduce the prize quantity to receive.
        this.WaitingPrizeQuantity--;

        // if no left prize just left.
        if (this.WaitingPrizeQuantity <= 0)
            this.ClosePrizeView();
    }
    public CardController GetCardInPrize(int uniqueCardID)
    {
        if (!PlayerPrizes.Any(x => x.Value.CardData.UniqueCardID == uniqueCardID)) return null;
        return PlayerPrizes.FirstOrDefault(x => x.Value.CardData.UniqueCardID == uniqueCardID).Value;
    }

    public void ShowPrizeView()
    {
        // We get the prize view.
        Transform prizeSelectionView = transform.Find("PrizeSelectionView");

        // We activate the selection view.
        prizeSelectionView.gameObject.SetActive(true);

        // We loop all the prizes.
        foreach (var prize in PlayerPrizes)
        {
            // We get the position of prize.
            Vector3 position = Vector3.zero;

            // One side is going to be on the left otherside is going to be right.
            position.x = -200 + prize.Key % 3 * 200;
            position.y = -100 + prize.Key % 2 * 200;

            // We force to move all the cards to 
            prize.Value.GoToTarget(position);

            // We update its parent.
            prize.Value.transform.SetParent(prizeSelectionView);

            // We set as prize view.
            prize.Value.SetState(BGCardStates.PrizeView);
        };
    }
    public void ClosePrizeView()
    {
        // List of player rewards.
        foreach (var prize in PlayerPrizes)
        {
            // We update the parent.
            prize.Value.transform.SetParent(TRPrize);

            // We apply the target.
            CardController cardMoveItem = prize.Value.GetComponent<CardController>();
            prize.Value.transform.localRotation = Quaternion.identity;

            // We calculate the new position.
            Vector3 prizePosition = Vector3.zero;
            prizePosition.z -= 2 * prize.Key;

            // We apply the target position.
            cardMoveItem.AutoRotate = false;

            // We set the target.
            cardMoveItem.GoToTarget(prizePosition);

            // We set as prize.
            if (prize.Value.CardState == BGCardStates.PrizeView)
                prize.Value.SetState(BGCardStates.Prize);
        }

        // We find the selection view.
        Transform prizeSelectionView = transform.Find("PrizeSelectionView");

        // We deactivate the view.
        prizeSelectionView.gameObject.SetActive(false);
    }
    public void UpdatePlayerPrizeText()
    {
        // We update the prize.
        this.TXTPrizeQuantity.text = $"{PlayerPrizes.Count}";
    }

    #endregion

    #region DECK

    public void AddToDeck(CardController card)
    {
        // We set an empty dto.
        card.SetCardData(BGCardDTO.ToEmpty(card));

        // We update it as hand.
        card.SetState(BGCardStates.Deck);

        // We transfer it to deck.
        card.transform.SetParent(TRDeck);

        // We update the arrive position.
        card.GoToTarget(Vector3.zero,() => { });
    }

    public CardController DrawACard(BGCardDTO card, Action onArrivedDestination = null)
    {
        // We draw a card.
        CardController cardItem = Instantiate(GOCard, TRDeck).GetComponent<CardController>();

        // We get the card controller.
        cardItem.SetInstantPosition(Vector3.zero);
        cardItem.SetCardMovement(this);
        cardItem.SetCardData(card);
        
        // We add to player hand.
        this.AddToHand(cardItem);

        // When arrived to destination will be triggered.
        cardItem.SetArriveAction(onArrivedDestination);

        // We return the data.
        return cardItem;
    }
    public IEnumerator DrawMultipleCards(List<BGCardDTO> cards, Action onAllArrivedToDestination = null)
    {
        // if no card exists.
        if (cards.Count == 0)
        {
            // We invoke the listener.
            if (onAllArrivedToDestination != null)
                onAllArrivedToDestination.Invoke();

            // We break the method.
            yield break;
        }

        // We loop all the cards given.
        foreach (BGCardDTO card in cards)
        {
            // We have to wait to make smooth draw action.
            yield return new WaitForSeconds(.3f);

            // We draw the card.
            this.DrawACard(card);
        }

        // We have to wait to make smooth draw action.
        yield return new WaitForSeconds(.3f);

        // if callback exists we will invoke it.
        if (onAllArrivedToDestination != null)
            onAllArrivedToDestination.Invoke();
    }

    #endregion

    #region BENCH

    public void AddToBench(CardController card, int index = -1)
    {
        // We add to benched list.
        if (index == -1)
            this.PlayerBenched.Add(card);
        else
            this.PlayerBenched.Insert(index, card);

        // We carry to bench.
        card.SetState(BGCardStates.Bench);

        // We update the parent.
        card.transform.SetParent(TRBench);

        // We update the position
        UpdateCardPositionsInBench();

    }
    public void RemoveFromBench(CardController card)
    {
        // We remove from the benched list.
        this.PlayerBenched.Remove(card);

        // We carry to bench.
        card.SetState(BGCardStates.None);

        // We update the parent.
        card.transform.SetParent(null);

        // We update the bench position.
        this.UpdateCardPositionsInBench();
    }
    public bool IsCardInBench(int uniqueCardId) => this.PlayerBenched.Exists(x => x.CardData.UniqueCardID == uniqueCardId);
    public CardController GetCardInBench(int uniqueCardID) => PlayerBenched.Find(x => x.CardData.UniqueCardID == uniqueCardID);
    public void UpdateCardPositionsInBench()
    {
        // We get the card props.
        Rect rect = GOCard.GetComponent<RectTransform>().rect;

        // We calculate per offset width.
        float cardOriginalWidth = rect.width + 5;

        // We loop benched cards.
        foreach (CardController card in PlayerBenched)
        {
            // Index of benced card.
            int index = PlayerBenched.IndexOf(card);

            //Main position to offset.
            Vector3 mainPositionOfBench = Vector3.zero;

            // We calculate the position.
            mainPositionOfBench.x += cardOriginalWidth / 2 + cardOriginalWidth * -2.5f + cardOriginalWidth * index;

            // We set the target.
            card.GoToTarget(mainPositionOfBench);
        }
    }

    #endregion

    #region ACTIVE

    public void AddToActive(CardController card, Action onArrivedTarget = null)
    {
        // We activate the card.
        card.gameObject.SetActive(true);

        // We update the active.
        this.PlayerActive = card;

        // We set as active.
        this.PlayerActive.SetState(BGCardStates.Active);

        // We change its parent.
        card.transform.SetParent(TRActive);

        // We set a target.
        card.GoToTarget(Vector3.zero, onArrivedTarget);
    }
    public void RemoveFromActive()
    {
        // if already empty just return.
        if (this.PlayerActive == null) return;

        // if details are shown we deactivate them.
        if (this.PlayerActive.CardState == BGCardStates.Action)
            this.PlayerActive.DeactivateDetails();

        // We clear the actve monster.
        this.PlayerActive = null;

        // if not a realy player just return.
        if (!this.IsRealPlayer) return;

        // if not player turn just return.
        if (!BattleGameController.Instance.IsPlayerTurn) return;

        // if there is waiting prizes we will wait to receive prizes.
        if (this.WaitingPrizeQuantity > 0) return;

        // We force to select active card.
        BattleNotiController.Instance.GOForceToSelectActiveCard.SetActive(true);
    }
    public bool IsCardActive(int uniqueCardId) => PlayerActive?.CardData.UniqueCardID == uniqueCardId;

    #endregion

    #region HAND

    public bool AddToHand(CardController card)
    {
        // We update it as hand.
        card.SetState(BGCardStates.Hand);

        // We transfer it to hand.
        card.transform.SetParent(TRHand);

        // We update the arrive position.
        card.GoToTarget(Vector3.zero);

        // if not a real player we will just close the card.
        if (!IsRealPlayer)
            card.SetCardData(BGCardDTO.ToEmpty(card));

        // We also reset the card state.
        card.ResetState();

        // We add to it hand.
        this.PlayerHand.Add(card);

        // We update card positions in hand.
        this.UpdateCardPositionsInHand();

        // We tell we added.
        return true;
    }
    public bool IsCardInHand(int uniqueCardId) => this.PlayerHand.Exists(x => x.CardData.UniqueCardID == uniqueCardId);
    public CardController GetCardInHand(int uniqueCardId) => PlayerHand.Find(x => x.CardData.UniqueCardID == uniqueCardId);
    public void UpdateCardPositionsInHand()
    {
        // Rect of the card to get width.
        Rect rect = GOCard.GetComponent<RectTransform>().rect;

        // We get the width.
        float cardOriginalWidth = rect.width;

        // We find the offset value of player cards.
        float offsetMultiplier = PlayerHand.Count - ResizeQuantity;

        // Offset quantity.
        if (offsetMultiplier < 0) offsetMultiplier = 0;

        // We force all the cards to offset eachothers.
        float offset = -(cardOriginalWidth * offsetMultiplier) / PlayerHand.Count;

        // We set the new positions of each cards.
        foreach (CardController card in PlayerHand)
        {
            // Index of card.
            int index = PlayerHand.IndexOf(card);

            // Width of card with offset value.
            float width = cardOriginalWidth + offset;

            // We get the main position.
            Vector3 mainPositionOfHand = Vector3.zero;

            // And we calculate the indexed card position.
            mainPositionOfHand.x -= width * ((PlayerHand.Count - 1) / -2f) + width * index;

            // And we force to offset.
            card.GoToTarget(mainPositionOfHand);
        }
    }
    public bool RemoveFromHand(CardController card)
    {
        // if card is already in the list just return.
        if (!this.PlayerHand.Contains(card)) return false;

        // We should remove from the list.
        this.PlayerHand.Remove(card);

        // We change its state to none.
        card.SetState(BGCardStates.None);

        // We clear the parent.
        card.transform.SetParent(null);

        // We reset card state.
        card.ResetState();

        // We update the discard pile.
        this.UpdateCardPositionsInHand();

        // We return true.
        return true;
    }

    #endregion

    #region DISCARD

    public bool AddToDiscard(CardController card)
    {
        // if card is already in the list just return.
        if (PlayerDiscard.Contains(card)) return false;

        // if not active we activate.
        if (!card.gameObject.activeSelf)
            card.gameObject.SetActive(true);

        // We change its state to discard.
        card.SetState(BGCardStates.Discard);

        // We change its parent.
        card.transform.SetParent(TRDiscardPile);

        // We add to discard pile.
        PlayerDiscard.Add(card);

        // We reset card state.
        card.ResetState();

        // We update the cards position.
        UpdateDiscardPositions();

        // We tell we did it.
        return true;
    }
    public IEnumerator AddToDiscardMultiple(List<CardController> cards, Action onActionCompleted = null)
    {
        // We loop all the cards.
        foreach (CardController card in cards)
        {
            // We add to discard.
            this.AddToDiscard(card);

            // We wait for each .3seconds.
            yield return new WaitForSeconds(.3f);
        }

        // We call the back.
        if (onActionCompleted != null)
            onActionCompleted.Invoke();
    }
    public bool RemoveFromDiscard(CardController card)
    {
        // if card is already in the list just return.
        if (!PlayerDiscard.Contains(card)) return false;

        // We should remove from the list.
        PlayerDiscard.Remove(card);

        // We change its state to discard.
        card.SetState(BGCardStates.None);

        // We clear the parent.
        card.transform.SetParent(null);

        // We reset card state.
        card.ResetState();

        // We update the discard pile.
        UpdateDiscardPositions();

        // We return true.
        return true;
    }
    public bool IsCardInDiscard(int uniqueCardId) => this.PlayerDiscard.Exists(x => x.CardData.UniqueCardID == uniqueCardId);
    public CardController GetCardInDiscard(int uniqueCardID) => PlayerDiscard.Find(x => x.CardData.UniqueCardID == uniqueCardID);
    public void UpdateDiscardPositions()
    {
        int i = 0;

        // We update all card positions.
        this.PlayerDiscard.ForEach(e =>
        {
            // We set the position.
            Vector3 discardPilePosition = Vector3.zero;
            discardPilePosition.z -= 2 * ++i;

            // And force to move through the point.
            e.GoToTarget(discardPilePosition);
        });
    }

    #endregion

    #region TRAINER

    public void PlayTrainer(CardController card)
    {
        // We change the state of the card.
        card.SetState(BGCardStates.Trainer);

        // We update the player trainer.
        PlayerTrainer = card;

        // We change the parent.
        card.transform.SetParent(TRTrainer);

        // We update the target position to move card.
        card.GoToTarget(Vector3.zero);

    }

    #endregion

    #region MULTIPLE MOVES

    public IEnumerator MoveFromDeckToPrizeMultiple(List<BGCardDTO> cards)
    {
        // We loop the card.
        foreach (BGCardDTO card in cards)
        {
            // we get the index.
            int index = cards.IndexOf(card);

            // We draw a card.
            GameObject cardItem = Instantiate(GOCard, TRDeck);

            // We set the new card position.
            cardItem.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            cardItem.transform.SetParent(TRPrize);

            // We apply the target.
            CardController cardMoveItem = cardItem.GetComponent<CardController>();

            // We set the owner of card.
            cardMoveItem.SetCardMovement(this);

            // We set state as prize.
            cardMoveItem.SetState(BGCardStates.Prize);

            // We calculate the new position.
            Vector3 prizePosition = Vector3.zero;
            prizePosition.z -= 2 * index;

            // We load the card data.
            cardMoveItem.SetCardData(card);

            // We set the target.
            cardMoveItem.GoToTarget(prizePosition);

            // We add to cards.
            PlayerPrizes.Add(index, cardMoveItem);

            // We send to prizes content.
            yield return new WaitForSeconds(.2f);

            // We update the prize text.
            this.UpdatePlayerPrizeText();
        }
    }

    public IEnumerator MoveFromDiscardToHandMultiple(List<BGCardDTO> cards, Action onAllArrivedToDestination = null)
    {
        // if no card exists.
        if (cards.Count == 0)
        {
            // We invoke the listener.
            if (onAllArrivedToDestination != null)
                onAllArrivedToDestination.Invoke();

            // We break the method.
            yield break;
        }

        // We loop all the cards given.
        foreach (BGCardDTO card in cards)
        {
            // We have to wait to make smooth draw action.
            yield return new WaitForSeconds(.3f);

            // We draw the card.
            this.MoveFromDiscardToHand(card.UniqueCardID);
        }

        // We have to wait to make smooth draw action.
        yield return new WaitForSeconds(.3f);

        // if callback exists we will invoke it.
        if (onAllArrivedToDestination != null)
            onAllArrivedToDestination.Invoke();
    }

    public IEnumerator MoveFromHandToDiscardMultiple(IEnumerable<int> cards, BGCardDTO[] cardInfs, Action onAllArrivedToDestination = null)
    {
        // if no card exists.
        if (cards.Count() == 0)
        {
            // We invoke the listener.
            if (onAllArrivedToDestination != null)
                onAllArrivedToDestination.Invoke();

            // We break the method.
            yield break;
        }

        // We loop all the cards given.
        foreach (int cardID in cards)
        {
            // We have to wait to make smooth draw action.
            yield return new WaitForSeconds(.3f);

            // if contains informations just bind informations.
            if (cardInfs.Length > 0)
            {
                // We make sure it is in player hand.
                if (IsCardInHand(cardID))
                {
                    // We get the card info.
                    CardController cardInHand = GetCardInHand(cardID);

                    // We get the card data.
                    BGCardDTO cardData = cardInfs.FirstOrDefault(y => y.UniqueCardID == cardInHand.CardData.UniqueCardID);

                    // if exists we bind it.
                    if (cardData != null)
                        cardInHand.SetCardData(cardData);
                }
            }

            // We draw the card.
            this.MoveFromHandToDiscard(cardID);
        }

        // We have to wait to make smooth draw action.
        yield return new WaitForSeconds(.3f);

        // if callback exists we will invoke it.
        if (onAllArrivedToDestination != null)
            onAllArrivedToDestination.Invoke();
    }
    public IEnumerator MoveFromHandToDiscardMultiple(IEnumerable<int> cards, Action onAllArrivedToDestination = null)
    {
        // if no card exists.
        if (cards.Count() == 0)
        {
            // We invoke the listener.
            if (onAllArrivedToDestination != null)
                onAllArrivedToDestination.Invoke();

            // We break the method.
            yield break;
        }

        // We loop all the cards given.
        foreach (int cardID in cards)
        {
            // We have to wait to make smooth draw action.
            yield return new WaitForSeconds(.3f);

            // We draw the card.
            this.MoveFromHandToDiscard(cardID);
        }

        // We have to wait to make smooth draw action.
        yield return new WaitForSeconds(.3f);

        // if callback exists we will invoke it.
        if (onAllArrivedToDestination != null)
            onAllArrivedToDestination.Invoke();
    }
    public IEnumerator MoveFromHandToDeckMultiple(List<CardController> cards, Action onAllArrivedToDestination)
    {
        // if no card exists.
        if (cards.Count() == 0)
        {
            // We invoke the listener.
            if (onAllArrivedToDestination != null)
                onAllArrivedToDestination.Invoke();

            // We break the method.
            yield break;
        }

        // We loop all the cards given.
        foreach (CardController card in cards)
        {
            // We have to wait to make smooth draw action.
            yield return new WaitForSeconds(.3f);

            // We throw the card.
            this.MoveFromHandToDeck(card.CardData.UniqueCardID);
        }

        // We have to wait to make smooth draw action.
        yield return new WaitForSeconds(.3f);

        // if callback exists we will invoke it.
        if (onAllArrivedToDestination != null)
            onAllArrivedToDestination.Invoke();
    }

    #endregion

    #region SINGLE MOVES

    public bool MoveFromDiscardToHand(int uniqueCardId)
    {
        // if not in discard just return.
        if (!IsCardInDiscard(uniqueCardId)) return false;

        // We get the card in discard.
        CardController cardInDiscard = this.GetCardInDiscard(uniqueCardId);

        // We remove from the discard pile.
        this.RemoveFromDiscard(cardInDiscard);

        // We add to it hand.
        this.AddToHand(cardInDiscard);

        // We return true.
        return true;
    }
    public void MoveFromDiscardToBench(int uniqueCardId, Action onCardArrived)
    {
        // if not in discard just return.
        if (!IsCardInDiscard(uniqueCardId))
        {
            // if callback exists we just call it.
            if (onCardArrived != null)
                onCardArrived.Invoke();

            // We return.
            return;
        }

        // We get the card in discard.
        CardController cardInDiscard = GetCardInDiscard(uniqueCardId);

        // We remove from discard.
        this.RemoveFromDiscard(cardInDiscard);

        // We add card to the bench.
        this.AddToBench(cardInDiscard);

        // We invoke the callback.
        if (onCardArrived != null)
            onCardArrived.Invoke();
    }

    public bool MoveFromHandToDiscard(int uniqueCardId)
    {
        // if card is not in hand just return.
        if (!IsCardInHand(uniqueCardId))
            return false;

        // We get the first card.
        CardController card = GetCardInHand(uniqueCardId);

        // We also add to hand.
        this.RemoveFromHand(card);

        // We add to discard pile.
        this.AddToDiscard(card);

        // We return true its because succeed.
        return true;
    }
    public bool MoveFromHandToActive(CardController handItem)
    {
        // We make sure card is in player hand.
        if (!IsCardInHand(handItem.CardData.UniqueCardID)) return false;

        // if already exists an active monster return.
        if (this.PlayerActive != null) return false;

        // We remove from player hand.
        this.RemoveFromHand(handItem);

        // We remove from the benched list.
        this.AddToActive(handItem, null);

        if (this.IsRealPlayer)
            BattleNotiController.Instance.GOForceToSelectActiveCard.SetActive(false);
        else
            BattleNotiController.Instance.GOYourOpSelectingAMonster.SetActive(false);

        // We tell it is succeed.
        return true;
    }
    public bool MoveFromHandToBench(CardController handItem, int index = -1)
    {
        // Maximum benched quantity 5.
        if (PlayerBenched.Count >= 5) return false;

        // if card is not in hand return.
        if (handItem.CardState != BGCardStates.Hand) return false;

        // We remove from hand.
        this.RemoveFromHand(handItem);

        // We add card to bench.
        this.AddToBench(handItem, index);

        // We return true.
        return true;
    }
    public bool MoveFromHandToDeck(int uniqueCardId, Action onCardArrived = null)
    {
        // if not in hand just return.
        if (!IsCardInHand(uniqueCardId)) return false;

        // We get the card in hand.
        CardController cardInHand = GetCardInHand(uniqueCardId);

        // We remove from hand.
        this.RemoveFromHand(cardInHand);

        // We add card to hand.
        this.AddToDeck(cardInHand);

        // We bind the target.
        cardInHand.SetArriveAction(onCardArrived);

        // We tell it succeed.
        return true;
    }

    public bool MoveFromBenchToDiscard(int uniqueCardId)
    {
        // if card is not in hand just return.
        if (!IsCardInBench(uniqueCardId))
            return false;

        // We get the first card.
        CardController card = this.GetCardInBench(uniqueCardId);

        // We remove from bench.
        this.RemoveFromBench(card);

        // We add to discard pile.
        this.AddToDiscard(card);

        // We return true its because succeed.
        return true;
    }
    public bool MoveFromBenchToActive(CardController benchItem)
    {
        // We make sure the card in bench.
        if (!IsCardInBench(benchItem.CardData.UniqueCardID)) return false;

        // Cannot be a card if exists.
        if (PlayerActive != null) return false;

        // We remove from the benched list.
        this.RemoveFromBench(benchItem);

        // We remove from the benched list.
        this.AddToActive(benchItem, null);

        if (this.IsRealPlayer)
            BattleNotiController.Instance.GOForceToSelectActiveCard.SetActive(false);
        else
            BattleNotiController.Instance.GOYourOpSelectingAMonster.SetActive(false);

        // When succeed return true.
        return true;
    }

    public bool MoveFromHandToTrainer(CardController handItem, Action onArrivedToLocation = null)
    {
        // We make sure card is in player hand.
        if (!IsCardInHand(handItem.CardData.UniqueCardID)) return false;

        // if there is already another trainer.
        if (this.PlayerTrainer != null)
        {
            // We will throw it to discard pile.
            this.AddToDiscard(this.PlayerTrainer);

            // We clear trainer card.
            this.PlayerTrainer = null;
        }

        // We remove from the hand.
        this.RemoveFromHand(handItem);

        // We play trainer card.
        this.PlayTrainer(handItem);

        // We bind trigger.
        handItem.SetArriveAction(onArrivedToLocation);

        // We return as it succeed.
        return true;
    }

    #endregion

    #region SWITCH MOVES

    public bool SwitchBetweenActiveToBench(CardController activeCard, CardController benchItem, Action onArrivedLocation = null)
    {
        // if not in the bench list just return.
        if (!this.IsCardInBench(benchItem.CardData.UniqueCardID)) return false;

        // if no monster exists just return.
        if (!this.IsCardActive(activeCard.CardData.UniqueCardID)) return false;

        // We remove its special condition.
        activeCard.SetSpecialCondition(BGSpecialConditions.None);

        // We remove also shield.
        activeCard.SetShield(BGShieldTypes.None);

        // We get the index of benched.
        int benchIndex = this.PlayerBenched.IndexOf(benchItem);

        // We remove from bench without erase data.
        this.RemoveFromBench(benchItem);

        // We update the active card.
        this.AddToActive(benchItem, onArrivedLocation);

        // We add active card to bench.
        this.AddToBench(activeCard, benchIndex);

        // We return success.
        return true;
    }
    public void SwitchBenchWithNewOne(CardController older, CardController newOne, Action onArrivedTarget)
    {
        // We activate the card.
        newOne.gameObject.SetActive(true);

        // We get the old one index.
        int indexOfOld = this.PlayerBenched.IndexOf(older);

        // We switch between cards.
        this.AddToBench(newOne, indexOfOld);

        // We remove old one.
        this.RemoveFromBench(older);

        // We bind callback.
        newOne.SetArriveAction(onArrivedTarget);
    }

    public IEnumerator SwitchEnergyBetweenTwoCards(CardController currentCard, CardController targetCard, CardController energyCard)
    {
        // We detach from here.
        currentCard.CardEnergyAttachment.DetachCard(energyCard.CardData.UniqueCardID);

        // We attach to opponent.
        targetCard.CardEnergyAttachment.AttachAnEnergyToMonsterCard(energyCard);

        // We set as discard temporarly.
        energyCard.SetState(BGCardStates.Detached);

        // We attach the card.
        energyCard.transform.SetParent(targetCard.transform);

        // We set target to 0.
        energyCard.GoToTarget(Vector3.zero);

        // We activate the action.
        energyCard.gameObject.SetActive(true);

        // We wait some seconds.
        yield return new WaitUntil(() => energyCard.OnArrivedToTarget == null);

        // We set as discard temporarly.
        energyCard.SetState(BGCardStates.Attached);

        // We set target to 0.
        energyCard.GoToTarget(Vector3.zero);

        yield return new WaitUntil(() => energyCard.transform.localScale.x <= 0);

        // We deactivate the card.
        energyCard.gameObject.SetActive(false);
    }

    #endregion

}
