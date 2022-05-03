using System;

namespace Assets.Scripts.BSSocket.Enums
{
    [Serializable]
    public enum BattleGameActions
    {
        Connect = 1,
        FlipCoinHeads = 2,
        FlipCoinTails = 3,
        PlayHandToActive = 4,
        PlayHandToBench = 5,
        DoneAction = 6,
        RevealCardsInActiveAndBench = 7,
        DrawACard = 8,
        AttachACardToCard = 9,
        EvolveAMonster = 10,
        RetreatAMonster = 11,
        AttackToOpponent = 12,
        PlayNewActiveMonster = 13,
        SelectAPrize = 14,
        GameIsOver = 15,
        AttackToOpponentEffect = 16,
        PlayTrainerCard = 17,
        PlayTrainerCardEffect = 18,
        Mulligan = 19,
        PlayAbility = 20,
        PlayAbilityEffect = 21,
        CancelAbility = 22,
    }
}
