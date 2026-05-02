using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

[System.Serializable]
public class BattleUnit
{
    public int maxHP = 100;
    public int currentHP;

    public int maxTP = 20;
    public int currentTP;

    public int attackDamage = 10;
    public int specialDamage = 15;
    public int healAmount = 8;

    public int defendTPGain = 4;
    public int specialCost = 6;
    public int healCost = 5;

    public bool isDefending = false;

    public void ResetUnit()
    {
        currentHP = maxHP;
        currentTP = maxTP;
        isDefending = false;
    }

    public int TakeDamage(int amount)
    {
        if (isDefending) {
            amount = Mathf.CeilToInt(amount * 0.5f);
            isDefending = false;
        }

        int oldHP = currentHP;
        currentHP = Mathf.Max(0, currentHP - amount);
        return oldHP - currentHP;
    }

    public int Heal()
    {
        int oldHP = currentHP;
        currentHP = Mathf.Min(maxHP, currentHP + healAmount);
        return currentHP - oldHP;
    }

    public void GainTP(int amount)
    {
        int oldTP = currentTP;
        currentTP = Mathf.Min(maxTP, currentTP + amount);
    }

    public void SpendTP(int amount)
    {
        currentTP -= amount;
    }

    public void Defend()
    {
        isDefending = true;
        GainTP(defendTPGain);
    }

    public bool CanUseSpecial()
    {
        return currentTP >= specialCost;
    }

    public bool CanHeal()
    {
        return currentTP >= healCost;
    }

    public bool IsDead()
    {
        return currentHP == 0;
    }
}