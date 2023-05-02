using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public int money;

    public void moneyIncrease(int amount)
    {
        money += amount;
    }
    public void moneyDecrease(int amount)
    {
        money -= amount;
    }
}