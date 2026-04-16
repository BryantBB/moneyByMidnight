using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Betting bettingScript;

    private float Speed;
    public float maxSpeed = 600;
    public bool isSpin = true;

    public GameObject pointer;    
    private Pointer pointerScript;
    private bool hasCheckedWinner = false;

    void Start()
    {
        RouletteSoundManager.LoopSound(RouletteSound.SPIN);

        Speed = maxSpeed;
        if (pointer != null)
        {
            pointerScript = pointer.GetComponent<Pointer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }


    void Rotate()
    {
        transform.Rotate(0,0, Speed * Time.deltaTime);

        if (isSpin == false && Speed > 0)
        {
            StopWheel();
        }
    }
    public void TriggerStop()
    {
        isSpin = false;
    }
    
    public void StopWheel()
    {   
        RouletteSoundManager.StopLoop();
        RouletteSoundManager.PlaySound(RouletteSound.SLOW);

        if (bettingScript.totalBet > 0)
        {
            Speed -= 100 * Time.deltaTime;
            if (Speed <= 0)
            {
                Speed = 0;

                if (!hasCheckedWinner)
                {
                    // Enable the collider so OverlapBox can see it
                    pointer.GetComponent<BoxCollider2D>().enabled = true;
                    
                    if (pointerScript != null)
                    {
                        pointerScript.PrintWinningSection();
                    }
                    hasCheckedWinner = true; 
                }
            }
        }
        else
        {
            Debug.Log("<color=orange>Action Blocked:</color> You must place a bet before spinning!");
            // Optional: Trigger a UI message that says "Place a bet first!"
        }
    }

    public void Reset()
    {
        isSpin = true;
        hasCheckedWinner = false; 
        pointer.GetComponent<BoxCollider2D>().enabled = false;
        Speed = Random.Range(500f, 900f);
    
        // 2. Disable the pointer collider until the wheel stops again
        if (pointer != null)
        {
        pointer.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
