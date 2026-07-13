using UnityEngine;
using UnityEngine.UI;

public class Minigame1Manager : MonoBehaviour
{

    private Rigidbody2D rb;

    [Header("Stress")]

    [SerializeField] private Slider stressBar;

    [SerializeField] private float stressAmount;

    [SerializeField] private float timeUntilDecay;
    [SerializeField] private float timer;

    [Header("GameObjects")]

    [SerializeField] private GameObject handAttack;
    [SerializeField] private GameObject person;


    [Header("Sprites")]

    public Sprite[] personPortraits;

    private SpriteRenderer personSR;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        personSR= person.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        timer = timeUntilDecay;
    }

    // Update is called once per frame
    void Update()
    {
        timer-= Time.deltaTime;
        if (timer < 0) timer= 0;
        if(timer <= 0)
        {
            stressAmount -= Time.deltaTime*0.01f;
        }

        if(stressAmount<= 0) stressAmount= 0;

        stressBar.value = stressAmount;

        personSR.sprite = personPortraits[(int)((personPortraits.Length-1) * stressAmount)];



        

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            timer = timeUntilDecay;
            print(collision.GetComponent<PlayerMovement>().isTheFlyMoving);
            stressAmount += Time.deltaTime * 0.01f * (collision.GetComponent<PlayerMovement>().isTheFlyMoving ? 3 : 1);
        }
    }
}
