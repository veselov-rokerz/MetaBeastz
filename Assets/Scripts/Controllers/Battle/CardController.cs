using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.Controllers.Attacks;
using Assets.Scripts.Controllers.Battle;
using Assets.Scripts.Controllers.Trainers.Trainers;
using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    /// <summary>
    /// Player card controller.
    /// </summary>
    public PlaygroundController Playground { get; set; }

    /// <summary>
    /// We have to set it is rotating.
    /// </summary>
    public bool AutoRotate { get; internal set; }

    /// <summary>
    /// Is this card dragging.
    /// </summary>
    public bool IsCardDragging { get; private set; }

    /// <summary>
    /// Evoluted monster will store its lower stage.
    /// </summary>
    public CardController AttachedLowStage { get; set; }

    /// <summary>
    /// Evoluted monster will store its higher stage.
    /// </summary>
    public CardController AttachedHighStage { get; set; }

    /// <summary>
    /// This card is attached to another card.
    /// Usage; Energy card is attached to a monster card.
    /// </summary>
    public CardController EnergyAttachedTo { get; set; }

    /// <summary>
    /// When a trainer card is attached to this card we will store it.
    /// </summary>
    public CardController AttachedTrainerCard { get; set; }

    /// <summary>
    /// When a monster evolve this state is going to be true to prevent multiple evolutions.
    /// Also when was the first time played it is going to be blocked.
    /// </summary>
    public bool EvolveBlocked { get; set; }

    /// <summary>
    /// We store the card informations.
    /// </summary>
    [Header("We store the card informations.")]
    public BGCardDTO CardData;

    [Header("Manages card damages.")]
    public CardDamageController CardDamage;

    [Header("Energies attached to this card.")]
    public CardEnergyAttachmentController CardEnergyAttachment;

    [Header("We manage retreat details.")]
    public CardRetreatController CardRetreat;

    [Header("Manages all the attacks.")]
    public CardAttackController CardAttack;

    [Header("Manages all the abilities.")]
    public CardAbilityController CardAbility;

    [Header("When any attack is going to block we will use this.")]
    public CardBlockAttackSelectController CardAttackBlocker;

    [Header("Going to activate when rotate enough.")]
    public GameObject GOActivatedObject;

    [Header("Going to deactivate when rotate enough")]
    public GameObject GODeactivatedObject;

    [Header("State of the card to make an action.")]
    public BGCardStates CardState;

    [Header("When card poisoned the damage it hit.")]
    public int PoisonDamage;

    [Header("When monster has a special condition it will be here.")]
    public BGSpecialConditions SpecialCondition;

    [Header("We use to set shield.")]
    public BGShieldTypes Shield;

    [Header("When attacking we will multiply value.")]
    public float SpeedMultiplier;

    [Header("When card poisoned we will show this.")]
    public GameObject GOPoisonMarker;

    [Header("When weakness energy modifed we will store it here.")]
    public Image IMGWeaknessEnergyType;

    [Header("When resistance modifed we will store it here.")]
    public Image IMGResistanceEnergyType;

    [Header("When selected outline will be shiny.")]
    public Outline Selection;

    /// <summary>
    /// When any target action arrived this action will work.
    /// </summary>
    public Action OnArrivedToTarget { get; private set; }

    /// <summary>
    /// Attack data that will store information in entire game when this card in game.
    /// When the card is discarded this value reset.
    /// </summary>
    public bool AttackDataPersistInPlay { get; set; }

    /// <summary>
    /// Attack data that will store information in for one turn in game.
    /// When the card is discarded this value reset.
    /// </summary>
    public bool PerTurnData { get; set; }

    /// <summary>
    /// When card hit by another card we will store the last damage.
    /// </summary>
    public BGAttackResponseDTO LastDamageData { get; set; }

    /// <summary>
    /// When card weakness has changed it will be stored here.
    /// </summary>
    public EnergyTypes WeaknessEnergyType { get; set; }

    /// <summary>
    /// When resistance has changed it will be stored here.
    /// </summary>
    public EnergyTypes ResistanceEnergyType { get; set; }

    /// <summary>
    /// When this card converted to an energy card we will store it here.
    /// </summary>
    public EnergyTypes TempEnergy { get; set; }

    /// <summary>
    /// When any action prevent to click on it.
    /// </summary>
    public bool IsNotClickable { get; set; }

    /// <summary>
    /// Returns the current left health.
    /// </summary>
    public int GetCurrentHealth => this.CardData.CardHp - this.CardDamage.TakenDamage;


    private Vector3 dragOffset;
    private RectTransform rectTransform;
    private Vector3 targetPosition;
    private Quaternion defaultRotation;

    private void Start()
    {
        // We make sure front is not activated.
        GOActivatedObject.SetActive(false);

        // We make sure backside is activated.
        GODeactivatedObject.SetActive(true);

        // Rect.
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        // We store the rotation.
        defaultRotation = rectTransform.rotation;
    }

    void Update()
    {
        // if holding we will drag the card..
        if (IsCardDragging)
        {
            // if not player turn return.
            if (!BattleGameController.Instance.IsPlayerTurn)
                return;

            // We get the mouse position for the ray.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // if hit something.
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // We make sure collider tag is ground.
                if (hit.collider.tag == "Ground")
                {
                    // We get the hit point.
                    Vector3 currentPosition = hit.point;

                    // We subtract the position to prevent offsets.
                    currentPosition.y -= dragOffset.y;
                    currentPosition.x -= dragOffset.x;
                    currentPosition.z -= dragOffset.z;

                    // We make it grow.
                    rectTransform.localScale = Vector3.one * Mathf.Clamp(rectTransform.localScale.x * 1.05f, 0, 1.5f);

                    // We move through the point.
                    rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, currentPosition, Playground.MoveSpeed * Time.deltaTime);
                }
            }

            // if we are holding we dont have to go further.
            return;
        }

        // We rescale the card.
        float scaleRate = 1;

        // if is benched then scale rate is going to be little bit shorter.
        switch (this.CardState)
        {
            case BGCardStates.Bench:
                scaleRate = .8f;
                break;
            case BGCardStates.Active:
                scaleRate = 1.25f;
                break;
            case BGCardStates.Attached:
                scaleRate = 0;
                break;
            case BGCardStates.PrizeView:
                scaleRate = 2;
                break;
        }

        // We make sure the scale of card is correct.
        if (rectTransform.localScale.x != scaleRate)
            rectTransform.localScale = Vector3.one * Mathf.Clamp(rectTransform.localScale.x * .95f, scaleRate, 1.5f);

        // if scale small enough just disable it.
        if (rectTransform.localScale.x <= 0)
            gameObject.SetActive(false);

        // We store position to change it.
        Vector3 tmpPosition = targetPosition;

        // if not clickable we will prevent to click.
        if (this.IsNotClickable)
        {
            if (this.Playground.IsRealPlayer)
                tmpPosition.y -= 20;
            else
                tmpPosition.y += 20;
        }

        // We move towards to target.
        rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, tmpPosition, Playground.MoveSpeed * SpeedMultiplier * Time.deltaTime);

        // if there is a listener on arrived action.
        if (OnArrivedToTarget != null)
        {
            // We call is arrived the destination. Then just invoke the listener.
            if (Vector3.Distance(rectTransform.anchoredPosition3D, tmpPosition) <= 20f)
            {
                // We call the method.
                OnArrivedToTarget.Invoke();

                // We remove the action to prevent multiple enterence.
                OnArrivedToTarget = null;

                // if cards state is deck we will destroy it.
                if (this.CardState == BGCardStates.Deck)
                    Destroy(gameObject);
            }
        }

        // if auto rotate is not activated just return.
        if (CardState != BGCardStates.Prize && CardState != BGCardStates.PrizeView)
        {
            if (!AutoRotate)
            {
                // We dont have to go further.
                return;
            }
        }

        // Target to rotate.
        Vector3 rotation = Playground.CardRotation;

        // We rotate card depends on status.
        switch (SpecialCondition)
        {
            case BGSpecialConditions.Asleep:
                rotation.z -= 90;
                break;
            case BGSpecialConditions.Confused:
                rotation.z += 180;
                break;
            case BGSpecialConditions.Paralyzed:
                rotation.z += 90;
                break;
        }

        // We rotate if required.
        switch (this.CardState)
        {
            case BGCardStates.Prize:
                {
                    // if real player we will flip.
                    if (this.Playground.IsRealPlayer)
                        rotation.y += 180;
                    else
                        rotation.y -= 180;

                    // We always rotate 90 degree.
                    rotation.z += 90;
                }
                break;
            case BGCardStates.Knockout:
                rotation.z += 45;
                break;
            case BGCardStates.PrizeView:
                rotation.y -= 180;
                rotation.x = this.transform.localRotation.x;
                break;
        }

        // We calculate the target.
        Quaternion target = Quaternion.Euler(rotation);

        // if not prize or prize view we will also rotate it to its default rotation.
        if (this.CardState != BGCardStates.Prize && this.CardState != BGCardStates.PrizeView)
        {
            // if backside active.
            if (this.CardData.MetaData.CardId == 0)
                target = defaultRotation;
        }

        // We rotate through the target.
        rectTransform.rotation = Quaternion.RotateTowards(rectTransform.rotation, target, Playground.RotationSpeed * Time.deltaTime);

        // if rotate enough we activate the frontside.
        if (rectTransform.rotation.eulerAngles.y <= 290)
        {
            if (this.CardData.MetaData.CardId > 0)
            {
                if (!GOActivatedObject.activeSelf)
                {
                    GOActivatedObject.SetActive(true);
                    GODeactivatedObject.SetActive(false);
                }
            }
            else
            {
                if (!GODeactivatedObject.activeSelf)
                {
                    GOActivatedObject.SetActive(false);
                    GODeactivatedObject.SetActive(true);
                }
            }
        }
    }

    public void SetInstantPosition(Vector3 position)
    {
        // Rect.
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        // We set the new card position.
        this.rectTransform.anchoredPosition3D = position;
    }
    public void GoToTarget(Vector3 position, Action onArrived = null)
    {
        // We set the target position.
        targetPosition = position;

        // We set the listener when arrived to destination.
        this.OnArrivedToTarget = onArrived;
    }
    public void SetArriveAction(Action callBack)
    {
        this.OnArrivedToTarget = callBack;
    }
    public void SetCardMovement(PlaygroundController playground) => this.Playground = playground;
    public void SetState(BGCardStates newState)
    {
        // We deselect the card.
        this.DeSelect();

        // if we have to rollback something before a card state change.
        switch (this.CardState)
        {
            case BGCardStates.Action:
                {
                    // if state changes then we will revert the action page.
                    if (newState != this.CardState)
                    {
                        // We also have to update old state to prevent stackoverflow.
                        this.CardState = newState;

                        // We deactivate action.
                        this.Playground.DeactivateAction();
                    }
                }
                break;
        }

        // Then we update the movement.
        this.CardState = newState;

        // After bind new state we will check auto rotation state.
        switch (this.CardState)
        {
            case BGCardStates.Prize:
            case BGCardStates.Action:
            case BGCardStates.PrizeView:
                // We disable auto rotation.
                this.AutoRotate = false;
                break;
            default:
                this.AutoRotate = true;
                break;
        }
    }
    public void SetCardData(BGCardDTO card)
    {
        // We set the card info.
        this.CardData = card;

        // Means card data should load.
        if (this.CardData.MetaData.CardId > 0)
        {
            switch (this.CardData.MetaData.CardTypeId)
            {
                case CardTypes.Pokemon:
                case CardTypes.Trainer:
                case CardTypes.Energy:
                    transform.GetComponentInChild<Image>("Front").sprite = ResourceController.Instance.GetCardSprite(this.CardData.MetaData.CardId);
                    break;
            }
        }
    }
    public void SetSpecialCondition(BGSpecialConditions condition, int poisonDamage = 0)
    {
        // We update the special condition.
        this.SpecialCondition = condition;

        // We always store the poison damage.
        this.PoisonDamage = poisonDamage;

        // if card is poisoned we will activate the marker.
        GOPoisonMarker.SetActive(SpecialCondition == BGSpecialConditions.Poisoned);
    }
    public void SetShield(BGShieldTypes shield)
    {
        // We activate the shield.
        this.Shield = shield;

        // if shielded we will just show the shielded message.
        if (this.Shield != BGShieldTypes.None)
            CardDamage.ShowShieldedMessage();
    }
    public void SetWeakness(EnergyTypes energy)
    {
        // We update the weakness.
        this.WeaknessEnergyType = energy;

        // We activate if other than none.
        IMGWeaknessEnergyType.gameObject.SetActive(this.WeaknessEnergyType != EnergyTypes.None);

        // We update its image.
        IMGWeaknessEnergyType.sprite = ResourceController.Instance.GetEnergyType(energy);
    }
    public void SetResistance(EnergyTypes energy)
    {
        // We update the resistance.
        this.ResistanceEnergyType = energy;

        // We activate if other than none.
        IMGResistanceEnergyType.gameObject.SetActive(this.ResistanceEnergyType != EnergyTypes.None);

        // We update its image.
        IMGResistanceEnergyType.sprite = ResourceController.Instance.GetEnergyType(energy);
    }
    public void SetClickState(bool isClickable)
    {
        IsNotClickable = isClickable;
    }
    public void SetAttachATrainerCard(CardController trainerCard)
    {
        // We update the attached trainer card.
        this.AttachedTrainerCard = trainerCard;

        // We execute action depends on trainer card.
        if (trainerCard != null)
        {
            switch ((BGTrainers)trainerCard.CardData.MetaData.CardId)
            {
                case BGTrainers.TRPlusPower:
                    {
                        // We tell to show damage increased message.
                        this.CardDamage.ShowDamageIncreased();
                    }
                    break;
                case BGTrainers.TRDefender:
                    {
                        // We tell to show damage increased message.
                        this.CardDamage.ShowArmorIncreased();
                    }
                    break;
            }
        }
    }
    public void ResetState()
    {
        // We clear evolution stages.
        this.AttachedLowStage = null;
        this.AttachedHighStage = null;

        // We clear attached trainer card.
        this.SetAttachATrainerCard(null);

        // We update the damage.
        this.CardDamage.SetDamage(0);

        // We set new special condition.
        this.SetSpecialCondition(BGSpecialConditions.None);

        // We detach all the energies.
        this.CardEnergyAttachment.DetachAll();

        // We reset the state.
        this.AttackDataPersistInPlay = false;

        /// We also clear per turn data.
        this.PerTurnData = false;

        // We clear the temp energy.
        this.TempEnergy = EnergyTypes.None;
    }

    public void Select()
    {
        // We add to selected list.
        if (!BattleGameController.Instance.SelectedCards.Contains(this))
            BattleGameController.Instance.SelectedCards.Add(this);

        // We set as selected.
        Selection.effectColor = Color.green;
    }
    public void DeSelect()
    {
        // We set as selected.
        Selection.effectColor = Color.black;
    }

    public void ActivateDetails()
    {
        // if not real player then we will deactivate all the actions.
        if (!this.Playground.IsRealPlayer || !BattleGameController.Instance.IsPlayerTurn)
        {
            // if player has to select opponent attack then.
            if (BattleNotiController.Instance.GOSelectYourOpponentAttack.activeSelf)
            {
                // We show the attacks to block.
                CardAttackBlocker.ShowAttacks(this);

                // We return.
                return;
            }

            // We deactivate all the interactable things.
            DeactivateDetails();

            // We dont have to go deeper.
            return;
        }

        // We load ability if exists.
        CardAbility.ShowAbilities(this);

        // if card not active just return.
        if (!this.Playground.IsCardActive(this.CardData.UniqueCardID)) return;

        // We load the reatreat informations.
        CardRetreat.LoadRetreat(this);

        // We load all the attack informations.
        CardAttack.ShowAttacks(this);
    }
    public void DeactivateDetails()
    {
        // We close the selection view.
        EnergyTypeSelectController.Instance.CloseSelectionView();

        // We hide all the attacks.
        CardAttackBlocker.HideAttacks();

        // We activate the retreat object.
        CardRetreat.HideRetreat();

        // We load all the attack informations.
        CardAttack.HideAttacks();

        // We load all the attack informations.
        CardAbility.HideAbilities();

        // We rollback all the energies.
        if (!CardRetreatEnergyDetachController.Instance.IsRetreating)
            CardRetreatEnergyDetachController.Instance.RollbackAllDetachedEnergiesAndClose();
        else
            CardRetreatEnergyDetachController.Instance.CloseDetachView();
    }

    public void OnClick()
    {
        // if the game is over just return.
        if (BattleGameController.Instance.IsGameOver) return;

        // if not clickable just return.
        if (this.IsNotClickable) return;

        // if opponent force to select a monster.
        if (BattleNotiController.Instance.GOSelectABenchedAMonster.activeSelf ||
            BattleNotiController.Instance.GOYourOpSelectingAMonster.activeSelf)
        {
            // if player selected to benched monster.
            if (BattleNotiController.Instance.GOSelectABenchedAMonster.activeSelf)
            {
                // if this is real player and selected card in their bench..
                if (this.Playground.IsRealPlayer && this.Playground.IsCardInBench(this.CardData.UniqueCardID))
                {
                    // We get the active attack data.
                    if (AttackController.Instance.ActiveAttack != null)
                    {
                        switch ((BGAttacks)AttackController.Instance.ActiveAttack.AttackData.AttackId)
                        {
                            case BGAttacks.ATKWhirlwind:
                                {
                                    // We get the whirlwind data.
                                    ATKWhirlwind whirlwind = (ATKWhirlwind)AttackController.Instance.ActiveAttack;

                                    // We update the target card id.
                                    whirlwind.RequestModel.TargetCardId = this.CardData.UniqueCardID;

                                    // We set the next action.
                                    AttackController.Instance.ActiveAttack.Attack(2);
                                }
                                break;
                            case BGAttacks.ATKPidgeottoWhirlwind:
                                {
                                    // We get the whirlwind data.
                                    ATKPidgeottoWhirlwind whirlwind = (ATKPidgeottoWhirlwind)AttackController.Instance.ActiveAttack;

                                    // We update the target card id.
                                    whirlwind.RequestModel.TargetCardId = this.CardData.UniqueCardID;

                                    // We set the next action.
                                    AttackController.Instance.ActiveAttack.Attack(2);
                                }
                                break;
                        }
                    }

                    // We get the active trainer card.
                    if (TrainerController.Instance.ActiveTrainer != null)
                    {
                        switch ((BGTrainers)TrainerController.Instance.ActiveTrainer.PlayedCard.CardData.MetaData.CardId)
                        {
                            case BGTrainers.TRSwitch:
                                {
                                    // We get the switch.
                                    TRSwitch sw = (TRSwitch)TrainerController.Instance.ActiveTrainer;

                                    // We update the data.
                                    sw.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We run the second action.
                                    TrainerController.Instance.ActiveTrainer.Play(false);
                                }
                                break;
                        }
                    }
                }
            }

            // We dont have to go below.
            return;
        }

        // if select a monster active we will force to select a monster from bench or active.
        if (BattleNotiController.Instance.GOSelectAMonster.activeSelf ||
            BattleNotiController.Instance.GOSelectAnotherMonster.activeSelf)
        {
            // We make sure it is real player.
            if (this.Playground.IsRealPlayer)
            {
                // We check it is in bench or active.
                if (this.Playground.IsCardInBench(this.CardData.UniqueCardID) ||
                    this.Playground.IsCardActive(this.CardData.UniqueCardID))
                {
                    // We get the active trainer card.
                    if (TrainerController.Instance.ActiveTrainer != null)
                    {
                        switch ((BGTrainers)TrainerController.Instance.ActiveTrainer.PlayedCard.CardData.MetaData.CardId)
                        {
                            case BGTrainers.TRPotion:
                                {
                                    // We get the potion.
                                    TRPotion p = (TRPotion)TrainerController.Instance.ActiveTrainer;

                                    // We update the data.
                                    p.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We run the second action.
                                    TrainerController.Instance.ActiveTrainer.Play(false);
                                }
                                break;
                            case BGTrainers.TRSuperPotion:
                                {
                                    // if no energy card exists just return.
                                    if (this.CardEnergyAttachment.AttachedEnergies.Count == 0) return;

                                    // We disable the select monster.
                                    BattleNotiController.Instance.GOSelectAMonster.SetActive(false);

                                    // We get the potion.
                                    TRSuperPotion sp = (TRSuperPotion)TrainerController.Instance.ActiveTrainer;

                                    // We update the data.
                                    sp.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We activate the view.
                                    this.Playground.ActivateAction(this);

                                    // We show the detach view.
                                    CardEnergyDetachController.Instance.ShowEnergyDetachViewForTrainer(this, sp.PlayedCard, true);
                                }
                                break;
                            case BGTrainers.TRSuperEnergyRemoval:
                                {
                                    // if no energy card exists just return.
                                    if (this.CardEnergyAttachment.AttachedEnergies.Count == 0) return;

                                    // We disable the select monster.
                                    BattleNotiController.Instance.GOSelectAMonster.SetActive(false);

                                    // We get the potion.
                                    TRSuperEnergyRemoval sp = (TRSuperEnergyRemoval)TrainerController.Instance.ActiveTrainer;

                                    // We update the data.
                                    sp.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We activate the view.
                                    this.Playground.ActivateAction(this);

                                    // We show the detach view.
                                    CardEnergyDetachController.Instance.ShowEnergyDetachViewForTrainer(this, sp.PlayedCard, true);
                                }
                                break;
                            case BGTrainers.TRDefender:
                                {
                                    // We get the potion.
                                    TRDefender p = (TRDefender)TrainerController.Instance.ActiveTrainer;

                                    // We update the data.
                                    p.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We run the second action.
                                    TrainerController.Instance.ActiveTrainer.Play(false);
                                }
                                break;
                            case BGTrainers.TRScoopUp:
                                {
                                    // We get the potion.
                                    TRScoopUp p = (TRScoopUp)TrainerController.Instance.ActiveTrainer;

                                    // We update the data.
                                    p.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We run the second action.
                                    TrainerController.Instance.ActiveTrainer.Play(false);
                                }
                                break;
                            case BGTrainers.TRDevolutionSpray:
                                {
                                    // We get the potion.
                                    TRDevolutionSpray p = (TRDevolutionSpray)TrainerController.Instance.ActiveTrainer;

                                    // We update the data.
                                    p.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We execute the action.
                                    p.Play(false);
                                }
                                break;
                        }
                    }

                    if (AbilityController.Instance.ActiveAbility != null)
                    {
                        switch ((BGAbilities)AbilityController.Instance.ActiveAbility.PlayedCard.CardData.MetaData.CardId)
                        {
                            case BGAbilities.ABIDamageSwap:
                                {
                                    // We are going to receive this card.
                                    if (AbilityController.Instance.ActiveAbility.RequestModel.TCardId == 0)
                                    {
                                        // if low hp cant select its because target unity may die.
                                        if (this.CardDamage.TakenDamage == 0) return;

                                        // We select the selected card.
                                        AbilityController.Instance.ActiveAbility.RequestModel.TCardId = this.CardData.UniqueCardID;

                                        // We set the heal value.
                                        AbilityController.Instance.ActiveAbility.RequestModel.TValue = 10;

                                        // We disable old alert.
                                        BattleNotiController.Instance.GOSelectAMonster.SetActive(false);

                                        // We enable new alert.
                                        BattleNotiController.Instance.GOSelectAnotherMonster.SetActive(true);
                                    }
                                    else
                                    {
                                        // if target card is not same with the old selection.
                                        if (AbilityController.Instance.ActiveAbility.RequestModel.TCardId != this.CardData.UniqueCardID)
                                        {
                                            // if low hp cant select its because target unity may die.
                                            if (this.GetCurrentHealth <= 10) return;

                                            // We select new card.
                                            AbilityController.Instance.ActiveAbility.RequestModel.SCardId = this.CardData.UniqueCardID;

                                            // We close alert.
                                            BattleNotiController.Instance.GOSelectAnotherMonster.SetActive(false);

                                            // We execute the action.
                                            AbilityController.Instance.ActiveAbility.Play(false);
                                        }
                                    }
                                }
                                break;
                            case BGAbilities.ABIEnergyTrans:
                                {
                                    // We are going to receive this card.
                                    if (AbilityController.Instance.ActiveAbility.RequestModel.TCardId == 0)
                                    {
                                        // if any energy not exists just return.
                                        if (!CardEnergyAttachment.AttachedEnergies.Exists(x => x.Item2.CardData.MetaData.EnergyTypeId == EnergyTypes.Grass))
                                            return;

                                        // We select the selected card.
                                        AbilityController.Instance.ActiveAbility.RequestModel.TCardId = this.CardData.UniqueCardID;

                                        // We disable old alert.
                                        BattleNotiController.Instance.GOSelectAMonster.SetActive(false);

                                        // We enable new alert.
                                        BattleNotiController.Instance.GOSelectAnotherMonster.SetActive(true);
                                    }
                                    else
                                    {
                                        // if target card is not same with the old selection.
                                        if (AbilityController.Instance.ActiveAbility.RequestModel.TCardId != this.CardData.UniqueCardID)
                                        {
                                            // We select new card.
                                            AbilityController.Instance.ActiveAbility.RequestModel.SCardId = this.CardData.UniqueCardID;

                                            // We close alert.
                                            BattleNotiController.Instance.GOSelectAnotherMonster.SetActive(false);

                                            // We execute the action.
                                            AbilityController.Instance.ActiveAbility.Play(false);
                                        }
                                    }
                                }
                                break;
                            case BGAbilities.ABIBuzzap:
                                {
                                    // if target card is not same with the old selection.
                                    if (AbilityController.Instance.ActiveAbility.RequestModel.CardID != this.CardData.UniqueCardID)
                                    {
                                        // We select new card.
                                        AbilityController.Instance.ActiveAbility.RequestModel.TCardId = this.CardData.UniqueCardID;

                                        // We close alert.
                                        BattleNotiController.Instance.GOSelectAnotherMonster.SetActive(false);

                                        // We execute the action.
                                        AbilityController.Instance.ActiveAbility.Play(false);
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            // We dont have to go below.
            return;
        }

        // if select a monster active we will force to select a monster from bench or active.
        if (BattleNotiController.Instance.GOSelectYourOpponentMonster.activeSelf)
        {
            // We make sure it is real player.
            if (!this.Playground.IsRealPlayer)
            {
                // We check it is in bench or active.
                if (this.Playground.IsCardInBench(this.CardData.UniqueCardID) ||
                    this.Playground.IsCardActive(this.CardData.UniqueCardID))
                {
                    // We get the active trainer card.
                    if (TrainerController.Instance.ActiveTrainer != null)
                    {
                        switch ((BGTrainers)TrainerController.Instance.ActiveTrainer.PlayedCard.CardData.MetaData.CardId)
                        {
                            case BGTrainers.TRSuperEnergyRemoval:
                                {
                                    // if no energy card exists just return.
                                    if (this.CardEnergyAttachment.AttachedEnergies.Count == 0) return;

                                    // We disable the select monster.
                                    BattleNotiController.Instance.GOSelectYourOpponentMonster.SetActive(false);

                                    // We get the energy removal.
                                    TRSuperEnergyRemoval sp = (TRSuperEnergyRemoval)TrainerController.Instance.ActiveTrainer;

                                    // We update the data.
                                    sp.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We activate the view.
                                    this.Playground.ActivateAction(this);

                                    // We show the detach view.
                                    CardEnergyDetachController.Instance.ShowEnergyDetachViewForTrainer(this, sp.PlayedCard, false);
                                }
                                break;
                            case BGTrainers.TREnergyRemoval:
                                {
                                    // if no energy card exists just return.
                                    if (this.CardEnergyAttachment.AttachedEnergies.Count == 0) return;

                                    // We disable the select monster.
                                    BattleNotiController.Instance.GOSelectYourOpponentMonster.SetActive(false);

                                    // We get the energy removal.
                                    TREnergyRemoval sp = (TREnergyRemoval)TrainerController.Instance.ActiveTrainer;

                                    // We update the data.
                                    sp.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We activate the view.
                                    this.Playground.ActivateAction(this);

                                    // We show the detach view.
                                    CardEnergyDetachController.Instance.ShowEnergyDetachViewForTrainer(this, sp.PlayedCard, false);
                                }
                                break;
                        }
                    }
                }
            }

            // We dont have to go below.
            return;
        }

        // if waiting a selection from player hand.
        if (BattleNotiController.Instance.GOSelect2CardFromYourHand.activeSelf ||
            BattleNotiController.Instance.GOSelect1CardFromYourHand.activeSelf)
        {
            // We make sure players own card and it is in player hand.
            if (this.Playground.IsRealPlayer && this.Playground.IsCardInHand(this.CardData.UniqueCardID))
            {
                // We get the active trainer card.
                if (TrainerController.Instance.ActiveTrainer != null)
                {
                    switch ((BGTrainers)TrainerController.Instance.ActiveTrainer.PlayedCard.CardData.MetaData.CardId)
                    {
                        case BGTrainers.TRComputerSearch:
                            {
                                // We get the switch.
                                TRComputerSearch cs = (TRComputerSearch)TrainerController.Instance.ActiveTrainer;

                                // if cards already selected just return.
                                if (cs.RequestModel.TCardIDs.Count == 2) return;

                                // We set card as selected.
                                cs.RequestModel.TCardIDs.Add(this.CardData.UniqueCardID);

                                // We set as selected.
                                this.Select();

                                // if card count 2 then we will send to server.
                                if (cs.RequestModel.TCardIDs.Count == 2)
                                {
                                    // We run the action.
                                    cs.Play(false);
                                }
                            }
                            break;
                        case BGTrainers.TRItemFinder:
                            {
                                // We get the switch.
                                TRItemFinder itemF = (TRItemFinder)TrainerController.Instance.ActiveTrainer;

                                // if cards already selected just return.
                                if (itemF.RequestModel.TCardIDs.Count == 2) return;

                                // We set card as selected.
                                itemF.RequestModel.TCardIDs.Add(this.CardData.UniqueCardID);

                                // We set as selected.
                                this.Select();

                                // if card count 2 then we will send to server.
                                if (itemF.RequestModel.TCardIDs.Count == 2)
                                {
                                    // We run the action.
                                    itemF.Play(false);
                                }
                            }
                            break;
                        case BGTrainers.TRMaintenance:
                            {
                                // We get the switch.
                                TRMaintenance itemF = (TRMaintenance)TrainerController.Instance.ActiveTrainer;

                                // if cards already selected just return.
                                if (itemF.RequestModel.TCardIDs.Count == 2) return;

                                // We set card as selected.
                                itemF.RequestModel.TCardIDs.Add(this.CardData.UniqueCardID);

                                // We set as selected.
                                this.Select();

                                // if card count 2 then we will send to server.
                                if (itemF.RequestModel.TCardIDs.Count == 2)
                                {
                                    // We run the action.
                                    itemF.Play(false);
                                }
                            }
                            break;
                        case BGTrainers.TREnergyRetrieval:
                            {
                                // We get the retrieval.
                                TREnergyRetrieval er = (TREnergyRetrieval)TrainerController.Instance.ActiveTrainer;

                                // We set card as selected.
                                er.RequestModel.TCardID = this.CardData.UniqueCardID;

                                // We set as selected.
                                this.Select();

                                // We run the action.
                                er.Play(false);
                            }
                            break;
                    }
                }
            }

            // We dont have to go below.
            return;
        }

        // if player can select opponent benched monster to switch it with active.
        if (BattleNotiController.Instance.GOSelectOpponentBenchMonster.activeSelf)
        {
            // We make sure it is not real player.
            if (!this.Playground.IsRealPlayer)
            {
                // if card is in bench.
                if (this.Playground.IsCardInBench(this.CardData.UniqueCardID))
                {
                    // We check for the active attack.
                    if (AttackController.Instance.ActiveAttack != null)
                    {
                        switch ((BGAttacks)AttackController.Instance.ActiveAttack.AttackData.AttackId)
                        {
                            case BGAttacks.ATKLure:
                                {
                                    // We cast to lure.
                                    ATKLure lure = (ATKLure)AttackController.Instance.ActiveAttack;

                                    // And we update the selected card.
                                    lure.RequestModel.TargetCardId = this.CardData.UniqueCardID;
                                }
                                break;
                        }
                    }

                    // if trainer active we run it.
                    if (TrainerController.Instance.ActiveTrainer != null)
                    {
                        switch ((BGTrainers)TrainerController.Instance.ActiveTrainer.PlayedCard.CardData.MetaData.CardId)
                        {
                            case BGTrainers.TRGustOfWind:
                                {
                                    // We get the switch.
                                    TRGustOfWind gow = (TRGustOfWind)TrainerController.Instance.ActiveTrainer;

                                    // We set card as selected.
                                    gow.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We run the action.
                                    gow.Play(false);

                                }
                                break;
                        }
                    }

                }
            }

            return;
        }

        // if select monster active we will ask for selection.
        if (BattleNotiController.Instance.GOSelect1MonsterCardFromYourHand.activeSelf)
        {
            // We make sure it is not real player.
            if (this.Playground.IsRealPlayer)
            {
                // if card is in hand.
                if (this.Playground.IsCardInHand(this.CardData.UniqueCardID))
                {
                    // if trainer active we run it.
                    if (TrainerController.Instance.ActiveTrainer != null)
                    {
                        switch ((BGTrainers)TrainerController.Instance.ActiveTrainer.PlayedCard.CardData.MetaData.CardId)
                        {
                            case BGTrainers.TRPokemonTraider:
                                {
                                    // if not a pokemon just return.
                                    if (this.CardData.MetaData.CardTypeId != CardTypes.Pokemon) return;

                                    // We get the trader.
                                    TRPokemonTrader pt = (TRPokemonTrader)TrainerController.Instance.ActiveTrainer;

                                    // We set card as selected.
                                    pt.RequestModel.TCardID = this.CardData.UniqueCardID;

                                    // We run the action.
                                    pt.Play(false);

                                }
                                break;
                        }
                    }
                }
            }

            return;
        }

        // if player waiting for the selection.
        if (Playground.WaitingPrizeQuantity > 0)
        {
            // if clicked card is prize.
            if (CardState == BGCardStates.PrizeView)
            {
                // We send to server
                BattleGameController.Instance.SendGameAction(BattleGameActions.SelectAPrize, new BGPlayCardRequestDTO
                {
                    UCardId = this.CardData.UniqueCardID
                });
            }

            // Player cant do any action.
            return;
        }

        // When player monster knockout we force to select a new monster.
        if (Playground.ForceToSelectActive)
        {
            // if not in bench just return.
            if (!Playground.IsCardInBench(this.CardData.UniqueCardID)) return;

            // We send to server
            BattleGameController.Instance.SendGameAction(BattleGameActions.PlayNewActiveMonster, new BGPlayCardRequestDTO
            {
                UCardId = this.CardData.UniqueCardID
            });

            return;
        }

        // By default player 
        switch (this.CardState)
        {
            case BGCardStates.Active:
                {
                    // We cant activate in first raund.
                    if (BattleGameController.Instance.IsFirstRound && !Playground.IsRealPlayer)
                        return;

                    // if the turn is player turn.
                    Playground.ActivateAction(this);
                }
                break;
            case BGCardStates.Bench:
                {
                    // if waiting for a switch benched monster.
                    if (CardRetreatEnergyDetachController.Instance.IsRetreating)
                        StartCoroutine(CardRetreatEnergyDetachController.Instance.SwitchTwoCard(this));
                    else // if just clicked on bench we just activate.
                        Playground.ActivateAction(this);
                }
                break;
        }
    }
    public void OnBeginDrag()
    {

        #region GENERAL CONDITIONS

        // if the game is over just return.
        if (BattleGameController.Instance.IsGameOver) return;

        // When a trainer card exists you cant play.
        if (TrainerController.Instance.ActiveTrainer != null) return;

        // Player cant drag and drop a card if player forced to select a new active monster.
        if (Playground.ForceToSelectActive)
        {
            // if trying to drag card not in bench.
            if (!Playground.IsCardInBench(this.CardData.UniqueCardID))
                return;
        }

        // if not player just return.
        if (!Playground.IsRealPlayer) return;

        // if not player turn just return.
        if (!BattleGameController.Instance.IsPlayerTurn) return;

        // if not rotated just return.
        if (!AutoRotate) return;

        // if already attached return.
        if (EnergyAttachedTo != null) return;

        #endregion

        #region BENCHED AND ACTIVE MONSTER CANT DRAG

        // if card is in bench just return.
        if (this.Playground.IsCardInBench(this.CardData.UniqueCardID)) return;

        // if card is active just return.
        if (this.Playground.IsCardActive(this.CardData.UniqueCardID)) return;

        #endregion

        #region ENERGY CARDS ROUNT 1 LIMITS

        // if energy card then can not be moved.
        if (this.CardData.MetaData.CardTypeId == CardTypes.Energy)
        {
            // The only condition is if it is the first round.
            if (BattleGameController.Instance.IsFirstRound)
                return;

            // if blastoise not active we just controll is valid or not.
            if (!AbilityController.Instance.IsAbilityActive(BGAbilities.ABIRainDance))
            {
                if (BattleGameController.Instance.IsAnEnergyAttached)
                    return;
            }
            else if (this.CardData.MetaData.EnergyTypeId != EnergyTypes.Water)
            {
                if (BattleGameController.Instance.IsAnEnergyAttached)
                    return;
            }
        }

        #endregion

        #region EVOLUTION LIMITATIONS

        // if this is an evolution card.
        if (this.CardData.MetaData.IsEvolutionCard)
        {
            // True when an evoluted card exists.
            bool canDrag = false;

            // if its target card evolution state is blocked just return.
            if (Playground.PlayerActive != null)
            {
                // if pokemon breeder active then we will force to select second stage monster.
                if (TrainerController.Instance.ActiveTrainer?.PlayedCard.CardData.MetaData.CardId == (int)BGTrainers.TRPokemonBreeder)
                {
                    // We make sure the card is evolution card of the current card.
                    if (CardMethods.GetSecondStageOfBasicCard(Playground.PlayerActive.CardData.MetaData.CardId)?.CardId == this.CardData.MetaData.CardId)
                    {
                        // And also we make sure it is blocked.
                        if (!Playground.PlayerActive.EvolveBlocked)
                            canDrag = true;
                    }
                }
                else // Otherwise we will just check for the card.
                {
                    if (Playground.PlayerActive.CardData.MetaData.CardId == this.CardData.MetaData.EvolutedCardId)
                    {
                        // And also we make sure it is blocked.
                        if (!Playground.PlayerActive.EvolveBlocked)
                            canDrag = true;
                    }
                }
            }

            // We check for the benched monster.
            foreach (CardController bench in Playground.PlayerBenched)
            {
                // if pokemon breeder active then we will force to select second stage monster.
                if (TrainerController.Instance.ActiveTrainer?.PlayedCard.CardData.MetaData.CardId == (int)BGTrainers.TRPokemonBreeder)
                {
                    // We make sure the card is evolution card of the current card.
                    if (CardMethods.GetSecondStageOfBasicCard(bench.CardData.MetaData.CardId)?.CardId == this.CardData.MetaData.CardId)
                    {
                        // And also we make sure it is blocked.
                        if (!bench.EvolveBlocked)
                            canDrag = true;
                    }
                }
                else // Otherwise we will just check for the card.
                {
                    if (bench.CardData.MetaData.CardId == this.CardData.MetaData.EvolutedCardId)
                    {
                        // And also we make sure it is blocked.
                        if (!bench.EvolveBlocked)
                            canDrag = true;
                    }
                }
            }

            // if no evolution monster exists just return.
            if (!canDrag)
                return;
        }

        #endregion

        #region Trainer Cards Condition

        // if energy car, we will check is playable.
        if (this.CardData.MetaData.CardTypeId == CardTypes.Trainer)
        {
            // You cant play trainer in first raund.
            if (BattleGameController.Instance.IsFirstRound) return;

            // if no benched and active monster exists.
            if (this.Playground.PlayerActive == null && this.Playground.PlayerBenched.Count == 0)
                return;

            // We get the action depends on card type.
            switch ((BGTrainers)this.CardData.MetaData.CardId)
            {
                // We will require card.
                case BGTrainers.TRComputerSearch:
                case BGTrainers.TRItemFinder:
                case BGTrainers.TRMaintenance:
                    {
                        // if no card exists you cant play energy retrieval
                        if (this.Playground.PlayerHand.Count <= 2)
                            return;
                    }
                    break;
                case BGTrainers.TREnergyRetrieval:
                    {
                        // if no card exists you cant play energy retrieval
                        if (this.Playground.PlayerHand.Count <= 1)
                            return;
                    }
                    break;
                case BGTrainers.TRGustOfWind:
                    {
                        // this action required a benched monster.
                        if (BattleGameController.Instance.GetOpponentOfPlayground(this.Playground).PlayerBenched.Count <= 0)
                            return;
                    }
                    break;
                case BGTrainers.TRSwitch:
                    {
                        // if player has not monster in bench just return.
                        if (this.Playground.PlayerBenched.Count <= 0)
                            return;
                    }
                    break;
                case BGTrainers.TRSuperPotion:
                    {
                        // Active card contains energy.
                        bool isActiveCardContainsEnergy = this.Playground.PlayerActive.CardEnergyAttachment.AttachedEnergies.Count > 0;

                        // We check benched cards are they contains energy.
                        bool isBenchedCardContainsEnergy = this.Playground.PlayerBenched.
                            Select(x => x.CardEnergyAttachment)
                            .Any(x => x.AttachedEnergies.Count > 0);

                        // if no energy exists in active and benched monster just return.
                        if (!isActiveCardContainsEnergy && !isBenchedCardContainsEnergy)
                            return;
                    }
                    break;
                case BGTrainers.TRSuperEnergyRemoval:
                    {
                        // Active card contains energy.
                        bool isActiveCardContainsEnergy = this.Playground.PlayerActive.CardEnergyAttachment.AttachedEnergies.Count > 0;

                        // We check benched cards are they contains energy.
                        bool isBenchedCardContainsEnergy = this.Playground.PlayerBenched.
                            Select(x => x.CardEnergyAttachment)
                            .Any(x => x.AttachedEnergies.Count > 0);

                        // if no energy exists in active and benched monster just return.
                        if (!isActiveCardContainsEnergy && !isBenchedCardContainsEnergy)
                            return;
                    }
                    break;
                case BGTrainers.TRRevive:
                    {
                        // if player has not monster in bench just return.
                        if (this.Playground.PlayerBenched.Count >= 5)
                            return;
                    }
                    break;
                case BGTrainers.TRPokemonFlute:
                    {
                        // if opponent player has too much monster in bench just return.
                        if (BattleGameController.Instance.GetOpponentOfPlayground(this.Playground).PlayerBenched.Count >= 5)
                            return;
                    }
                    break;
                case BGTrainers.TRPokemonBreeder:
                    {
                        // if true we will prevent the playing card.
                        bool isGoingToPrevent = true;

                        // We get the second evolution card of it.
                        var secondStageOfActivePlayer = CardMethods.GetSecondStageOfBasicCard(this.Playground.PlayerActive.CardData.MetaData.CardId);

                        // if evolution card exists.
                        if (secondStageOfActivePlayer != null)
                        {
                            // We make sure evolve is not blocked.
                            if (!this.Playground.PlayerActive.EvolveBlocked)
                            {
                                // if evolution card is in hand.
                                if (this.Playground.PlayerHand.Exists(y => y.CardData.MetaData.CardId == secondStageOfActivePlayer.CardId))
                                {
                                    // We wont prevent it.
                                    isGoingToPrevent = false;
                                }
                            }
                        }

                        // if still it is going to be prevented we can check the benched cards.
                        if (isGoingToPrevent)
                        {
                            // We loop benched monsters.
                            foreach (CardController benchedCards in this.Playground.PlayerBenched)
                            {
                                // We get the second evolution card of it.
                                var secondStageOfBench = CardMethods.GetSecondStageOfBasicCard(this.Playground.PlayerActive.CardData.MetaData.CardId);

                                // if evolution card exists.
                                if (secondStageOfBench != null)
                                {
                                    // We make sure evolve is not blocked.
                                    if (!benchedCards.EvolveBlocked)
                                    {
                                        // if evolution card is in hand.
                                        if (this.Playground.PlayerHand.Exists(y => y.CardData.MetaData.CardId == secondStageOfBench.CardId))
                                        {
                                            // We wont prevent it.
                                            isGoingToPrevent = false;

                                            // We dont have to go more.
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // if no exists second stage in player hand just return.
                        if (isGoingToPrevent) return;

                    }
                    break;
            }
        }

        #endregion

        // if exists we stop coroutine.
        OnMouseExit();

        // We set as holding.
        IsCardDragging = true;

        // We reset the offset value to prevent logic errors.
        dragOffset = Vector3.zero;

        // We update its sibling index to make card at the top most..
        transform.SetAsLastSibling();

        // Positions of the list.
        List<RectTransform> rects = GetComponentsInParent<RectTransform>().Where(x => x != rectTransform)
            .ToList();

        // We add positions to prevent position offsets.
        foreach (RectTransform rect in rects)
        {
            dragOffset = new Vector3(
                dragOffset.x + rect.anchoredPosition3D.x,
                dragOffset.y + rect.anchoredPosition3D.y,
                dragOffset.z + rect.anchoredPosition3D.z
            );
        }
    }
    public void OnEndDrag()
    {
        // We get the index of card in hand.
        int indexOfCard = Playground.PlayerHand.IndexOf(this);

        // We offset because there might be other cards.
        int offsetCard = transform.parent.childCount - Playground.PlayerHand.Count;

        // We update the sibling index to put it same order.
        transform.SetSiblingIndex(offsetCard + indexOfCard);

        // if card is not dragging just return.
        if (!IsCardDragging)
            return;

        // We tell we release the drag.
        IsCardDragging = false;

        // if the game is over just return.
        if (BattleGameController.Instance.IsGameOver) return;

        // Player cant drag and drop a card if player forced to select a new active monster.
        if (Playground.ForceToSelectActive)
        {
            // if this card is not inbench just return.
            if (!this.Playground.IsCardInBench(this.CardData.UniqueCardID))
                return;
        }

        // if not realy player just return.
        if (!Playground.IsRealPlayer) return;

        // if not player turn just return.
        if (!BattleGameController.Instance.IsPlayerTurn) return;

        // if not rotated just return.
        if (!AutoRotate) return;

        // if already attached return.
        if (EnergyAttachedTo != null) return;

        // We do the action the type of card.
        switch (this.CardData.MetaData.CardTypeId)
        {
            case CardTypes.Pokemon:
                {
                    #region if A Monster Card

                    // We get the mouse position on the scren.
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    // if hit something through the click point.
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        // We check is that placeable?
                        CardPlaceableAreaController placeable = hit.transform.GetComponent<CardPlaceableAreaController>();

                        // if place able.
                        if (placeable)
                        {
                            // We check the action we can handle.
                            switch (placeable.PlaceState)
                            {
                                case BGCardStates.Bench:
                                    {
                                        // You cant play if your active monster is null
                                        if (this.Playground.PlayerActive == null) return;

                                        // if already an active player exists.
                                        if (this.CardData.MetaData.IsEvolutionCard)
                                        {
                                            // In first round noone can evolve a card.
                                            if (BattleGameController.Instance.IsFirstRound) return;

                                            // We find a possible evolve card.
                                            CardController firstBeast = UnityExtends.GetHits()
                                                .Select(x => x.gameObject.GetComponent<CardController>())
                                                .Where(x => x != null && x != this && x.CardData.MetaData.CardTypeId == CardTypes.Pokemon)
                                                .FirstOrDefault();

                                            // if monster not exists return.
                                            if (firstBeast == null)
                                                return;

                                            // if monster blocked to evolve just return.
                                            if (firstBeast.EvolveBlocked)
                                                return;

                                            // can released?
                                            bool canRelease = false;

                                            // if pokemon breeder active then we will force to select second stage monster.
                                            if (TrainerController.Instance.ActiveTrainer?.PlayedCard.CardData.MetaData.CardId == (int)BGTrainers.TRPokemonBreeder)
                                            {
                                                // We make sure the card is evolution card of the current card.
                                                if (CardMethods.GetSecondStageOfBasicCard(firstBeast.CardData.MetaData.CardId)?.CardId == this.CardData.MetaData.CardId)
                                                {
                                                    // And also we make sure it is blocked.
                                                    if (!firstBeast.EvolveBlocked)
                                                    {
                                                        canRelease = true;

                                                        // We deactivate the alert.
                                                        BattleNotiController.Instance.GOSelectEvoluteOneOfYourMonstersToSecondStage.SetActive(false);
                                                    }
                                                }
                                            }
                                            else // Otherwise we will just check for the card.
                                            {
                                                if (firstBeast.CardData.MetaData.CardId == this.CardData.MetaData.EvolutedCardId)
                                                {
                                                    // And also we make sure it is blocked.
                                                    if (!firstBeast.EvolveBlocked)
                                                        canRelease = true;
                                                }
                                            }

                                            // if any condition not ready just return.
                                            if (!canRelease) return;

                                            // We evolve the active beasts.
                                            this.Playground.EvolveBenchedMonster(firstBeast, this);

                                            // We set as blocked to prevent multiple evolutions.
                                            this.EvolveBlocked = true;

                                            // We send to server evolve informations.
                                            SendToServerAsEvolved();

                                            // We dont have to go down.
                                            return;
                                        }

                                        // if not a basic pokemon you cant attach it.
                                        if (!this.CardData.MetaData.IsBasic)
                                            return;

                                        // We play card in player.
                                        if (Playground.MoveFromHandToBench(this))
                                        {
                                            // We block for multiple evolutions.
                                            this.EvolveBlocked = true;

                                            // We send to server.
                                            SendToServerHandToBench();
                                        }
                                    }
                                    break;
                                case BGCardStates.Active:

                                    // if card playing from bench.
                                    if (this.CardState == BGCardStates.Bench)
                                    {
                                        // if played as active.
                                        Playground.MoveFromBenchToActive(this);

                                        // We dont have to go below.
                                        return;
                                    }

                                    // if playing from hand to active.
                                    if (this.CardState == BGCardStates.Hand)
                                    {
                                        // if already an active player exists.
                                        if (this.CardData.MetaData.IsEvolutionCard)
                                        {
                                            // In first round noone can evolve a card.
                                            if (BattleGameController.Instance.IsFirstRound) return;

                                            // if active pokemon not exists return.
                                            if (this.Playground.PlayerActive == null)
                                                return;

                                            // if active card has block just return.
                                            if (this.Playground.PlayerActive.EvolveBlocked)
                                                return;

                                            // can released?
                                            bool canRelease = false;

                                            // if pokemon breeder active then we will force to select second stage monster.
                                            if (TrainerController.Instance.ActiveTrainer?.PlayedCard.CardData.MetaData.CardId == (int)BGTrainers.TRPokemonBreeder)
                                            {
                                                // We make sure the card is evolution card of the current card.
                                                if (CardMethods.GetSecondStageOfBasicCard(this.Playground.PlayerActive.CardData.MetaData.CardId)?.CardId == this.CardData.MetaData.CardId)
                                                {
                                                    // And also we make sure it is blocked.
                                                    if (!this.Playground.PlayerActive.EvolveBlocked)
                                                    {
                                                        canRelease = true;

                                                        // We deactivate the alert.
                                                        BattleNotiController.Instance.GOSelectEvoluteOneOfYourMonstersToSecondStage.SetActive(false);
                                                    }
                                                }
                                            }
                                            else // Otherwise we will just check for the card.
                                            {
                                                if (this.Playground.PlayerActive.CardData.MetaData.CardId == this.CardData.MetaData.EvolutedCardId)
                                                {
                                                    // And also we make sure it is blocked.
                                                    if (!this.Playground.PlayerActive.EvolveBlocked)
                                                        canRelease = true;
                                                }
                                            }

                                            // if any condition not ready just return.
                                            if (!canRelease) return;

                                            // We evolve the active beasts.
                                            this.Playground.EvolveActiveMonster(this);

                                            // We block the evolve to prevent multiple evolutions.
                                            this.EvolveBlocked = true;

                                            // We send to server evolve informations.
                                            SendToServerAsEvolved();

                                            // We dont have to go down.
                                            return;
                                        }

                                        // if not a basic pokemon you cant attach it.
                                        if (!this.CardData.MetaData.IsBasic)
                                            return;

                                        // We play hand to active.
                                        if (Playground.MoveFromHandToActive(this))
                                        {
                                            // When you play for the first time its evolve is going to be blocked.
                                            this.EvolveBlocked = true;

                                            // We send to server.
                                            SendToServerHandToActive();
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    #endregion
                }
                break;
            case CardTypes.Energy:
                {
                    // if an energy already attached just return.
                    if (BattleGameController.Instance.IsAnEnergyAttached) return;

                    // Cant attach any energy in first round.
                    if (BattleGameController.Instance.IsFirstRound) return;

                    // We find a possible attached card.
                    CardController firstAttachedType = UnityExtends.GetHits()
                        .Select(x => x.gameObject.GetComponent<CardController>())
                        .Where(x => x != null)
                        .Where(x => x != this)
                        .Where(x => x.CardData.MetaData.CardTypeId == CardTypes.Pokemon)
                        .Where(x => x.Playground.IsRealPlayer)
                        .Where(x => x.CardState == BGCardStates.Active || x.CardState == BGCardStates.Bench)
                        .Where(x => x.AttachedHighStage == null)
                        .FirstOrDefault();

                    // if exists.
                    if (firstAttachedType)
                    {
                        // We remove from hand.
                        this.Playground.AddToAttachmentsFromHand(this, firstAttachedType);

                        // We attach the card.
                        firstAttachedType.CardEnergyAttachment.AttachAnEnergyToMonsterCard(this);

                        // Set as attached.
                        this.EnergyAttachedTo = firstAttachedType;

                        // We set as energy attached to prevent dublicates.
                        // Blastoise has a special ability to attach more than one energy for only water
                        if (AbilityController.Instance.IsAbilityActive(BGAbilities.ABIRainDance))
                        {
                            // if not a water energy we will prevent.
                            if (this.CardData.MetaData.EnergyTypeId != EnergyTypes.Water)
                                BattleGameController.Instance.IsAnEnergyAttached = true;
                        }
                        else
                            BattleGameController.Instance.IsAnEnergyAttached = true;

                        // We notify the clients.
                        SendToServerAsAttached();
                    }
                }
                break;
            case CardTypes.Trainer:
                {
                    #region if A Trainer Card

                    // We get the mouse position on the scren.
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    // if hit something through the click point.
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        // We check is that placeable?
                        CardPlaceableAreaController placeable = hit.transform.GetComponent<CardPlaceableAreaController>();

                        // if place able.
                        if (placeable)
                        {
                            // We check the action we can handle.
                            switch (placeable.PlaceState)
                            {
                                case BGCardStates.Trainer:
                                case BGCardStates.Active:
                                case BGCardStates.Bench:
                                    {
                                        // if playing from hand to trainer.
                                        if (this.CardState == BGCardStates.Hand)
                                        {
                                            // We move card to trainer.
                                            this.Playground.MoveFromHandToTrainer(this);

                                            // We play the card.
                                            TrainerController.Instance.NewTrainer(this);
                                        }
                                        break;
                                    }
                            }
                        }
                    }

                    #endregion
                }
                break;
        }

        // if the card is not attached we have to rechecked its state its because mouse on it.
        OnMouseEnter();
    }

    public void OnMouseEnter()
    {
        // Only can be reveal if card is in hand or bench.
        if (this.CardState == BGCardStates.Hand ||
            this.CardState == BGCardStates.Bench ||
            this.CardState == BGCardStates.Trainer)
        {
            // And rotation must be activated.
            if (this.AutoRotate && this.CardData.MetaData.CardId > 0)
                CardDetailController.Instance.ShowCard(this);
        }
    }

    public void OnMouseExit()
    {
        CardDetailController.Instance.CloseShownCard();
    }

    #region Server Methods

    public void SendToServerAsEvolved()
    {
        BattleGameController.Instance.SendGameAction(BattleGameActions.EvolveAMonster, new BGEvolveMonsterDTO
        {
            EvUCardID = this.CardData.UniqueCardID,
            UCardID = this.AttachedLowStage.CardData.UniqueCardID
        });
    }
    public void SendToServerAsAttached()
    {
        BattleGameController.Instance.SendGameAction(BattleGameActions.AttachACardToCard, new BGCardAttachDTO
        {
            ECardID = this.CardData.UniqueCardID,
            MCardID = this.EnergyAttachedTo.CardData.UniqueCardID
        });
    }
    public void SendToServerHandToActive()
    {
        BattleGameController.Instance.SendGameAction(BattleGameActions.PlayHandToActive, new BGPlayCardRequestDTO
        {
            UCardId = this.CardData.UniqueCardID
        });
    }
    public void SendToServerHandToBench()
    {
        BattleGameController.Instance.SendGameAction(BattleGameActions.PlayHandToBench, new BGPlayCardRequestDTO
        {
            UCardId = this.CardData.UniqueCardID
        });
    }

    #endregion
}
