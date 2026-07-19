using UnityEngine;
using UnityEngine.UI;

public class HandAttack : MonoBehaviour
{
    [Header("Entities")]

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject minigameManager;

    [Header("Parameters")]

    [SerializeField] private bool autoSetLifetime;
    [SerializeField] private float timeUntilAttack;
    [SerializeField] private float lifeTime;

    [Header("Sprites")]

    public Sprite hand;

    private bool flyInHitbox;

    private void Awake()
    {
        minigameManager = GameObject.Find("MinigameManager");
        player = GameObject.Find("Fly");
        transform.position = player.transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lifeTime = autoSetLifetime ? timeUntilAttack + 0.5f : lifeTime;
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilAttack-= Time.deltaTime;

        if(timeUntilAttack <= 0) Attack();
    }

    public void Attack()
    {
        GetComponent<SpriteRenderer>().sprite = hand;
        timeUntilAttack = 100;
        if (flyInHitbox) Failed();
    }

    public void Failed()
    {
        player.SetActive(false);
        minigameManager.GetComponent<Minigame1Manager>().ShowFinalScreen(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            flyInHitbox= true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            flyInHitbox = false;
        }

    }
}
