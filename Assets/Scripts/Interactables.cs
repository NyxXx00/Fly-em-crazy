using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum InteractType { SceneLoader, MenuDisplayer, StressModifier }


[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Interactables : MonoBehaviour
{

    [Header("Settings")]

    [SerializeField] private bool isActive = true;
    [SerializeField] private InteractType type;

    [Header("variables")]

    [SerializeField] private int sceneNumber;
    [SerializeField] private GameObject menuToDisplay;
    

    private CircleCollider2D objCollider;
    private Rigidbody2D rb;

    private bool flyNearby;

    private void Awake()
    {
        objCollider= GetComponent<CircleCollider2D>();
        objCollider.isTrigger= true;
        rb= GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
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
        switch (type)
        {
            case InteractType.SceneLoader:
                SceneManager.LoadScene(sceneNumber);
                break;
            case InteractType.MenuDisplayer:
                menuToDisplay.SetActive(true);
                break;
            case InteractType.StressModifier:
                //TBA. aumenta, disminuye o afecta de alguna forma al progreso de la barra de estres
                break;


            default:
                SceneManager.LoadScene(0);
                break;
        }
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
