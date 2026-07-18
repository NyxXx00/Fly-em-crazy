using UnityEngine;
using UnityEngine.InputSystem;

public class ParryFly : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    public Transform[] lanes;
    public float moveSpeed = 10f;

    private int currentLane = 1;
    public int CurrentLane => currentLane;

    void Start() {
        transform.position = lanes[currentLane].position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) {
            currentLane--;
            spriteRenderer.flipX = true;
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) {
            currentLane++;
            spriteRenderer.flipX = false;
        }

        currentLane = Mathf.Clamp(currentLane, 0, lanes.Length - 1);

        transform.position = Vector3.MoveTowards(
            transform.position,
            lanes[currentLane].position,
            moveSpeed * Time.deltaTime
        );


    }
}
