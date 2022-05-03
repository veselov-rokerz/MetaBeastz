using Assets.SimpleLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationController : MonoBehaviour
{
    public static LocalizationController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // We read all the localizations.
        LocalizationManager.Read();

        // We change depends on the system language.
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Turkish:
                ChangeLanguage("Turkish");
                break;
            default:
                ChangeLanguage("English");
                break;
        }
    }

    public void ChangeLanguage(string language)
    {
        LocalizationManager.Language = language;
    }

    public string GetLanguage(string keyword, params object[] parameters) => LocalizationManager.Localize(keyword, parameters);

}
