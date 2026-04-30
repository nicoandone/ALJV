using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class EnemyEncounter : MonoBehaviour
{
    public Animator playerAnimator;
    private bool encounterStarted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (encounterStarted) return;

        if (other.CompareTag("player"))
        {
            StartCoroutine(Encounter(other));
        }
    }

    private IEnumerator Encounter(Collider2D other)
    {
        encounterStarted = true;

        PlayerController playerController = other.GetComponent<PlayerController>();
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        playerAnimator.SetBool("isWalking", false);
        playerAnimator.SetBool("isJumping", false);
        playerAnimator.SetTrigger("Encounter");
        
        playerController.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }
}