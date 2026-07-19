using System.Collections;
using UnityEngine;

public class DirectionalHandAttack : MonoBehaviour {
    [Header("Attack Timing")]
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private float handDuration = 0.5f;

    [Header("Sprites")]
    [SerializeField] private Sprite warningSprite;
    [SerializeField] private Sprite handSprite;

    private SpriteRenderer spriteRenderer;

    private DirectionalManager manager;
    private GameObject player;

    private bool flyInHitbox;
    private bool handIsActive;
    private bool attackFinished;
    private bool playerWasHit;

    private DirectionalManager.AttackDirection attackDirection;

    private void Awake() {
        spriteRenderer =
            GetComponent<SpriteRenderer>();
    }

    public void Initialize(
        DirectionalManager newManager,
        GameObject newPlayer,
        DirectionalManager.AttackDirection direction
    ) {
        manager = newManager;
        player = newPlayer;
        attackDirection = direction;

        ConfigureDirection();

        StartCoroutine(AttackSequence());
    }

    private void ConfigureDirection() {
        switch (attackDirection) {
            case DirectionalManager.AttackDirection.Top:
                break;

            case DirectionalManager.AttackDirection.Bottom:
                break;

            case DirectionalManager.AttackDirection.Left:
                break;

            case DirectionalManager.AttackDirection.Right:
                break;
        }
    }

    private IEnumerator AttackSequence() {
        ShowWarning();

        yield return new WaitForSeconds(
            warningDuration
        );

        ActivateHand();

        yield return new WaitForSeconds(
            handDuration
        );

        FinishAttack();
    }

    private void ShowWarning() {
        handIsActive = false;

        if (spriteRenderer != null &&
            warningSprite != null) {
            spriteRenderer.sprite =
                warningSprite;
        }
    }

    private void ActivateHand() {
        if (playerWasHit)
            return;

        handIsActive = true;

        if (spriteRenderer != null &&
            handSprite != null) {
            spriteRenderer.sprite =
                handSprite;
        }

        // The fly was already inside when the hand appeared.
        if (flyInHitbox) {
            KillPlayer();
        }
    }

    private void KillPlayer() {
        if (playerWasHit)
            return;

        playerWasHit = true;
        handIsActive = false;

        if (manager != null) {
            manager.PlayerHit();
        }
    }

    private void FinishAttack() {
        if (attackFinished || playerWasHit)
            return;

        attackFinished = true;
        handIsActive = false;

        if (manager != null) {
            manager.AttackFinished(this);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player"))
            return;

        flyInHitbox = true;


        if (handIsActive) {
            KillPlayer();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Player"))
            return;

        flyInHitbox = true;

        if (handIsActive) {
            KillPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            flyInHitbox = false;
        }
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }
}