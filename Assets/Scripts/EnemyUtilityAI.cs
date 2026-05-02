using UnityEngine;

public enum EnemyActionType
{
    Attack,
    Special,
    Heal,
    Defend
}

public enum EnemyDifficulty
{
    Random,
    Normal
}

public class EnemyUtilityAI : MonoBehaviour
{
    public EnemyDifficulty difficulty = EnemyDifficulty.Normal;

    private void SyncDifficulty()
    {
        if (GameSettings.selectedDifficulty == GameDifficulty.Easy)
            difficulty = EnemyDifficulty.Random;
        else
            difficulty = EnemyDifficulty.Normal;
    }
    
    public EnemyActionType DecideAction(BattleUnit enemy, BattleUnit player)
    {
        SyncDifficulty();

        float attackScore;
        float specialScore;
        float healScore;
        float defendScore;

        if (difficulty == EnemyDifficulty.Random)
        {
            attackScore = Random.Range(0f, 100f);
            defendScore = Random.Range(0f, 100f);
            if(enemy.CanUseSpecial())
                specialScore = Random.Range(0f, 100f);
            else
                specialScore = -999f;

            if(enemy.CanHeal())
                healScore = Random.Range(0f, 100f);
            else
                healScore = -999f;
        }
        else
        {
            attackScore = ScoreAttack(enemy, player);
            specialScore = ScoreSpecial(enemy, player);
            healScore = ScoreHeal(enemy, player);
            defendScore = ScoreDefend(enemy, player);
        }

        float bestScore = attackScore;
        EnemyActionType bestAction = EnemyActionType.Attack;

        if (specialScore > bestScore)
        {
            bestScore = specialScore;
            bestAction = EnemyActionType.Special;
        }

        if (healScore > bestScore)
        {
            bestScore = healScore;
            bestAction = EnemyActionType.Heal;
        }

        if (defendScore > bestScore)
        {
            bestScore = defendScore;
            bestAction = EnemyActionType.Defend;
        }

        return bestAction;
    }

    private float ScoreAttack(BattleUnit enemy, BattleUnit player)
    {
        int enemyDealDamage = enemy.attackDamage;
        if (player.isDefending) enemyDealDamage = Mathf.CeilToInt(enemyDealDamage * 0.5f); 
        
        float score = 60f;

        float playerLowHP = 1f - (float)player.currentHP / player.maxHP;
        float enemyLowHP = 1f - (float)enemy.currentHP / enemy.maxHP;

        score += playerLowHP * 50f;
        score -= enemyLowHP * 20f;
        if(player.isDefending) score -= 15f;

        if (player.currentHP <= enemyDealDamage)
                return 100f;

        return score;
    }

    private float ScoreSpecial(BattleUnit enemy, BattleUnit player)
    {
        if(!enemy.CanUseSpecial())
            return -999f;

        int enemyDealNormalDamage = enemy.attackDamage;
        if (player.isDefending) enemyDealNormalDamage = Mathf.CeilToInt(enemyDealNormalDamage * 0.5f);

        int enemyDealSpecialDamage = enemy.specialDamage;
        if (player.isDefending) enemyDealSpecialDamage = Mathf.CeilToInt(enemyDealSpecialDamage * 0.5f);
        
        float score = 65f;

        float playerLowHP = 1f - (float)player.currentHP / player.maxHP;
        float enemyLowHP = 1f - (float)enemy.currentHP / enemy.maxHP;

        score += playerLowHP * 50f;
        score -= enemyLowHP * 20f;
        if(player.isDefending) score -= 15f;

        if (player.currentHP <= enemyDealSpecialDamage && player.currentHP > enemyDealNormalDamage)
            return 100f;

        if (player.currentHP <= enemyDealNormalDamage)
            return 0f;

        return score;
    }

    private float ScoreHeal(BattleUnit enemy, BattleUnit player)
    {
        if(!enemy.CanHeal())
            return -999f;
        
        float score = 40f;

        float enemyLowHP = 1f - (float)enemy.currentHP / enemy.maxHP;

        score += enemyLowHP * 25f;

        if (enemy.currentHP <= player.attackDamage)
            score += 20f;

        if (enemy.currentHP + enemy.healAmount <= player.specialDamage)
            score -= 30f;
        
        if (enemy.currentHP + enemy.healAmount <= player.attackDamage)
            score -= 30;

        //rules for when enemy would decide to defend instead of heal => compute hp profit

        return score;
    }

    private float ScoreDefend(BattleUnit enemy, BattleUnit player)
    {
        float score = 50f;

        float enemyLowHP = 1f - (float)enemy.currentHP / enemy.maxHP;
        bool enemyLowTP = enemy.currentTP <= 6; 

        if(enemyLowTP) score += 40f;
        
        score += enemyLowHP * 20f;

        if (player.attackDamage >= enemy.currentHP)
            score += 30f;

        if (enemy.CanUseSpecial() || enemy.CanHeal())
            score -= 10f;

        if (enemy.isDefending)
            score -= 30f;
        
        //rules for when enemy would decide to heal instead of defend => compute hp profit

        return score;
    }
}