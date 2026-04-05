using UnityEngine;
using TMPro;

public class TimeSelector : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI hoursText;
    public TextMeshProUGUI minutesText;

    [Header("Connecting to the Timer")]
    public CountdownTimer countdownScript; // Connects to our new script
    public GameObject setupCanvas;         // To turn off when starting
    public GameObject countdownCanvas;     // To turn on when starting

    private int currentHours = 0;
    private int currentMinutes = 0;

    void Start()
    {
        UpdateDisplay();
    }

    // --- HOURS LOGIC ---
    public void IncreaseHours()
    {
        currentHours++;
        if (currentHours > 99) currentHours = 99;
        UpdateDisplay();
    }

    public void DecreaseHours()
    {
        currentHours--;
        if (currentHours < 0) currentHours = 0;
        UpdateDisplay();
    }

    // --- MINUTES LOGIC ---
    public void IncreaseMinutes()
    {
        currentMinutes += 1; 
        
        if (currentMinutes >= 60)
        {
            currentMinutes = 0; 
            IncreaseHours();    
        }
        UpdateDisplay();
    }

    public void DecreaseMinutes()
    {
        currentMinutes -= 1;
        
        if (currentMinutes < 0)
        {
            if (currentHours > 0)
            {
                currentMinutes = 55; 
                DecreaseHours();     
            }
            else
            {
                currentMinutes = 0;  
            }
        }
        UpdateDisplay();
    }

    // --- UPDATE THE VISUALS ---
    private void UpdateDisplay()
    {
        hoursText.text = currentHours.ToString("00");
        minutesText.text = currentMinutes.ToString("00");
    }

    // --- NEW: START THE TIMER ---
    public void StartLaundryTask()
    {
        // Prevent the user from starting a timer for 0 minutes
        if (currentHours == 0 && currentMinutes == 0) return;

        // 1. Send the time over to the countdown script
        countdownScript.BeginCountdown(currentHours, currentMinutes);

        // 2. Switch the Canvases
        setupCanvas.SetActive(false);
        countdownCanvas.SetActive(true);
    }
}