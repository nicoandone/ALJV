//TODO:
//utility ai 2 difficulties
//choose difficulty at start page

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
    public TMP_Text narrationText;

    [Header("Buttons")]
    public Button attackButton;
    public Button specialButton;
    public Button healButton;

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
        Heal
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
    }

    private void RefreshUI()
    {
        playerHPText.text = "HP: " + player.currentHP;
        enemyHPText.text = "HP: " + enemy.currentHP;
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
            playerAnimator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackAnimDelay);

            enemyAnimator.SetTrigger("Hurt");

            int damage = enemy.TakeDamage(player.specialDamage);
            narrationText.text = "You use SPECIAL for " + damage + " damage!";

            RefreshUI();
            yield return new WaitForSeconds(hurtAnimDelay);
        }
        else if (action == ActionType.Heal)
        {
            int healed = player.Heal();
            narrationText.text = " You heal " + healed + " HP!";
            RefreshUI();
        }

        yield return new WaitForSeconds(2f);

        if (enemy.IsDead())
        {
            state = BattleState.Won;
            narrationText.text =  " Enemy was defeated!";
            yield break;
        }

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

            int damage = player.TakeDamage(enemy.specialDamage);
            narrationText.text = "Enemy uses SPECIAL with " + damage + " damage!";

            RefreshUI();
            yield return new WaitForSeconds(hurtAnimDelay);
        }
        else
        {
            int healed = enemy.Heal();
            narrationText.text = "Enemy heals " + healed + " HP!";
            RefreshUI();
        }

        yield return new WaitForSeconds(2f);

        if (player.IsDead())
        {
            state = BattleState.Lost;
            narrationText.text = "You were defeated!";
            yield break;
        }

        StartPlayerTurn();
    }
}