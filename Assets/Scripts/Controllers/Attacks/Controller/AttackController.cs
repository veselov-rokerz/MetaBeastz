using Assets.Scripts.BSSocket.Enums;
using Assets.Scripts.Controllers.Attacks;
using Assets.Scripts.GSSocket.DTO;
using System;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public static AttackController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Current active attack.
    /// </summary>
    public IAttack ActiveAttack { get; private set; }

    public void NewAttack(CardController attacker, AttackDTO attack, bool isCopy = false)
    {
        // if already exists just destroy it.
        if (ActiveAttack != null)
            Destroy(((MonoBehaviour)ActiveAttack).gameObject);

        // We get the type of comp.
        Type type = GetAttack((BGAttacks)attack.AttackId);

        // We get the component.
        Component attackItem = new GameObject("Attack").AddComponent(type);

        // We changed the basic attack.
        ActiveAttack = (IAttack)attackItem;

        // We load the default values.
        BaseATK baseAttack = attackItem.GetComponent<BaseATK>();

        // When play any trainer card.
        BattleNotiController.Instance.GODone.SetActive(false);

        // if not a copy load.
        if (!isCopy)
            baseAttack.LoadData(attacker, attack);
    }

    public Type GetAttack(BGAttacks attackId)
    {
        // We create a new attack.
        switch (attackId)
        {
            default:
                return null;
            case BGAttacks.None:
                return null;
            case BGAttacks.ATKScratch:
                return typeof(ATKScratch);
            case BGAttacks.ATKEmber:
                return typeof(ATKEmber);
            case BGAttacks.ATKLeechSeed:
                return typeof(ATKLeechSeed);
            case BGAttacks.ATKPsyshock:
                return typeof(ATKPsyshock);
            case BGAttacks.ATKDig:
                return typeof(ATKDig);
            case BGAttacks.ATKMudSlap:
                return typeof(ATKMudSlap);
            case BGAttacks.ATKSleepingGas:
                return typeof(ATKSleepingGas);
            case BGAttacks.ATKDestinyBond:
                return typeof(ATKDestinyBond);
            case BGAttacks.ATKWhirlwind:
                return typeof(ATKWhirlwind);
            case BGAttacks.ATKHornHazard:
                return typeof(ATKHornHazard);
            case BGAttacks.ATKBubble:
                return typeof(ATKBubble);
            case BGAttacks.ATKWithdraw:
                return typeof(ATKWithdraw);
            case BGAttacks.ATKJab:
                return typeof(ATKJab);
            case BGAttacks.ATKSpecialPunch:
                return typeof(ATKSpecialPunch);
            case BGAttacks.ATKRaichuAgility:
                return typeof(ATKRaichuAgility);
            case BGAttacks.ATKRaichuThunder:
                return typeof(ATKRaichuThunder);
            case BGAttacks.ATKCharmeleonSlash:
                return typeof(ATKCharmeleonSlash);
            case BGAttacks.ATKCharmeleonFlamethrower:
                return typeof(ATKCharmeleonFlamethrower);
            case BGAttacks.ATKFireSpin:
                return typeof(ATKFireSpin);
            case BGAttacks.ATKPidgeottoWhirlwind:
                return typeof(ATKPidgeottoWhirlwind);
            case BGAttacks.ATKMirrorMove:
                return typeof(ATKMirrorMove);
            case BGAttacks.ATKPsychic:
                return typeof(ATKPsychic);
            case BGAttacks.ATKBarrier:
                return typeof(ATKBarrier);
            case BGAttacks.ATKWarTortleWithdraw:
                return typeof(ATKWarTortleWithdraw);
            case BGAttacks.ATKWarTortleBite:
                return typeof(ATKWarTortleBite);
            case BGAttacks.ATKHydroPump:
                return typeof(ATKHydroPump);
            case BGAttacks.ATKVineWhip:
                return typeof(ATKVineWhip);
            case BGAttacks.ATKPoisonpowder:
                return typeof(ATKPoisonpowder);
            case BGAttacks.ATKSolarbeam:
                return typeof(ATKSolarbeam);
            case BGAttacks.ATKConfuseRay:
                return typeof(ATKConfuseRay);
            case BGAttacks.ATKDragonRage:
                return typeof(ATKDragonRage);
            case BGAttacks.ATKBubblebeam:
                return typeof(ATKBubblebeam);
            case BGAttacks.ATKThunderWave:
                return typeof(ATKThunderWave);
            case BGAttacks.ATKSelfdestruct:
                return typeof(ATKSelfdestruct);
            case BGAttacks.ATKMagnemiteThunderWave:
                return typeof(ATKMagnemiteThunderWave);
            case BGAttacks.ATKMagnemiteSelfdestruct:
                return typeof(ATKMagnemiteSelfdestruct);
            case BGAttacks.ATKFuryAttack:
                return typeof(ATKFuryAttack);
            case BGAttacks.ATKHeadbutt:
                return typeof(ATKHeadbutt);
            case BGAttacks.ATKPoisonSting:
                return typeof(ATKPoisonSting);
            case BGAttacks.ATKVulpixConfuseRay:
                return typeof(ATKVulpixConfuseRay);
            case BGAttacks.ATKBite:
                return typeof(ATKBite);
            case BGAttacks.ATKTackle:
                return typeof(ATKTackle);
            case BGAttacks.ATKThunder:
                return typeof(ATKThunder);
            case BGAttacks.ATKThunderbolt:
                return typeof(ATKThunderbolt);
            case BGAttacks.ATKTwineedle:
                return typeof(ATKTwineedle);
            case BGAttacks.ATKBeedrillPoisonSting:
                return typeof(ATKBeedrillPoisonSting);
            case BGAttacks.ATKSlap:
                return typeof(ATKSlap);
            case BGAttacks.ATKFoulGas:
                return typeof(ATKFoulGas);
            case BGAttacks.ATKLowKick:
                return typeof(ATKLowKick);
            case BGAttacks.ATKMagikarpTackle:
                return typeof(ATKMagikarpTackle);
            case BGAttacks.ATKFlail:
                return typeof(ATKFlail);
            case BGAttacks.ATKGnaw:
                return typeof(ATKGnaw);
            case BGAttacks.ATKThunderJolt:
                return typeof(ATKThunderJolt);
            case BGAttacks.ATKWaterGun:
                return typeof(ATKWaterGun);
            case BGAttacks.ATKStringShot:
                return typeof(ATKStringShot);
            case BGAttacks.ATKFlare:
                return typeof(ATKFlare);
            case BGAttacks.ATKSmashKick:
                return typeof(ATKSmashKick);
            case BGAttacks.ATKFlameTail:
                return typeof(ATKFlameTail);
            case BGAttacks.ATKPound:
                return typeof(ATKPound);
            case BGAttacks.ATKDrowzeePound:
                return typeof(ATKDrowzeePound);
            case BGAttacks.ATKDrowzeeConfuseRay:
                return typeof(ATKDrowzeeConfuseRay);
            case BGAttacks.ATKSlash:
                return typeof(ATKSlash);
            case BGAttacks.ATKEarthquake:
                return typeof(ATKEarthquake);
            case BGAttacks.ATKThundershock:
                return typeof(ATKThundershock);
            case BGAttacks.ATKThunderpunch:
                return typeof(ATKThunderpunch);
            case BGAttacks.ATKElectricShock:
                return typeof(ATKElectricShock);
            case BGAttacks.ATKAuroraBeam:
                return typeof(ATKAuroraBeam);
            case BGAttacks.ATKIceBeam:
                return typeof(ATKIceBeam);
            case BGAttacks.ATKDoubleslap:
                return typeof(ATKDoubleslap);
            case BGAttacks.ATKMeditate:
                return typeof(ATKMeditate);
            case BGAttacks.ATKRecover:
                return typeof(ATKRecover);
            case BGAttacks.ATKSuperPsy:
                return typeof(ATKSuperPsy);
            case BGAttacks.ATKSeismicToss:
                return typeof(ATKSeismicToss);
            case BGAttacks.ATKStarmieRecover:
                return typeof(ATKStarmieRecover);
            case BGAttacks.ATKStarFreeze:
                return typeof(ATKStarFreeze);
            case BGAttacks.ATKTangelaBind:
                return typeof(ATKTangelaBind);
            case BGAttacks.ATKTangelaPoisonpowder:
                return typeof(ATKTangelaPoisonpowder);
            case BGAttacks.ATKStiffen:
                return typeof(ATKStiffen);
            case BGAttacks.ATKStunSpore:
                return typeof(ATKStunSpore);
            case BGAttacks.ATKRaticateBite:
                return typeof(ATKRaticateBite);
            case BGAttacks.ATKRaticateSuperFang:
                return typeof(ATKRaticateSuperFang);
            case BGAttacks.ATKFlamethrower:
                return typeof(ATKFlamethrower);
            case BGAttacks.ATKTakeDown:
                return typeof(ATKTakeDown);
            case BGAttacks.ATKScrunch:
                return typeof(ATKScrunch);
            case BGAttacks.ATKDoubleedge:
                return typeof(ATKDoubleedge);
            case BGAttacks.ATKFirePunch:
                return typeof(ATKFirePunch);
            case BGAttacks.ATKMagmarFlamethrower:
                return typeof(ATKMagmarFlamethrower);
            case BGAttacks.ATKDoubleKick:
                return typeof(ATKDoubleKick);
            case BGAttacks.ATKHornDrill:
                return typeof(ATKHornDrill);
            case BGAttacks.ATKKakunaStiffen:
                return typeof(ATKKakunaStiffen);
            case BGAttacks.ATKKakunaPoisonpowder:
                return typeof(ATKKakunaPoisonpowder);
            case BGAttacks.ATKKarateChop:
                return typeof(ATKKarateChop);
            case BGAttacks.ATKSubmission:
                return typeof(ATKSubmission);
            case BGAttacks.ATKHypnosis:
                return typeof(ATKHypnosis);
            case BGAttacks.ATKDreamEater:
                return typeof(ATKDreamEater);
            case BGAttacks.ATKThrash:
                return typeof(ATKThrash);
            case BGAttacks.ATKToxic:
                return typeof(ATKToxic);
            case BGAttacks.ATKRockThrow:
                return typeof(ATKRockThrow);
            case BGAttacks.ATKLeekSlap:
                return typeof(ATKLeekSlap);
            case BGAttacks.ATKPotSmash:
                return typeof(ATKPotSmash);
            case BGAttacks.ATKSandAttack:
                return typeof(ATKSandAttack);
            case BGAttacks.ATKHarden:
                return typeof(ATKHarden);
            case BGAttacks.ATKLure:
                return typeof(ATKLure);
            case BGAttacks.ATKFireBlast:
                return typeof(ATKFireBlast);
            case BGAttacks.ATKPoliwrathWaterGun:
                return typeof(ATKPoliwrathWaterGun);
            case BGAttacks.ATKWhirlpool:
                return typeof(ATKWhirlpool);
            case BGAttacks.ATKSlam:
                return typeof(ATKSlam);
            case BGAttacks.ATKHyperBeam:
                return typeof(ATKHyperBeam);
            case BGAttacks.ATKAmnesia:
                return typeof(ATKAmnesia);
            case BGAttacks.ATKPoliwhirlDoubleslap:
                return typeof(ATKPoliwhirlDoubleslap);
            case BGAttacks.ATKSing:
                return typeof(ATKSing);
            case BGAttacks.ATKMetronome:
                return typeof(ATKMetronome);
            case BGAttacks.ATKConversion1:
                return typeof(ATKConversion1);
            case BGAttacks.ATKConversion2:
                return typeof(ATKConversion2);
        }
    }

    public bool IsAttackActive(string uniqueId) => ActiveAttack?.UniqueID == uniqueId;
    public bool IsAttackActive(BGAttacks attack) => (BGAttacks)ActiveAttack?.AttackData?.AttackId == attack;

    public void ClearAttack()
    {
        this.ActiveAttack = null;
    }
}
