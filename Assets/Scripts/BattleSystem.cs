//TODO:
//utility ai 2 difficulties
//choose difficulty at start page

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    [Header("Units")]
    public BattleUnit player;
    public BattleUnit enemy;
    public EnemyUtilityAI enemyAI;

    [Header("Battle Animators")]
    public Animator playerAnimator;
    public Animator enemyAnimator;

    [Header("Animation Timing")]
    public float attackAnimDelay = 0.35f;
    public float hurtAnimDelay = 0.2f;

    [Header("UI Text")]
    public TMP_Text playerHPText;
    public TMP_Text enemyHPText;
    public TMP_Text playerTPText;
    public TMP_Text enemyTPText;
    public TMP_Text narrationText;

    [Header("Buttons")]
    public Button attackButton;
    public Button specialButton;
    public Button healButton;
    public Button defendButton;

    private BattleState state;

    private enum BattleState
    {
        Start,
        PlayerTurn,
        Busy,
        EnemyTurn,
        Won,
        Lost
    }

    private enum ActionType
    {
        Attack,
        Special,
        Heal,
        Defend
    }

    private void Start()
    {
        player.ResetUnit();
        enemy.ResetUnit();

        RefreshUI();
        SetButtons(false);

        StartCoroutine(BeginBattle());
    }

    private IEnumerator BeginBattle()
    {
        state = BattleState.Start;
        narrationText.text ="Enemy appeared!";
        yield return new WaitForSeconds(2f);

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        state = BattleState.PlayerTurn;
        narrationText.text = "Choose an action.";
        SetButtons(true);
    }

    private void SetButtons(bool value)
    {
        attackButton.gameObject.SetActive(value);
        specialButton.gameObject.SetActive(value);
        healButton.gameObject.SetActive(value);
        defendButton.gameObject.SetActive(value);
    }

    private void RefreshUI()
    {
        playerHPText.text = "HP: " + player.currentHP;
        enemyHPText.text = "HP: " + enemy.currentHP;
        playerTPText.text = "TP: " + player.currentTP;
        enemyTPText.text = "TP: " + enemy.currentTP;
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerAction(ActionType.Attack));
    }

    public void OnSpecialButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerAction(ActionType.Special));
    }

    public void OnHealButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerAction(ActionType.Heal));
    }

    public void OnDefendButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerAction(ActionType.Defend));
    }

    private IEnumerator PlayerAction(ActionType action)
    {
        state = BattleState.Busy;
        SetButtons(false);

        if (action == ActionType.Attack)
        {
            playerAnimator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackAnimDelay);

            enemyAnimator.SetTrigger("Hurt");

            int damage = enemy.TakeDamage(player.attackDamage);
            narrationText.text = "You attack for " + damage + " damage!";

            RefreshUI();
            yield return new WaitForSeconds(hurtAnimDelay);
        }
        else if (action == ActionType.Special)
        {
            if (!player.CanUseSpecial())
            {
                narrationText.text = "Not enough TP for SPECIAL!";
                yield return new WaitForSeconds(2f);
                StartPlayerTurn();
                yield break;
            }

            player.SpendTP(player.specialCost);
            playerAnimator.SetTrigger("Special");
            yield return new WaitForSeconds(attackAnimDelay);

            enemyAnimator.SetTrigger("Hurt");

            int damage = enemy.TakeDamage(player.specialDamage);
            narrationText.text = "You use SPECIAL for " + damage + " damage!";

            RefreshUI();
            yield return new WaitForSeconds(hurtAnimDelay);
        }
        else if (action == ActionType.Heal)
        {
            if (!player.CanHeal())
            {
                narrationText.text = "Not enough TP to heal!";
                yield return new WaitForSeconds(2f);
                StartPlayerTurn();
                yield break;
            }
            
            player.SpendTP(player.healCost);
            int healed = player.Heal();
            narrationText.text = " You heal " + healed + " HP!";
            RefreshUI();
        }
        else if (action == ActionType.Defend)
        {
            player.Defend();
            narrationText.text = "You defend yourself against the next attack.";
            RefreshUI();
        }

        yield return new WaitForSeconds(2f);

        if (enemy.IsDead())
        {
            state = BattleState.Won;
            narrationText.text =  " Enemy was defeated!";
            yield break;
        }

        enemy.isDefending = false;
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        state = BattleState.EnemyTurn;
        narrationText.text = "Enemy's turn...";
        yield return new WaitForSeconds(2f);

        EnemyActionType choice = enemyAI.DecideAction(enemy, player);

        if (choice == EnemyActionType.Attack)
        {
            enemyAnimator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackAnimDelay);

            playerAnimator.SetTrigger("Hurt");

            int damage = player.TakeDamage(enemy.attackDamage);
            narrationText.text = "Enemy attacks with " + damage + " damage!";

            RefreshUI();
            yield return new WaitForSeconds(hurtAnimDelay);
        }
        else if (choice == EnemyActionType.Special)
        {
            enemyAnimator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackAnimDelay);

            playerAnimator.SetTrigger("Hurt");

            enemy.SpendTP(enemy.specialCost);
            int damage = player.TakeDamage(enemy.specialDamage);
            narrationText.text = "Enemy uses SPECIAL with " + damage + " damage!";

            RefreshUI();
            yield return new WaitForSeconds(hurtAnimDelay);
        }
        else if (choice == EnemyActionType.Heal)
        {
            enemy.SpendTP(enemy.healCost);
            int healed = enemy.Heal();
            narrationText.text = "Enemy heals " + healed + " HP!";
            RefreshUI();
        }
        else if (choice == EnemyActionType.Defend)
        {
            enemy.Defend();
            narrationText.text = "The enemy defended against your next attack.";
            RefreshUI();
        }

        yield return new WaitForSeconds(2f);

        if (player.IsDead())
        {
            state = BattleState.Lost;
            narrationText.text = "You were defeated!";
            yield break;
        }

        player.isDefending = false;
        StartPlayerTurn();
    }
}