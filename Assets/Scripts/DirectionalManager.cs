using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DirectionalManager : MonoBehaviour {

    private bool playerInStressZone;

    [Header("Stress")]
    [SerializeField] private Slider stressBar;
    [SerializeField] private float timeUntilDecay = 2f;
    [SerializeField] private float stressIncreaseSpeed = 0.01f;
    [SerializeField] private float stressDecaySpeed = 0.01f;

    private float stressAmount;
    private float decayTimer;

    [Header("Game Objects")]
    [SerializeField] private GameObject handAttackPrefab;
    [SerializeField] private GameObject person;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject finalScreen;

    [Header("Attack Positions")]
    [SerializeField] private Transform attackFromTop;
    [SerializeField] private Transform attackFromBottom;
    [SerializeField] private Transform attackFromLeft;
    [SerializeField] private Transform attackFromRight;

    [Header("Person Portraits")]
    [SerializeField] private Sprite[] personPortraits;

    private SpriteRenderer personSpriteRenderer;

    [Header("Attack Timing")]
    [SerializeField] private float initialTimeBetweenAttacks = 2.5f;
    [SerializeField] private float minimumTimeBetweenAttacks = 0.8f;
    [SerializeField] private float consecutiveAttackDelay = 0.25f;

    [Header("Scene Change")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float delayBeforeReturningToMenu = 0.5f;

    private float attackTimer;
    private bool attackPatternActive;
    private bool gameFinished;

    private readonly List<DirectionalHandAttack> activeAttacks =
        new List<DirectionalHandAttack>();

    public enum AttackDirection {
        Top,
        Bottom,
        Left,
        Right
    }

    private void Start() {
        if (person != null) {
            personSpriteRenderer =
                person.GetComponent<SpriteRenderer>();
        }

        stressAmount = 0f;
        decayTimer = timeUntilDecay;
        attackTimer = initialTimeBetweenAttacks;

        if (stressBar != null) {
            stressBar.minValue = 0f;
            stressBar.maxValue = 1f;
            stressBar.value = stressAmount;
        }

        if (finalScreen != null) {
            finalScreen.SetActive(false);
        }
    }

    private void Update() {
        if (gameFinished)
            return;

        UpdateStress();
        UpdatePersonPortrait();
        UpdateAttackTimer();

        if (stressAmount >= 1f) {
            ShowFinalScreen(false);
        }
    }

    private void UpdateStress() {
        decayTimer -= Time.deltaTime;

        if (decayTimer <= 0f) {
            stressAmount -= stressDecaySpeed * Time.deltaTime;
        }

        stressAmount = Mathf.Clamp01(stressAmount);

        if (stressBar != null) {
            stressBar.value = stressAmount;
        }
    }

    private void UpdatePersonPortrait() {
        if (personSpriteRenderer == null ||
            personPortraits == null ||
            personPortraits.Length == 0) {
            return;
        }

        int portraitIndex = Mathf.FloorToInt(
            stressAmount * personPortraits.Length
        );

        portraitIndex = Mathf.Clamp(
            portraitIndex,
            0,
            personPortraits.Length - 1
        );

        personSpriteRenderer.sprite =
            personPortraits[portraitIndex];
    }

    private void UpdateAttackTimer() {
        if (!playerInStressZone) {
            attackTimer = initialTimeBetweenAttacks;
            return;
        }

        if (attackPatternActive)
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f) {
            StartCoroutine(PlayAttackPattern());
        }
    }

    private IEnumerator PlayAttackPattern() {
        attackPatternActive = true;

        float currentStress = stressAmount;

        if (currentStress < 0.30f) {
            // Level 1: one attack.
            SpawnRandomAttack();

            yield return new WaitUntil(
                () => activeAttacks.Count == 0 || gameFinished
            );
        }
        else if (currentStress < 0.60f) {
            // Level 2: two consecutive attacks.
            SpawnRandomAttack();

            yield return new WaitUntil(
                () => activeAttacks.Count == 0 || gameFinished
            );

            if (gameFinished)
                yield break;

            yield return new WaitForSeconds(
                consecutiveAttackDelay
            );

            SpawnRandomAttack();

            yield return new WaitUntil(
                () => activeAttacks.Count == 0 || gameFinished
            );
        }
        else if (currentStress < 0.85f) {
            // Level 3: two simultaneous attacks.
            SpawnTwoSimultaneousAttacks();

            yield return new WaitUntil(
                () => activeAttacks.Count == 0 || gameFinished
            );
        }
        else {
            // Level 4: two simultaneous attacks,
            // followed by one extra attack.
            SpawnTwoSimultaneousAttacks();

            yield return new WaitUntil(
                () => activeAttacks.Count == 0 || gameFinished
            );

            if (gameFinished)
                yield break;

            yield return new WaitForSeconds(
                consecutiveAttackDelay
            );

            SpawnRandomAttack();

            yield return new WaitUntil(
                () => activeAttacks.Count == 0 || gameFinished
            );
        }

        if (gameFinished)
            yield break;

        attackTimer = Mathf.Lerp(
            initialTimeBetweenAttacks,
            minimumTimeBetweenAttacks,
            stressAmount
        );

        attackPatternActive = false;
    }

    private void SpawnRandomAttack() {
        AttackDirection direction =
            GetRandomDirection();

        SpawnAttack(direction);
    }

    private void SpawnTwoSimultaneousAttacks() {
        AttackDirection firstDirection =
            GetRandomDirection();

        AttackDirection secondDirection =
            GetDifferentDirection(firstDirection);

        SpawnAttack(firstDirection);
        SpawnAttack(secondDirection);
    }

    private void SpawnAttack(AttackDirection direction) {
        if (handAttackPrefab == null) {
            Debug.LogWarning(
                "Hand Attack Prefab is not assigned."
            );

            return;
        }

        Transform spawnPoint =
            GetAttackPoint(direction);

        if (spawnPoint == null) {
            Debug.LogWarning(
                "Attack point is not assigned for: " +
                direction
            );

            return;
        }

        GameObject newAttack = Instantiate(
            handAttackPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        newAttack.transform.localScale =
            spawnPoint.localScale;

        DirectionalHandAttack attackScript =
            newAttack.GetComponent<DirectionalHandAttack>();

        if (attackScript == null) {
            Debug.LogWarning(
                "The attack prefab does not contain " +
                "DirectionalHandAttack."
            );

            Destroy(newAttack);
            return;
        }

        activeAttacks.Add(attackScript);

        attackScript.Initialize(
            this,
            player,
            direction
        );
    }

    private AttackDirection GetRandomDirection() {
        return (AttackDirection)Random.Range(0, 4);
    }

    private AttackDirection GetDifferentDirection(
        AttackDirection excludedDirection
    ) {
        AttackDirection newDirection;

        do {
            newDirection = GetRandomDirection();
        }
        while (newDirection == excludedDirection);

        return newDirection;
    }

    private Transform GetAttackPoint(
        AttackDirection direction
    ) {
        switch (direction) {
            case AttackDirection.Top:
                return attackFromTop;

            case AttackDirection.Bottom:
                return attackFromBottom;

            case AttackDirection.Left:
                return attackFromLeft;

            case AttackDirection.Right:
                return attackFromRight;

            default:
                return null;
        }
    }

    public void AttackFinished(
        DirectionalHandAttack finishedAttack
    ) {
        if (finishedAttack != null) {
            activeAttacks.Remove(finishedAttack);
        }
    }

    public void PlayerHit() {
        if (gameFinished)
            return;

        gameFinished = true;

        StopAllCoroutines();

        if (player != null) {
            player.SetActive(false);
        }

        DestroyAllAttacks();

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu() {
        yield return new WaitForSeconds(
            delayBeforeReturningToMenu
        );

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ShowFinalScreen(bool failed) {
        if (failed) {
            PlayerHit();
            return;
        }

        if (gameFinished)
            return;

        gameFinished = true;
        StopAllCoroutines();
        DestroyAllAttacks();

        if (finalScreen != null) {
            Text finalText =
                finalScreen.GetComponentInChildren<Text>();

            if (finalText != null) {
                finalText.text = "Success";
            }

            finalScreen.SetActive(true);
        }
    }

    private void DestroyAllAttacks() {
        for (int i = activeAttacks.Count - 1; i >= 0; i--) {
            if (activeAttacks[i] != null) {
                Destroy(activeAttacks[i].gameObject);
            }
        }

        activeAttacks.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (gameFinished)
            return;

        if (!collision.CompareTag("Player"))
            return;

        playerInStressZone = true;
        decayTimer = timeUntilDecay;

        float movementMultiplier = 1f;

        PlayerMovement playerMovement =
            collision.GetComponent<PlayerMovement>();

        if (playerMovement != null &&
            playerMovement.isTheFlyMoving) {
            movementMultiplier = 3f;
        }

        stressAmount +=
            stressIncreaseSpeed *
            movementMultiplier *
            Time.deltaTime;

        stressAmount = Mathf.Clamp01(stressAmount);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player"))
            return;

        playerInStressZone = true;
        decayTimer = timeUntilDecay;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Player"))
            return;

        playerInStressZone = false;
        attackTimer = initialTimeBetweenAttacks;
    }
}