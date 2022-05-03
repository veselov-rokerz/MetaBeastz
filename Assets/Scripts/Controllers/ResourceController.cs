using Assets.Scripts.GSSocket.DTO;
using System.Linq;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public static ResourceController Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Sprite GetCardTemplate(EnergyTypes et) => Resources.Load<Sprite>($"CardTemplates/{et}");
    public Sprite GetCardSprite(int cardId) => Resources.Load<Sprite>($"Cards/Card_{cardId}");
    public Sprite GetEnergyType(EnergyTypes et) => Resources.LoadAll<Sprite>($"Energies").FirstOrDefault(x=> x.name == $"{et}");
    public Sprite GetDeckSprite(int deckId) => Resources.Load<Sprite>($"Decks/Deck_{deckId}");

}
