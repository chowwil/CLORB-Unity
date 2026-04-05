using UnityEngine;
using TMPro; // Required for TextMeshPro

public class TimeSelector : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI hoursText;
    public TextMeshProUGUI minutesText;

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
        if (currentHours > 99) currentHours = 99; // Cap at 99 hours
        UpdateDisplay();
    }

    public void DecreaseHours()
    {
        currentHours--;
        if (currentHours < 0) currentHours = 0; // Don't go below 0
        UpdateDisplay();
    }

    // --- MINUTES LOGIC ---
    public void IncreaseMinutes()
    {
        currentMinutes += 5; // Jumping by 5 minutes is usually best for productivity timers
        
        if (currentMinutes >= 60)
        {
            currentMinutes = 0; // Reset to 0
            IncreaseHours();    // Automatically bump up the hour!
        }
        UpdateDisplay();
    }

    public void DecreaseMinutes()
    {
        currentMinutes -= 5;
        
        if (currentMinutes < 0)
        {
            if (currentHours > 0)
            {
                currentMinutes = 55; // Roll back to 55
                DecreaseHours();     // Drop an hour
            }
            else
            {
                currentMinutes = 0;  // Stuck at 00:00
            }
        }
        UpdateDisplay();
    }

    // --- UPDATE THE VISUALS ---
    private void UpdateDisplay()
    {
        // "00" ensures that numbers less than 10 have a leading zero (e.g., "05")
        hoursText.text = currentHours.ToString("00");
        minutesText.text = currentMinutes.ToString("00");
    }
}