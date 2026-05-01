using UnityEngine;

public enum EnemyActionType
{
    Attack,
    Special,
    Heal
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

        if (difficulty == EnemyDifficulty.Random)
        {
            attackScore = Random.Range(0f, 100f);
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

        return bestAction;
    }

    private float ScoreAttack(BattleUnit enemy, BattleUnit player)
    {
        float score = 50f;

        float playerLowHP = 1f - (float)player.currentHP / player.maxHP;
        float enemyLowHP = 1f - (float)enemy.currentHP / enemy.maxHP;
        bool enemyLowTP = (enemy.currentTP <= 6); 

        score += playerLowHP * 50f;
        score -= enemyLowHP * 50f;
        if(enemyLowTP) score += 25f;

        if (player.currentHP <= enemy.attackDamage)
            score = 100f;

        return score;
    }

    private float ScoreSpecial(BattleUnit enemy, BattleUnit player)
    {
        if(!enemy.CanUseSpecial())
            return -999f;
        
        float score = 50f;

        float playerLowHP = 1f - (float)player.currentHP / player.maxHP;
        float enemyLowHP = 1f - (float)enemy.currentHP / enemy.maxHP;

        score += (1f - playerLowHP) * 50f;
        score -= enemyLowHP * 50f;

        if (player.currentHP <= enemy.specialDamage && player.currentHP > enemy.attackDamage)
            score = 100f;

        if (player.currentHP <= enemy.attackDamage)
            score = 0f;

        return score;
    }

    private float ScoreHeal(BattleUnit enemy, BattleUnit player)
    {
        if(!enemy.CanHeal())
            return -999f;
        
        float score = 50f;

        float enemyLowHP = 1f - (float)enemy.currentHP / enemy.maxHP;

        score += enemyLowHP * 25f;

        if (enemy.currentHP <= player.attackDamage)
            score += 20f;

        if (enemy.currentHP + enemy.healAmount <= player.specialDamage)
            score -= 30f;
        
        if (enemy.currentHP + enemy.healAmount <= player.attackDamage)
            score = 0;

        return score;
    }
}