using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f;

    private SpriteRenderer spriteRenderer;

    [Header("Flight")]
    public float waveAmplitude = 0.25f;
    public float waveFrequency = 12f;

    private Vector3 targetPosition;
    private float flightTime;

    void Start() {
        targetPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            targetPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            targetPosition.z = 0f;

        }

        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude > 0.05f) {
            direction.Normalize();

            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

            flightTime += Time.deltaTime;
            float wave = Mathf.Sin(flightTime * waveFrequency) * waveAmplitude;

            Vector3 nextPosition =
                transform.position +
                direction * moveSpeed * Time.deltaTime +
                perpendicular * wave * Time.deltaTime;

            transform.position = nextPosition;

            // Flip the sprite based on movement direction
            if (direction.x > 0) {
                spriteRenderer.flipX = true;
            }
            else if (direction.x < 0) {
                spriteRenderer.flipX = false;
            }
        }
    }
}