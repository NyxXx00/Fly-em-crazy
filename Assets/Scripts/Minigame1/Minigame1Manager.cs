using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Minigame1Manager : MonoBehaviour {
    private Rigidbody2D rb;
    private bool musicStarted;
    private AudioSource musicSource;

    [SerializeField] private AudioClip musicClip;

    [SerializeField] private GameObject darkOverlay;
    [SerializeField] private GameObject mainHUD;

    [Header("Sleeping")]
    [SerializeField] private bool sleep;
    [SerializeField] private float wakeUpThreshold;

    [Header("Stress")]
    [SerializeField] private Slider stressBar;
    [SerializeField] private float timeUntilDecay;
    [SerializeField] private float steadyBarSpeed;
    [SerializeField] private float movingBarSpeed = 2f;

    private float stressAmount;
    private float timer;

    [Header("Game Objects")]
    [SerializeField] private GameObject handAttack;
    [SerializeField] private GameObject person;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject finalScreen;

    [Header("Sprites")]
    [SerializeField] private Sprite[] personPortraits;

    private SpriteRenderer personSpriteRenderer;

    [Header("Hand Attack")]
    [SerializeField] private float timeUntilTryAttack = 1f;
    [SerializeField] private float chanceMultiplier = 1f;

    private float attackTimer;

    [Header("Additive Minigames")]
    [Tooltip("Write the exact names of the three minigame scenes.")]
    [SerializeField] private string[] minigameSceneNames;

    [SerializeField] private bool disablePlayerWhilePlaying = true;

    private bool minigameIsLoading;
    private string currentMinigameScene;

    private void Start() {
        musicSource = GetComponent<AudioSource>();

        if (musicSource != null && musicClip != null) {
            musicSource.clip = musicClip;
            musicClip.LoadAudioData();
        }

        if (person != null) {
            personSpriteRenderer =
                person.GetComponent<SpriteRenderer>();
        }

        rb = GetComponent<Rigidbody2D>();
        timer = timeUntilDecay;
        attackTimer = timeUntilTryAttack;

        if (stressBar != null) {
            stressBar.value = stressAmount;
        }
    }

    private void Update() {
        if (minigameIsLoading)
            return;

        timer -= Time.deltaTime;

        if (timer < 0f) {
            timer = 0f;
        }

        if (timer <= 0f) {
            stressAmount -= Time.deltaTime * 0.01f;
        }

        stressAmount = Mathf.Clamp01(stressAmount);

        if (stressBar != null) {
            stressBar.value = stressAmount;
        }

        if (!sleep) {
            UpdatePersonPortrait();

            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f) {
                TryAttack();
            }
        }
        else if (stressAmount >= wakeUpThreshold / 100f) {
            sleep = false;
        }
    }

    private void UpdatePersonPortrait() {
        if (personSpriteRenderer == null ||
            personPortraits == null ||
            personPortraits.Length == 0) {
            return;
        }

        int portraitIndex = Mathf.FloorToInt(
            (personPortraits.Length - 1) * stressAmount
        );

        portraitIndex = Mathf.Clamp(
            portraitIndex,
            0,
            personPortraits.Length - 1
        );

        personSpriteRenderer.sprite =
            personPortraits[portraitIndex];
    }

    public void TryAttack() {
        attackTimer = timeUntilTryAttack;

        float attackChance =
            stressAmount * 100f * chanceMultiplier;

        if (Random.Range(0f, 100f) <= attackChance) {
            Instantiate(handAttack);
        }
    }

    public void FlyWasHit() {
        if (minigameIsLoading)
            return;

        StartCoroutine(LoadRandomMinigame());
    }

    private IEnumerator LoadRandomMinigame() {
        if (minigameSceneNames == null ||
            minigameSceneNames.Length == 0) {
            Debug.LogError(
                "No minigame scenes have been assigned."
            );

            yield break;
        }

        minigameIsLoading = true;

        int randomIndex = Random.Range(
            0,
            minigameSceneNames.Length
        );

        currentMinigameScene =
            minigameSceneNames[randomIndex];

        if (string.IsNullOrWhiteSpace(currentMinigameScene)) {
            Debug.LogError(
                "One of the minigame scene names is empty."
            );

            minigameIsLoading = false;
            yield break;
        }

        if (disablePlayerWhilePlaying && player != null) {
            player.SetActive(false);
        }

        if (mainHUD != null) {
            mainHUD.SetActive(false);
        }

        if (darkOverlay != null) {
            darkOverlay.SetActive(true);
        }

        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(
                currentMinigameScene,
                LoadSceneMode.Additive
            );

        if (loadOperation == null) {
            Debug.LogError(
                "The minigame scene could not be loaded: " +
                currentMinigameScene
            );

            minigameIsLoading = false;

            if (player != null) {
                player.SetActive(true);
            }

            yield break;
        }

        while (!loadOperation.isDone) {
            yield return null;
        }

        Scene loadedScene =
            SceneManager.GetSceneByName(
                currentMinigameScene
            );

        if (loadedScene.IsValid()) {
            SceneManager.SetActiveScene(loadedScene);
        }
    }

    public void CloseCurrentMinigame() {
        if (string.IsNullOrEmpty(currentMinigameScene))
            return;

        StartCoroutine(UnloadCurrentMinigame());
    }

    private IEnumerator UnloadCurrentMinigame() {
        AsyncOperation unloadOperation =
            SceneManager.UnloadSceneAsync(
                currentMinigameScene
            );

        if (unloadOperation != null) {
            while (!unloadOperation.isDone) {
                yield return null;
            }
        }

        currentMinigameScene = string.Empty;
        minigameIsLoading = false;

        SceneManager.SetActiveScene(gameObject.scene);

        if (player != null) {
            player.SetActive(true);
        }

        if (mainHUD != null) {
            mainHUD.SetActive(true);
        }

        if (darkOverlay != null) {
            darkOverlay.SetActive(false);
        }

        attackTimer = timeUntilTryAttack;
    }

    public void ShowFinalScreen(bool fails) {
        if (finalScreen == null)
            return;

        Text finalText =
            finalScreen.GetComponentInChildren<Text>();

        if (finalText != null) {
            finalText.text =
                fails ? "Failed" : "Success";
        }

        finalScreen.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Player"))
            return;

        if (!musicStarted) {
            musicStarted = true;

            if (stressBar != null) {
                stressBar.gameObject.SetActive(true);
            }

            if (musicSource != null &&
                musicSource.clip != null) {
                musicSource.Play();
            }
        }

        timer = timeUntilDecay;

        PlayerMovement playerMovement =
            collision.GetComponent<PlayerMovement>();

        bool playerIsMoving =
            playerMovement != null &&
            playerMovement.isTheFlyMoving;

        float currentBarSpeed =
            playerIsMoving
                ? movingBarSpeed
                : steadyBarSpeed;

        stressAmount +=
            Time.deltaTime *
            0.01f *
            currentBarSpeed;

        stressAmount = Mathf.Clamp01(stressAmount);
    }
}