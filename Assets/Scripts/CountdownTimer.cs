using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI Box References")]
    // The Left Box
    public TextMeshProUGUI leftValueText;
    public TextMeshProUGUI leftLabelText;
    
    // The Right Box
    public TextMeshProUGUI rightValueText;
    public TextMeshProUGUI rightLabelText;

    [Header("Canvas Transitions")]
    public GameObject countdownCanvas;
    public GameObject congratsCanvas;

    private float timeRemainingInSeconds;
    private bool isTimerRunning = false;

    // Called by your Setup screen
    public void BeginCountdown(int hours, int minutes)
    {
        // 1. Force the screen sideways the moment the timer starts!
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        timeRemainingInSeconds = (hours * 3600f) + (minutes * 60f);
        isTimerRunning = true;
        UpdateDisplay();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timeRemainingInSeconds -= Time.deltaTime;

            if (timeRemainingInSeconds <= 0)
            {
                timeRemainingInSeconds = 0;
                isTimerRunning = false;
                FinishTimer();
            }

            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        // Calculate raw hours, minutes, and seconds
        int h = Mathf.FloorToInt(timeRemainingInSeconds / 3600f);
        int m = Mathf.FloorToInt((timeRemainingInSeconds % 3600f) / 60f);
        int s = Mathf.FloorToInt(timeRemainingInSeconds % 60f);

        Debug.Log("Time Remaining: " + h + ":" + m + ":" + s);

        // Dynamic Display Logic
        if (h > 0)
        {
            // If there is an hour or more left, show HH:MM
            leftValueText.text = h.ToString("00");
            leftLabelText.text = "HOURS";

            rightValueText.text = m.ToString("00");
            rightLabelText.text = "MINUTES";
        }
        else
        {
            // If it drops under an hour, switch to showing MM:SS
            leftValueText.text = m.ToString("00");
            leftLabelText.text = "MINUTES";

            rightValueText.text = s.ToString("00");
            rightLabelText.text = "SECONDS";
        }
    }

    public void AddTime(int minutesToAdd)
    {
        // Only allow adding time if the timer is actually running
        if (isTimerRunning)
        {
            // Convert the minutes to seconds and add it to the total
            timeRemainingInSeconds += (minutesToAdd * 60f);
            
            // Instantly update the UI so the player sees the numbers jump up
            UpdateDisplay();
        }
    }

    private void FinishTimer()
    {
        // 2. Force the screen back upright the moment the timer hits zero!
        Screen.orientation = ScreenOrientation.Portrait;

        // 3. Swap the Canvases
        countdownCanvas.SetActive(false);
        congratsCanvas.SetActive(true);
    }
}