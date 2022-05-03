using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.BSSocket.Enums;
using System;
using TMPro;
using UnityEngine;

public class CardDamageController : MonoBehaviour
{
    [Header("Store the card informations.")]
    public CardController CardItem;

    [Header("Counter view with text.")]
    public GameObject GOCardCounter;

    [Header("When damage taken we will print the count.")]
    public TMP_Text TXTDamageCounter;

    [Header("Store the taken damage.")]
    public int TakenDamage;

    /// <summary>
    /// When damage taken enough it is dead.
    /// </summary>
    public bool IsDeath => TakenDamage >= CardItem.CardData.CardHp;

    public void ShowShieldedMessage()
    {
        // We text the counter.
        TXTDamageCounter.text = $"<color=#008080>Shielded</color>";
        TXTDamageCounter.gameObject.SetActive(false);
        TXTDamageCounter.gameObject.SetActive(true);
    }

    public void HitDamage(CardController attacker, int damage)
    {
        HitDamage(attacker, new DamageDTO { Damage = damage });
    }
    public void HitDamage(CardController attacker, DamageDTO damageData)
    {
        // We text the counter.
        if (damageData.Blocked)
            TXTDamageCounter.text = $"BLOCKED{Environment.NewLine}{damageData.Damage}";
        else if (damageData.IsResist)
            TXTDamageCounter.text = $"RESISTANCE{Environment.NewLine}{damageData.Damage}";
        else if (damageData.IsWeakness)
            TXTDamageCounter.text = $"WEAKNESS{Environment.NewLine}{damageData.Damage}";
        else
            TXTDamageCounter.text = $"{damageData.Damage}";

        TXTDamageCounter.gameObject.SetActive(false);
        TXTDamageCounter.gameObject.SetActive(true);

        // We add to taken damage.
        TakenDamage += damageData.Damage;

        // if this card is machamp then we will hit back.
        if (this.CardItem.SpecialCondition != BGSpecialConditions.Asleep &&
            this.CardItem.SpecialCondition != BGSpecialConditions.Confused &&
            this.CardItem.SpecialCondition != BGSpecialConditions.Paralyzed)
        {
            if (this.CardItem.CardData.MetaData.CardId == (int)BGAbilities.ABIStrikesBack)
            {
                // We maker sure it is exists or the attacker and defender is not in the same player.
                if (attacker != null && attacker != this.CardItem && attacker.Playground != this.CardItem.Playground)
                {
                    // We just hit 10 damage to opponent.
                    attacker.CardDamage.HitDamage(null, 10);
                }
            }
        }

        // We refresh the damage counter on card.
        RefreshDamageCounterOnCard();

        // if die we will return true.
        if (TakenDamage >= CardItem.CardData.MetaData.CardHp)
        {
            // We tell the knockout.
            StartCoroutine(CardItem.Playground.ToKnockout(this.CardItem, null));
        }
    }

    public void RecoverHealth(int heal)
    {
        // We get the recovery value.
        TakenDamage -= heal;

        // We clamp it to prevent minus.
        if (TakenDamage < 0)
            TakenDamage = 0;

        // We text the counter.
        TXTDamageCounter.text = $"<color=green>HEAL{Environment.NewLine}{heal}</color>";
        TXTDamageCounter.gameObject.SetActive(false);
        TXTDamageCounter.gameObject.SetActive(true);

        // We refresh the value.
        RefreshDamageCounterOnCard();
    }

    public void RemoveSpecialCondition()
    {
        // We remove the special condition.
        this.CardItem.SetSpecialCondition(BGSpecialConditions.None);

        // We text the counter.
        TXTDamageCounter.text = $"<color=#008080>SPECIAL CONDITION{Environment.NewLine}REMOVED</color>";

        TXTDamageCounter.gameObject.SetActive(false);
        TXTDamageCounter.gameObject.SetActive(true);
    }

    public void RecoverFullHealth()
    {
        // We get the recovery value.
        int recoveryValue = TakenDamage - CardItem.CardData.CardHp;

        // We text the counter.
        TXTDamageCounter.text = $"<color=green>{recoveryValue}</color>";
        TXTDamageCounter.gameObject.SetActive(false);
        TXTDamageCounter.gameObject.SetActive(true);

        // We add to taken damage.
        TakenDamage = 0;

        // We refresh the value.
        RefreshDamageCounterOnCard();
    }

    public void SetDamage(int takenDamage)
    {
        // We add to taken damage.
        TakenDamage = takenDamage;

        // We refresh the value.
        RefreshDamageCounterOnCard();
    }

    public void RefreshDamageCounterOnCard()
    {
        // We activate the counter.
        GOCardCounter.gameObject.SetActive(TakenDamage > 0);

        // We Print the text.
        GOCardCounter.GetComponentInChildren<TMP_Text>().text = $"{TakenDamage}";
    }

    public void ShowDamageIncreased()
    {
        // We text the counter.
        TXTDamageCounter.text = $"<color=orange>DAMAGE INCREASED</color>";
        TXTDamageCounter.gameObject.SetActive(false);
        TXTDamageCounter.gameObject.SetActive(true);

    }

    internal void ShowArmorIncreased()
    {
        // We text the counter.
        TXTDamageCounter.text = $"<color=orange>DAMAGE REDUCED</color>";
        TXTDamageCounter.gameObject.SetActive(false);
        TXTDamageCounter.gameObject.SetActive(true);
    }
}
