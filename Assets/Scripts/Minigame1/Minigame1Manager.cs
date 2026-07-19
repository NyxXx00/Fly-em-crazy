using UnityEngine;
using UnityEngine.UI;

public class Minigame1Manager : MonoBehaviour
{

    private Rigidbody2D rb;

    [Header("Stress")]

    [SerializeField] private Slider stressBar;
    [SerializeField] private float timeUntilDecay;

    private float stressAmount;
    private float timer;
    

    [Header("GameObjects")]

    [SerializeField] private GameObject handAttack;
    [SerializeField] private GameObject person;
    [SerializeField] private GameObject finalScreen;


    [Header("Sprites")]

    public Sprite[] personPortraits;

    private SpriteRenderer personSR;


    [Header("HandAttack")]

    [SerializeField] private float timeUntilTryAttack = 1;
    [SerializeField] private float chanceMultiplier = 1;

    private float attackTimer;

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


        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0) TryAttack();
        

    }

    public void TryAttack()
    {
        attackTimer = timeUntilTryAttack;

        if (Random.Range(0, 101) <= stressAmount * 100 * chanceMultiplier) 
        {
            Instantiate(handAttack);
        }
    }

    public void ShowFinalScreen(bool fails) 
    {
        finalScreen.GetComponentInChildren<Text>().text = fails ? "Failed" : "Success";
        finalScreen.SetActive(true); 
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            timer = timeUntilDecay;
            stressAmount += Time.deltaTime * 0.01f * (collision.GetComponent<PlayerMovement>().isTheFlyMoving ? 3 : 1);
        }
    }
}
