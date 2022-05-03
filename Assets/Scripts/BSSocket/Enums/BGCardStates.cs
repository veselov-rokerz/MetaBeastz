using System;

namespace Assets.Scripts.BSSocket.Enums
{
    [Serializable]
    public enum BGCardStates
    {
        None,
        Deck,
        Discard,
        Hand,
        Bench,
        Active,
        Prize,
        Action,
        Attached,
        Knockout,
        Trainer,
        Detached,
        PrizeView
    }
}