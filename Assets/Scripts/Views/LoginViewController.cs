using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginViewController : MonoBehaviour
{
    [Header("Wallet address is going to read when you login.")]
    public TMP_InputField INPWalletAddress;

    [Header("Login button to send wallet to server.")]
    public Button BTNLogin;

    public void OnClickLogin()
    {
        // Get the address.
        string walletAddress = INPWalletAddress.text.Trim();

        // We try to login.
        LoginController.Instance.LoginToServer(walletAddress, (isSucceed) =>
        {
            // We disable the view.
            gameObject.SetActive(false);
        });
    }

    public void LoginWithWallet(string walletCode)
    {
        INPWalletAddress.text = walletCode;
    }
}
