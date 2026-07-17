using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Interactables : MonoBehaviour
{

    private CircleCollider2D objCollider;
    private Rigidbody2D rb;

    private bool flyNearby;

    private void Awake()
    {
        objCollider= GetComponent<CircleCollider2D>();
        objCollider.isTrigger= true;
        rb= GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(flyNearby /*ToDo: && al pulsar E*/)
        {
            action();
        }
    }

    public void action()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        flyNearby= true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        flyNearby= false;
    }

}
