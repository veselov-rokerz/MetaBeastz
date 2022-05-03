using System;

namespace Assets.Scripts.GSSocket.DTO
{
    [Serializable]
    public class LoginRequestDTO
    {
        public string Wallet;
    }

    [Serializable]
    public class LoginResponseDTO
    {
        public bool IsLoggedIn;
        public LoginErrors LoginError;
        public string RequestToken;
    }

    [Serializable]
    public enum LoginErrors
    {
        None,
        InValidWallet,
        UserBanned
    }
}
