using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackManager : MonoBehaviour {
    [SerializeField] private TMP_Text messageText;

    private int currentStage = 1;
    private bool doubleAttackUnlocked = false;
    private bool gameFinished = false;

    [Header("Lane Positions")]
    public Transform[] lanes;

    [Header("Warnings")]
    public GameObject[] warnings;
    public float warningHeight = 1f;
    public float warningDuration = 1f;
    public int warningBlinks = 3;

    [Header("Fly")]
    public ParryFly fly;

    [Header("Fists")]
    public Transform[] fists;
    public Transform[] fistStartPositions;
    public Transform[] fistEndPositions;

    public float fistDownSpeed = 15f;
    public float fistUpSpeed = 10f;

    [Header("Stress")]
    [SerializeField] private Slider stressBar;
    [SerializeField] private float stressPerDodge = 0.05f;
    [SerializeField] private float maxStress = 1f;

    private float currentStress;

    [Header("Difficulty Stages")]
    public float stageOneAttackInterval = 2f;
    public float stageTwoAttackInterval = 1.3f;
    public float stageThreeAttackInterval = 0.8f;
    public float stageFourAttackInterval = 0.45f;

    [Range(0f, 1f)]
    public float doublePunchThreshold = 0.9f;

    private float timeBetweenAttacks;

    void Start() {
        messageText.text = "";
        messageText.gameObject.SetActive(false);

        foreach (GameObject currentWarning in warnings) {
            currentWarning.SetActive(false);
        }

        foreach (Transform currentFist in fists) {
            currentFist.gameObject.SetActive(false);
        }

        currentStress = 0f;

        stressBar.minValue = 0f;
        stressBar.maxValue = maxStress;
        stressBar.value = currentStress;

        timeBetweenAttacks = stageOneAttackInterval;

        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop() {
        while (!gameFinished) {
            yield return new WaitForSeconds(timeBetweenAttacks);

            int firstLane = fly.CurrentLane;

            if (doubleAttackUnlocked) {
                int secondLane;

                do {
                    secondLane = Random.Range(0, lanes.Length);
                }
                while (secondLane == firstLane);

                yield return StartCoroutine(
                    ShowWarnings(firstLane, secondLane)
                );

                Coroutine firstPunch =
                    StartCoroutine(Punch(fists[0], firstLane));

                Coroutine secondPunch =
                    StartCoroutine(Punch(fists[1], secondLane));

                yield return firstPunch;
                yield return secondPunch;
            }
            else {
                yield return StartCoroutine(
                    ShowWarnings(firstLane)
                );

                yield return StartCoroutine(
                    Punch(fists[0], firstLane)
                );
            }
        }
    }

    IEnumerator ShowWarnings(int firstLane, int secondLane = -1) {
        warnings[0].transform.position =
            lanes[firstLane].position +
            new Vector3(0f, warningHeight, 0f);

        bool showDoubleWarning =
            secondLane >= 0 &&
            warnings.Length >= 2;

        if (showDoubleWarning) {
            warnings[1].transform.position =
                lanes[secondLane].position +
                new Vector3(0f, warningHeight, 0f);
        }

        float blinkTime =
            warningDuration / (warningBlinks * 2f);

        for (int i = 0; i < warningBlinks; i++) {
            warnings[0].SetActive(true);

            if (showDoubleWarning) {
                warnings[1].SetActive(true);
            }

            yield return new WaitForSeconds(blinkTime);

            warnings[0].SetActive(false);

            if (showDoubleWarning) {
                warnings[1].SetActive(false);
            }

            yield return new WaitForSeconds(blinkTime);
        }

        warnings[0].SetActive(false);

        if (warnings.Length >= 2) {
            warnings[1].SetActive(false);
        }
    }

    IEnumerator Punch(Transform selectedFist, int attackLane) {
        Transform startPosition =
            fistStartPositions[attackLane];

        Transform endPosition =
            fistEndPositions[attackLane];

        selectedFist.position = startPosition.position;
        selectedFist.gameObject.SetActive(true);

        while (
            Vector3.Distance(
                selectedFist.position,
                endPosition.position
            ) > 0.01f
        ) {
            selectedFist.position = Vector3.MoveTowards(
                selectedFist.position,
                endPosition.position,
                fistDownSpeed * Time.deltaTime
            );

            yield return null;
        }

        if (fly.CurrentLane == attackLane) {
            Debug.Log("Fly hit!");
        }
        else {
            Debug.Log("Fly dodged!");
            IncreaseStress();
        }

        while (
            Vector3.Distance(
                selectedFist.position,
                startPosition.position
            ) > 0.01f
        ) {
            selectedFist.position = Vector3.MoveTowards(
                selectedFist.position,
                startPosition.position,
                fistUpSpeed * Time.deltaTime
            );

            yield return null;
        }

        selectedFist.gameObject.SetActive(false);
    }

    void IncreaseStress() {
        if (gameFinished)
            return;

        currentStress += stressPerDodge;

        currentStress = Mathf.Clamp(
            currentStress,
            0f,
            maxStress
        );

        stressBar.value = currentStress;

        UpdateDifficultyStage();

        if (currentStress >= maxStress) {
            gameFinished = true;

            StartCoroutine(
                FinishMinigame()
            );
        }
    }

    void UpdateDifficultyStage() {
        float stressPercentage =
            currentStress / maxStress;

        if (stressPercentage >= doublePunchThreshold) {
            timeBetweenAttacks = stageFourAttackInterval;

            if (!doubleAttackUnlocked) {
                doubleAttackUnlocked = true;
                currentStage = 5;

                StartCoroutine(
                    ShowMessage("DOUBLE ATTACK!", 2f)
                );
            }

            return;
        }

        if (stressPercentage >= 0.75f) {
            timeBetweenAttacks = stageFourAttackInterval;

            if (currentStage < 4) {
                currentStage = 4;

                StartCoroutine(
                    ShowMessage("FINAL PHASE!", 1.5f)
                );
            }
        }
        else if (stressPercentage >= 0.50f) {
            timeBetweenAttacks = stageThreeAttackInterval;

            if (currentStage < 3) {
                currentStage = 3;

                StartCoroutine(
                    ShowMessage("STRESS LEVEL 3", 1.5f)
                );
            }
        }
        else if (stressPercentage >= 0.25f) {
            timeBetweenAttacks = stageTwoAttackInterval;

            if (currentStage < 2) {
                currentStage = 2;

                StartCoroutine(
                    ShowMessage("STRESS LEVEL 2", 1.5f)
                );
            }
        }
        else {
            timeBetweenAttacks = stageOneAttackInterval;
        }
    }

    IEnumerator ShowMessage(string message, float duration) {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        messageText.gameObject.SetActive(false);
    }

    IEnumerator FinishMinigame() {
        foreach (GameObject currentWarning in warnings) {
            currentWarning.SetActive(false);
        }

        foreach (Transform currentFist in fists) {
            currentFist.gameObject.SetActive(false);
        }

        messageText.text = "JOHN SNAPPED!";
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        Debug.Log("Minigame completed!");
    }
}