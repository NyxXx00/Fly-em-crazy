using UnityEngine;
using UnityEngine.InputSystem;

public class ParryFly : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Transform[] lanes;
    public float moveSpeed = 10f;

    private int currentLane = 1;
    public int CurrentLane => currentLane;

    private bool isMoving;
    private Vector3 targetPosition;

    private void Start()
    {
        currentLane = Mathf.Clamp(currentLane, 0, lanes.Length - 1);

        transform.position = lanes[currentLane].position;
        targetPosition = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            ReadMovementInput();
        }

        MoveToLane();
    }

    private void ReadMovementInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame &&
            currentLane > 0)
        {
            StartMovement(currentLane - 1, true);
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame &&
                 currentLane < lanes.Length - 1)
        {
            StartMovement(currentLane + 1, false);
        }
    }

    private void StartMovement(int newLane, bool movingLeft)
    {
        currentLane = newLane;
        targetPosition = lanes[currentLane].position;
        isMoving = true;

        spriteRenderer.flipX = !movingLeft;
    }

    private void MoveToLane()
    {
        if (!isMoving)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) <= 0.001f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }
}