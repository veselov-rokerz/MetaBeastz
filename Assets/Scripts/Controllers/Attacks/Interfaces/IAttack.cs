using Assets.Scripts.BSSocket.DTO;
using Assets.Scripts.GSSocket.DTO;
using System;

namespace Assets.Scripts.Controllers.Attacks
{
    public interface IAttack
    {
        string UniqueID { get; set; }
        CardController CardData { get; set; }
        AttackDTO AttackData { get; set; }
        BGAttackRequestDTO RequestModel { get; set; }
        void Simulate(BGAttackResponseDTO response, Action onSimulationCompleted);
        void Attack(int actionIndex);
        void Generate();
    }
}
