using System;
using UnityEngine;

namespace Assets.Scripts.GSSocket.DTO
{
    [Serializable]
    public class ConfigDTO
    {
        [Header("Game server ip address to connect.")]
        public string GameServerIP;
    }
}
