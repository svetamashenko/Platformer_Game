using UnityEngine;

public class CoinCounterComponent : MonoBehaviour
{
    [SerializeField] private int _coinBalance;

    public void AddToBalance(int coinValue)
    {
        _coinBalance += coinValue;
        ShowBalance();
    }

    public void ShowBalance()
    {
        Debug.Log($"Player has {_coinBalance} currency.");
    }
}
