using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.Controllers.Attacks;
using Assets.Scripts.GSSocket.DTO;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnergyTypeSelectController : MonoBehaviour
{
    public static EnergyTypeSelectController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// We store the card item.
    /// </summary>
    public CardController CardItem { get; private set; }

    /// <summary>
    /// Attack information.
    /// </summary>
    public AttackDTO AttackItem { get; private set; }

    public bool IsAbility { get; private set; }

    [Header("Energy selection view.")]
    public GameObject GOEnergySelectView;

    [Header("We will use the energy item to instantiate.")]
    public GameObject GOEnergyItem;

    [Header("We will store the energies instantiated.")]
    public Transform TREnergyContent;

    public void ShowEnergySelectViewForAbility(CardController card, params EnergyTypes[] ignored)
    {
        // We set as ability.
        this.IsAbility = true;

        // We show the selection view.
        ShowEnergySelectView(card, ignored);
    }

    public void ShowEnergySelectView(CardController card, AttackDTO attack, params EnergyTypes[] ignored)
    {
        // We bind the attack.
        this.AttackItem = attack;

        // We show the selection view.
        ShowEnergySelectView(card, ignored);
    }

    private void ShowEnergySelectView(CardController card, params EnergyTypes[] ignored)
    {
        // We activate the energy selection view.
        GOEnergySelectView.SetActive(true);

        // We bind the card.
        this.CardItem = card;

        // We clear older items.
        TREnergyContent.RemoveAllChildsOfTransform();

        // We loop all the energies.
        foreach (EnergyTypes energy in Enum.GetValues(typeof(EnergyTypes)))
        {
            // Energy if none skip.
            if (energy == EnergyTypes.None) continue;

            // if ignored skip.
            if (ignored.Contains(energy)) continue;

            // We create energy.
            GameObject energyItem = Instantiate(GOEnergyItem, TREnergyContent);

            // We set the image.
            energyItem.GetComponent<Image>().sprite = ResourceController.Instance.GetEnergyType(energy);

            // When click on it we will do the action.
            energyItem.GetComponent<Button>().onClick.AddListener(() => OnClickEnergy(energy));
        }
    }

    private void OnClickEnergy(EnergyTypes energy)
    {
        // if no active attack just return.
        if (AttackController.Instance.ActiveAttack != null)
        {
            // We make the action depend on attack.
            switch ((BGAttacks)AttackController.Instance.ActiveAttack.AttackData.AttackId)
            {
                case BGAttacks.ATKConversion1:
                    {
                        // We get the item.
                        ATKConversion1 atkConversion = (ATKConversion1)AttackController.Instance.ActiveAttack;

                        // We do the manual attack.
                        atkConversion.ManualAttack(energy);
                    }
                    break;
                case BGAttacks.ATKConversion2:
                    {
                        // We get the item.
                        ATKConversion2 atkConversion = (ATKConversion2)AttackController.Instance.ActiveAttack;

                        // We do the manual attack.
                        atkConversion.ManualAttack(energy);
                    }
                    break;
            }
        }

        if (AbilityController.Instance.ActiveAbility != null)
        {
            switch ((BGAbilities)this.CardItem.CardData.MetaData.CardId)
            {
                case BGAbilities.ABIBuzzap:
                    {
                        // We set an energy.
                        AbilityController.Instance.ActiveAbility.RequestModel.Energy = energy;

                        // We have to select a target.
                        BattleNotiController.Instance.GOSelectAnotherMonster.SetActive(true);

                        // We deactivate the action.
                        this.CardItem.Playground.DeactivateAction();
                    }
                    break;
            }
        }
    }

    public void CloseSelectionView()
    {
        // We deactivate the energy selection view.
        GOEnergySelectView.SetActive(false);
    }
}
