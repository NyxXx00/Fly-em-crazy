using UnityEngine;
using UnityEngine.InputSystem;

public class ParryMinigame : MonoBehaviour
{

    [SerializeField] private GameObject parryDisplay;

    [Header("Variables")]

    [SerializeField] private float timeForParry = 0.5f;
    [SerializeField] private float minTimeUntilParry = 4f;
    [SerializeField] private float maxTimeUntilParry = 10f;

    private float timeUntilParry;
    private bool parryDoable = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
    
        parryDoable = false;

        timeUntilParry = Random.Range(minTimeUntilParry, maxTimeUntilParry);

        await Awaitable.WaitForSecondsAsync(timeUntilParry);

        parryDoable = true;

        parryDisplay.GetComponent<SpriteRenderer>().color = Color.green;

        await Awaitable.WaitForSecondsAsync(timeForParry);

        parryDisplay.GetComponent<SpriteRenderer>().color = Color.red;
        parryDoable= false;

        Failed();

    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame) AttemptParry();
      
    }

    public void AttemptParry()
    {
        if (parryDoable) Success();
        else Failed();
    }

    public void Success()
    {
        print("success");
        Time.timeScale = 0;
    }

    public void Failed()
    {
        print("failed");
        Time.timeScale = 0;
    }


}
