using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    public float velocidad = 5f;

    [Header("Vuelo")]
    public float amplitud = 0.25f;
    public float frecuencia = 12f;

    private Vector3 objetivo;
    private float tiempoVuelo;

    void Start() {
        objetivo = transform.position;
    }

    void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            objetivo = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            objetivo.z = 0f;

            tiempoVuelo = 0f;
        }

        Vector3 direccion = (objetivo - transform.position);

        if (direccion.magnitude > 0.05f) {
            direccion.Normalize();

            Vector3 perpendicular = new Vector3(-direccion.y, direccion.x, 0);

            tiempoVuelo += Time.deltaTime;
            float onda = Mathf.Sin(tiempoVuelo * frecuencia) * amplitud;

            Vector3 siguientePos =
                transform.position +
                direccion * velocidad * Time.deltaTime +
                perpendicular * onda * Time.deltaTime;

            transform.position = siguientePos;

            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angulo - 90);
        }
    }
}