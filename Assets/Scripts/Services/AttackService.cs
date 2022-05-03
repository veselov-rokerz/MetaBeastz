using Assets.Scripts.GSSocket.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackService : MonoBehaviour
{
    public static AttackService Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void LoadSystemAttacks(Action<List<AttackDTO>> onLoaded = null)
    {
        GSController.Instance.SendToServer(GSMethods.SystemAttacks, string.Empty, (GSSocketResponse response) =>
        {
            // We get the system attacks.
            List<AttackDTO> attacks = response.GetDataList<AttackDTO>();

            // We return the attacks.
            if (onLoaded != null)
                onLoaded.Invoke(attacks);
        });
    }

    public AttackDTO GetAttackById(List<AttackDTO> attacks, int attackId) => attacks.Find(x => x.AttackId == attackId);
    public string GetAttackName(int attackId) => LocalizationController.Instance.GetLanguage($"Attack_{attackId}");
    public string GetAttackDescription(int attackId) => LocalizationController.Instance.GetLanguage($"Attack_Desc_{attackId}");
}
