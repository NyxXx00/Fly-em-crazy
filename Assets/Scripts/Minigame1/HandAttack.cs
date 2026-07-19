using UnityEngine;

public class HandAttack : MonoBehaviour {
    [Header("Parameters")]
    [SerializeField] private bool autoSetLifetime = true;
    [SerializeField] private float timeUntilAttack = 1f;
    [SerializeField] private float lifeTime = 1.5f;

    [Header("Sprites")]
    [SerializeField] private Sprite handSprite;

    private Minigame1Manager minigameManager;
    private GameObject player;
    private SpriteRenderer spriteRenderer;

    private bool flyInHitbox;
    private bool attackExecuted;
    private bool playerWasHit;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        minigameManager =
            FindFirstObjectByType<Minigame1Manager>();

        player =
            GameObject.FindGameObjectWithTag("Player");

        if (spriteRenderer == null) {
            Debug.LogError(
                "HandAttack requires a SpriteRenderer.",
                gameObject
            );
        }

        if (minigameManager == null) {
            Debug.LogError(
                "Minigame1Manager was not found in the scene.",
                gameObject
            );
        }

        if (player == null) {
            Debug.LogError(
                "No GameObject with the Player tag was found.",
                gameObject
            );
        }
    }

    private void Start() {
        if (player == null || minigameManager == null) {
            Destroy(gameObject);
            return;
        }

        transform.position = player.transform.position;

        if (autoSetLifetime) {
            lifeTime = timeUntilAttack + 0.5f;
        }

        Destroy(gameObject, lifeTime);
    }

    private void Update() {
        if (attackExecuted || playerWasHit)
            return;

        timeUntilAttack -= Time.deltaTime;

        if (timeUntilAttack <= 0f) {
            Attack();
        }
    }

    private void Attack() {
        if (attackExecuted)
            return;

        attackExecuted = true;

        if (spriteRenderer != null &&
            handSprite != null) {
            spriteRenderer.sprite = handSprite;
        }

        if (flyInHitbox) {
            HitPlayer();
        }
    }

    private void HitPlayer() {
        if (playerWasHit)
            return;

        playerWasHit = true;

        if (minigameManager != null) {
            minigameManager.FlyWasHit();
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player"))
            return;

        flyInHitbox = true;

        if (attackExecuted) {
            HitPlayer();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Player"))
            return;

        flyInHitbox = true;

        if (attackExecuted) {
            HitPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            flyInHitbox = false;
        }
    }
}