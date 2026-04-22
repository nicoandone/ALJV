using UnityEngine;

[System.Serializable]
public class BattleUnit
{
    public int maxHP = 100;
    public int currentHP;

    public int attackDamage = 10;
    public int specialDamage = 20;
    public int healAmount = 8;

    public void ResetUnit()
    {
        currentHP = maxHP;
    }

    public int TakeDamage(int amount)
    {
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

    public bool IsDead()
    {
        return currentHP == 0;
    }
}