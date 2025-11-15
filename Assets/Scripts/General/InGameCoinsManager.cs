using UnityEngine;
using TMPro;

public class InGameCoinsManager : MonoBehaviour
{
    public int coins;
    [SerializeField] TMP_Text coinsText;
    static public InGameCoinsManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void AddCoins(int coinsToAdd)
    {
        coins += coinsToAdd;
        HandleIU();
    }

    public void BuyWithCoins(int coinsToSubstract)
    {
        coins -= coinsToSubstract;
        HandleIU();
    }

    void HandleIU()
    {
        coinsText.text = coins.ToString();
    }
}
